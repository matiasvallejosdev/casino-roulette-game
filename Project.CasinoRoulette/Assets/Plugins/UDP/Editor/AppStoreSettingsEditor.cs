using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine.Networking;
using UnityEngine.UDP.Editor.Analytics;

namespace UnityEngine.UDP.Editor
{
#if (UNITY_5_6_OR_NEWER && !UNITY_5_6_0)
    [CustomEditor(typeof(AppStoreSettings))]
    public class AppStoreSettingsEditor : UnityEditor.Editor
    {
        [MenuItem("Window/Unity Distribution Portal/Settings", false, 111)]
        public static void CreateAppStoreSettingsAsset()
        {
            if (File.Exists(AppStoreSettings.appStoreSettingsAssetPath))
            {
                AppStoreSettings existedAppStoreSettings = CreateInstance<AppStoreSettings>();
                existedAppStoreSettings =
                    (AppStoreSettings) AssetDatabase.LoadAssetAtPath(AppStoreSettings.appStoreSettingsAssetPath,
                        typeof(AppStoreSettings));
                EditorUtility.FocusProjectWindow();
                Selection.activeObject = existedAppStoreSettings;
                return;
            }

            if (!Directory.Exists(AppStoreSettings.appStoreSettingsAssetFolder))
                Directory.CreateDirectory(AppStoreSettings.appStoreSettingsAssetFolder);

            var appStoreSettings = CreateInstance<AppStoreSettings>();
            AssetDatabase.CreateAsset(appStoreSettings, AppStoreSettings.appStoreSettingsAssetPath);
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = appStoreSettings;

            // Go to Inspector Window
#if UNITY_2018_2_OR_NEWER
            EditorApplication.ExecuteMenuItem("Window/General/Inspector");
#else
            EditorApplication.ExecuteMenuItem("Window/Inspector");
#endif
        }

        [MenuItem("Window/Unity Distribution Portal/Settings", true)]
        public static bool CheckUnityOAuthValidation()
        {
            return EnableOAuth;
        }

        private string _clientSecretInMemory;
        private string _callbackUrlInMemory;
        private List<string> pushRequestList = new List<string>();

        private static readonly bool EnableOAuth =
            Utils.FindTypeByName("UnityEditor.Connect.UnityOAuth") != null;

        private string _callbackUrlLast;

        private const string STEP_GET_CLIENT = "get_client";
        private const string STEP_UPDATE_CLIENT = "update_client";
        private const string STEP_UPDATE_CLIENT_SECRET = "update_client_secret";

        private const string STEP_UPDATE_GAME_TITLE = "update_game_title";

        private List<TestAccount> _testAccounts = new List<TestAccount>();
        private List<bool> _testAccountsDirty = new List<bool>();
        private List<string> _testAccountsValidationMsg = new List<string>();

        private AppItem _currentAppItem;
        private string _targetStep;
        private bool _clientChecked = false;

        public struct ReqStruct
        {
            public string currentStep;
            public string targetStep;
            public string eventName;
            public IapItem curIapItem;
            public TestAccount currTestAccount;
            public int arrayPos;
            public UnityWebRequest request;
            public GeneralResponse resp;
        }

        private Queue<ReqStruct> _requestQueue = new Queue<ReqStruct>();

        private SerializedProperty _unityProjectId;
        private SerializedProperty _unityClientId;
        private SerializedProperty _unityClientKey;
        private SerializedProperty _unityClientRsaPublicKey;
        private SerializedProperty _appName;
        private SerializedProperty _appSlug;
        private SerializedProperty _appItemId;

        private bool _isOperationRunning = false; // Lock all panels
        private bool _isIapUpdating = false; // Lock iap part.
        private bool _isTestAccountUpdating = false; // Lock testAccount part.
        private State _currentState = State.Success;

        void OnEnable()
        {
            // For unity client settings.
            _unityProjectId = serializedObject.FindProperty("UnityProjectID");
            _unityClientId = serializedObject.FindProperty("UnityClientID");
            _unityClientKey = serializedObject.FindProperty("UnityClientKey");
            _unityClientRsaPublicKey = serializedObject.FindProperty("UnityClientRSAPublicKey");
            _appName = serializedObject.FindProperty("AppName");
            _appSlug = serializedObject.FindProperty("AppSlug");
            _appItemId = serializedObject.FindProperty("AppItemId");

            _testAccounts = new List<TestAccount>();
            _currentAppItem = new AppItem();

            EditorApplication.update += CheckRequestUpdate;

            if (!EnableOAuth)
            {
                _currentState = State.CannotUseOAuth;
                return;
            }

            if (!string.IsNullOrEmpty(Application.cloudProjectId))
            {
                _cloudProjectId = Application.cloudProjectId;
                InitializeSecrets();
            }
            else
            {
                _currentState = State.CannotGetCloudProjectId;
            }
        }


        enum State
        {
            CannotUseOAuth,
            CannotGetCloudProjectId,
            LinkProject,
            Success,
        }

        void CheckRequestUpdate()
        {
            if (_requestQueue.Count == 0)
            {
                return;
            }

            ReqStruct reqStruct = _requestQueue.Dequeue();
            UnityWebRequest request = reqStruct.request;
            GeneralResponse resp = reqStruct.resp;

            if (request != null && request.isDone)
            {
                if (request.error != null || request.responseCode / 100 != 2)
                {
                    // Deal with errors
                    if (request.downloadHandler.text.Contains(AppStoreOnboardApi.invalidAccessTokenInfo)
                        || request.downloadHandler.text.Contains(AppStoreOnboardApi.forbiddenInfo)
                        || request.downloadHandler.text.Contains(AppStoreOnboardApi.expiredAccessTokenInfo))
                    {
                        UnityWebRequest newRequest = AppStoreOnboardApi.RefreshToken();
                        TokenInfo tokenInfoResp = new TokenInfo();
                        ReqStruct newReqStruct = new ReqStruct();
                        newReqStruct.request = newRequest;
                        newReqStruct.resp = tokenInfoResp;
                        newReqStruct.targetStep = reqStruct.targetStep;
                        _requestQueue.Enqueue(newReqStruct);
                    }
                    else if (request.downloadHandler.text.Contains(AppStoreOnboardApi.invalidRefreshTokenInfo)
                             || request.downloadHandler.text.Contains(AppStoreOnboardApi.expiredRefreshTokenInfo))
                    {
                        if (reqStruct.targetStep == "LinkProject")
                        {
                            _targetStep = reqStruct.targetStep;
                        }
                        else
                        {
                            _targetStep = STEP_GET_CLIENT;
                        }

                        AppStoreOnboardApi.tokenInfo.access_token = null;
                        AppStoreOnboardApi.tokenInfo.refresh_token = null;
                        CallApiAsync();
                    }
                    else
                    {
                        _isOperationRunning = false;
                        _isIapUpdating = false;
                        _isTestAccountUpdating = false;
                        ErrorResponse response = JsonUtility.FromJson<ErrorResponse>(request.downloadHandler.text);

                        #region Analytics Fails

                        if (resp.GetType() == typeof(EventRequestResponse))
                        {
                            // Debug.Log("[Debug] Event Request Failed: " + reqStruct.eventName);
                            return; // Do not show error dialog
                        }

                        if (resp.GetType() == typeof(UnityClientResponse))
                        {
                            string eventName = null;
                            switch (request.method)
                            {
                                case UnityWebRequest.kHttpVerbPOST:
                                    eventName = EditorAnalyticsApi.k_ClientCreateEventName;
                                    break;
                                case UnityWebRequest.kHttpVerbPUT:
                                    eventName = EditorAnalyticsApi.k_ClientUpdateEventName;
                                    break;
                                default:
                                    eventName = null;
                                    break;
                            }

                            if (eventName != null)
                            {
                                UnityWebRequest analyticsRequest =
                                    EditorAnalyticsApi.ClientEvent(eventName, null, response.message);

                                ReqStruct analyticsReqStruct = new ReqStruct
                                {
                                    request = analyticsRequest,
                                    resp = new EventRequestResponse(),
                                    eventName = eventName,
                                };

                                _requestQueue.Enqueue(analyticsReqStruct);
                            }
                        }

                        if (resp.GetType() == typeof(AppItemResponse))
                        {
                            string eventName;
                            switch (request.method)
                            {
                                case UnityWebRequest.kHttpVerbPOST:
                                    eventName = EditorAnalyticsApi.k_AppCreateEventName;
                                    break;
                                case UnityWebRequest.kHttpVerbPUT:
                                    eventName = EditorAnalyticsApi.k_AppUpdateEventName;
                                    break;
                                default:
                                    eventName = null;
                                    break;
                            }

                            if (eventName != null)
                            {
                                UnityWebRequest analyticsRequest =
                                    EditorAnalyticsApi.AppEvent(eventName, _unityClientId.stringValue, null,
                                        response.message);

                                ReqStruct analyticsRequestStruct = new ReqStruct
                                {
                                    request = analyticsRequest,
                                    resp = new EventRequestResponse(),
                                    eventName = eventName,
                                };

                                _requestQueue.Enqueue(analyticsRequestStruct);
                            }
                        }

                        #endregion

                        ProcessErrorRequest(reqStruct);
                        if (response != null && response.message != null && response.details != null &&
                            response.details.Length != 0)
                        {
                            Debug.LogError("[UDP] " + response.details[0].field + ": " + response.message);
                            EditorUtility.DisplayDialog("Error",
                                response.details[0].field + ": " + response.message,
                                "OK");
                            if (response.message == "Project not found")
                            {
                                _currentState = State.CannotGetCloudProjectId;
                            }
                        }
                        else if (response != null && response.message != null)
                        {
                            Debug.LogError("[UDP] " + response.message);
                            EditorUtility.DisplayDialog("Error",
                                response.message,
                                "OK");
                        }
                        else
                        {
                            Debug.LogError("[UDP] Network error, no response received.");
                            EditorUtility.DisplayDialog("Error",
                                "Network error, no response received",
                                "OK");
                        }

                        this.Repaint();
                    }
                }
                else
                {
                    if (resp.GetType() == typeof(UnityClientResponse))
                    {
                        // LinkProject & Get Role (later action) will result in this response.
                        resp = JsonUtility.FromJson<UnityClientResponse>(request.downloadHandler.text);
                        _unityClientId.stringValue = ((UnityClientResponse) resp).client_id;
                        _unityClientKey.stringValue = ((UnityClientResponse) resp).client_secret;
                        _unityClientRsaPublicKey.stringValue = ((UnityClientResponse) resp).channel.publicRSAKey;
                        _unityProjectId.stringValue = ((UnityClientResponse) resp).channel.projectGuid;
                        _clientSecretInMemory = ((UnityClientResponse) resp).channel.channelSecret;
                        _callbackUrlInMemory = ((UnityClientResponse) resp).channel.callbackUrl;
                        _callbackUrlLast = _callbackUrlInMemory;
                        AppStoreOnboardApi.tps = ((UnityClientResponse) resp).channel.thirdPartySettings;
                        AppStoreOnboardApi.updateRev = ((UnityClientResponse) resp).rev;
                        serializedObject.ApplyModifiedProperties();
                        this.Repaint();
                        AssetDatabase.SaveAssets();
                        saveGameSettingsProps(((UnityClientResponse) resp).client_id);

                        if (request.method == UnityWebRequest.kHttpVerbPOST) // Generated Client
                        {
                            UnityWebRequest analyticsRequest =
                                EditorAnalyticsApi.ClientEvent(EditorAnalyticsApi.k_ClientCreateEventName,
                                    ((UnityClientResponse) resp).client_id, null);

                            ReqStruct analyticsReqStruct = new ReqStruct
                            {
                                request = analyticsRequest,
                                resp = new EventRequestResponse(),
                                eventName = EditorAnalyticsApi.k_ClientCreateEventName,
                            };

                            _requestQueue.Enqueue(analyticsReqStruct);
                        }
                        else if (request.method == UnityWebRequest.kHttpVerbPUT) // Updated Client
                        {
                            UnityWebRequest analyticsRequest =
                                EditorAnalyticsApi.ClientEvent(EditorAnalyticsApi.k_ClientUpdateEventName,
                                    ((UnityClientResponse) resp).client_id, null);

                            ReqStruct analyticsReqStruct = new ReqStruct
                            {
                                request = analyticsRequest,
                                resp = new EventRequestResponse(),
                                eventName = EditorAnalyticsApi.k_ClientUpdateEventName,
                            };

                            _requestQueue.Enqueue(analyticsReqStruct);
                        }

                        if (reqStruct.targetStep == "LinkProject")
                        {
                            UnityClientInfo unityClientInfo = new UnityClientInfo();
                            //                            unityClientInfo.ClientId = unityClientID.stringValue;
                            unityClientInfo.ClientId = _clientIdToBeLinked;
                            UnityWebRequest newRequest =
                                AppStoreOnboardApi.UpdateUnityClient(Application.cloudProjectId, unityClientInfo,
                                    _callbackUrlInMemory);
                            UnityClientResponse clientResp = new UnityClientResponse();
                            ReqStruct newReqStruct = new ReqStruct();
                            newReqStruct.request = newRequest;
                            newReqStruct.resp = clientResp;
                            newReqStruct.targetStep = "GetRole";
                            _requestQueue.Enqueue(newReqStruct);
                        }
                        else if (reqStruct.targetStep == "GetRole")
                        {
                            UnityWebRequest newRequest = AppStoreOnboardApi.GetUserId();
                            UserIdResponse userIdResp = new UserIdResponse();
                            ReqStruct newReqStruct = new ReqStruct();
                            newReqStruct.request = newRequest;
                            newReqStruct.resp = userIdResp;
                            newReqStruct.targetStep = STEP_GET_CLIENT;
                            _requestQueue.Enqueue(newReqStruct);
                        }
                        else
                        {
                            if (reqStruct.targetStep == STEP_UPDATE_CLIENT)
                            {
                                EditorUtility.DisplayDialog("Hint",
                                    "Unity Client updated successfully.",
                                    "OK");
                                RemovePushRequest(_unityClientId.stringValue);
                                _callbackUrlChanged = false;
                            }

                            if (_currentAppItem.status == "STAGE")
                            {
                                UnityWebRequest newRequest = AppStoreOnboardApi.UpdateAppItem(_currentAppItem);
                                AppItemResponse appItemResponse = new AppItemResponse();
                                ReqStruct newReqStruct = new ReqStruct();
                                newReqStruct.request = newRequest;
                                newReqStruct.resp = appItemResponse;
                                _requestQueue.Enqueue(newReqStruct);
                            }
                            else
                            {
                                UnityWebRequest newRequest = AppStoreOnboardApi.GetAppItem(_unityClientId.stringValue);
                                AppItemResponseWrapper appItemResponseWrapper = new AppItemResponseWrapper();
                                ReqStruct newReqStruct = new ReqStruct();
                                newReqStruct.request = newRequest;
                                newReqStruct.resp = appItemResponseWrapper;
                                _requestQueue.Enqueue(newReqStruct);
                            }
                        }
                    }
                    else if (resp.GetType() == typeof(UserIdResponse))
                    {
                        resp = JsonUtility.FromJson<UserIdResponse>(request.downloadHandler.text);
                        AppStoreOnboardApi.userId = ((UserIdResponse) resp).sub;
                        UnityWebRequest newRequest = AppStoreOnboardApi.GetOrgId(Application.cloudProjectId);
                        OrgIdResponse orgIdResp = new OrgIdResponse();
                        ReqStruct newReqStruct = new ReqStruct();
                        newReqStruct.request = newRequest;
                        newReqStruct.resp = orgIdResp;
                        newReqStruct.targetStep = reqStruct.targetStep;
                        _requestQueue.Enqueue(newReqStruct);
                    }
                    else if (resp.GetType() == typeof(OrgIdResponse))
                    {
                        resp = JsonUtility.FromJson<OrgIdResponse>(request.downloadHandler.text);
                        AppStoreOnboardApi.orgId = ((OrgIdResponse) resp).org_foreign_key;

                        if (reqStruct.targetStep == STEP_GET_CLIENT)
                        {
                            UnityWebRequest newRequest =
                                AppStoreOnboardApi.GetUnityClientInfo(Application.cloudProjectId);
                            UnityClientResponseWrapper clientRespWrapper = new UnityClientResponseWrapper();
                            ReqStruct newReqStruct = new ReqStruct();
                            newReqStruct.request = newRequest;
                            newReqStruct.resp = clientRespWrapper;
                            newReqStruct.targetStep = reqStruct.targetStep;
                            _requestQueue.Enqueue(newReqStruct);
                        }
                        else if (reqStruct.targetStep == STEP_UPDATE_CLIENT)
                        {
                            UnityClientInfo unityClientInfo = new UnityClientInfo();
                            unityClientInfo.ClientId = _unityClientId.stringValue;
                            string callbackUrl = _callbackUrlInMemory;
                            UnityWebRequest newRequest =
                                AppStoreOnboardApi.UpdateUnityClient(Application.cloudProjectId, unityClientInfo,
                                    callbackUrl);
                            UnityClientResponse clientResp = new UnityClientResponse();
                            ReqStruct newReqStruct = new ReqStruct();
                            newReqStruct.request = newRequest;
                            newReqStruct.resp = clientResp;
                            newReqStruct.targetStep = reqStruct.targetStep;
                            _requestQueue.Enqueue(newReqStruct);
                        }
                        else if (reqStruct.targetStep == STEP_UPDATE_CLIENT_SECRET)
                        {
                            string clientId = _unityClientId.stringValue;
                            UnityWebRequest newRequest = AppStoreOnboardApi.UpdateUnityClientSecret(clientId);
                            UnityClientResponse clientResp = new UnityClientResponse();
                            ReqStruct newReqStruct = new ReqStruct();
                            newReqStruct.request = newRequest;
                            newReqStruct.resp = clientResp;
                            newReqStruct.targetStep = reqStruct.targetStep;
                            _requestQueue.Enqueue(newReqStruct);
                        }
                        else if (reqStruct.targetStep == "LinkProject")
                        {
                            UnityWebRequest newRequest =
                                AppStoreOnboardApi.GetUnityClientInfoByClientId(_clientIdToBeLinked);
                            UnityClientResponse unityClientResponse = new UnityClientResponse();
                            ReqStruct newReqStruct = new ReqStruct();
                            newReqStruct.request = newRequest;
                            newReqStruct.resp = unityClientResponse;
                            newReqStruct.targetStep = reqStruct.targetStep;
                            _requestQueue.Enqueue(newReqStruct);
                        }

                        serializedObject.ApplyModifiedProperties();
                        AssetDatabase.SaveAssets();
                    }
                    else if (resp.GetType() == typeof(UnityClientResponseWrapper))
                    {
                        string raw = "{ \"array\": " + request.downloadHandler.text + "}";
                        resp = JsonUtility.FromJson<UnityClientResponseWrapper>(raw);
                        // only one element in the list
                        if (((UnityClientResponseWrapper) resp).array.Length > 0)
                        {
                            if (reqStruct.targetStep != null && reqStruct.targetStep == "CheckUpdate")
                            {
                                _targetStep = STEP_GET_CLIENT;
                                CallApiAsync();
                            }
                            else
                            {
                                UnityClientResponse unityClientResp = ((UnityClientResponseWrapper) resp).array[0];
                                AppStoreOnboardApi.tps = unityClientResp.channel.thirdPartySettings;
                                _unityClientId.stringValue = unityClientResp.client_id;
                                _unityClientKey.stringValue = unityClientResp.client_secret;
                                _unityClientRsaPublicKey.stringValue = unityClientResp.channel.publicRSAKey;
                                _unityProjectId.stringValue = unityClientResp.channel.projectGuid;
                                _clientSecretInMemory = unityClientResp.channel.channelSecret;
                                _callbackUrlInMemory = unityClientResp.channel.callbackUrl;
                                _callbackUrlLast = _callbackUrlInMemory;
                                AppStoreOnboardApi.updateRev = unityClientResp.rev;
                                AppStoreOnboardApi.loaded = true;
                                serializedObject.ApplyModifiedProperties();
                                this.Repaint();
                                AssetDatabase.SaveAssets();
                                saveGameSettingsProps(unityClientResp.client_id);
                                UnityWebRequest newRequest = AppStoreOnboardApi.GetAppItem(_unityClientId.stringValue);
                                AppItemResponseWrapper appItemResponseWrapper = new AppItemResponseWrapper();
                                ReqStruct newReqStruct = new ReqStruct();
                                newReqStruct.request = newRequest;
                                newReqStruct.resp = appItemResponseWrapper;
                                _requestQueue.Enqueue(newReqStruct);
                            }
                        }
                        else
                        {
                            if (reqStruct.targetStep != null &&
                                (reqStruct.targetStep == "LinkProject" || reqStruct.targetStep == "CheckUpdate"))
                            {
                                _isOperationRunning = false;
                            }
                            // no client found, generate one or link to one
                            else
                            {
                                if (!_clientChecked)
                                {
                                    _currentState = State.LinkProject;
                                    _clientChecked = true;
                                    _isOperationRunning = false;
                                    Repaint();
                                }
                                else
                                {
                                    UnityClientInfo unityClientInfo = new UnityClientInfo();
                                    string callbackUrl = _callbackUrlInMemory;
                                    UnityWebRequest newRequest =
                                        AppStoreOnboardApi.GenerateUnityClient(Application.cloudProjectId,
                                            unityClientInfo,
                                            callbackUrl);
                                    UnityClientResponse clientResp = new UnityClientResponse();
                                    ReqStruct newReqStruct = new ReqStruct();
                                    newReqStruct.request = newRequest;
                                    newReqStruct.resp = clientResp;
                                    newReqStruct.targetStep = reqStruct.targetStep;
                                    _requestQueue.Enqueue(newReqStruct);
                                }
                            }
                        }
                    }
                    else if (resp.GetType() == typeof(UnityClientResponse))
                    {
                        resp = JsonUtility.FromJson<UnityClientResponse>(request.downloadHandler.text);
                        _unityClientId.stringValue = ((UnityClientResponse) resp).client_id;
                        _unityClientKey.stringValue = ((UnityClientResponse) resp).client_secret;
                        _unityClientRsaPublicKey.stringValue = ((UnityClientResponse) resp).channel.publicRSAKey;
                        _unityProjectId.stringValue = ((UnityClientResponse) resp).channel.projectGuid;
                        _clientSecretInMemory = ((UnityClientResponse) resp).channel.channelSecret;
                        _callbackUrlInMemory = ((UnityClientResponse) resp).channel.callbackUrl;
                        _callbackUrlLast = _callbackUrlInMemory;
                        AppStoreOnboardApi.tps = ((UnityClientResponse) resp).channel.thirdPartySettings;
                        AppStoreOnboardApi.updateRev = ((UnityClientResponse) resp).rev;
                        serializedObject.ApplyModifiedProperties();
                        this.Repaint();
                        AssetDatabase.SaveAssets();
                        saveGameSettingsProps(((UnityClientResponse) resp).client_id);

                        if (request.method == UnityWebRequest.kHttpVerbPOST) // Generated Client
                        {
                            UnityWebRequest analyticsRequest =
                                EditorAnalyticsApi.ClientEvent(EditorAnalyticsApi.k_ClientCreateEventName,
                                    ((UnityClientResponse) resp).client_id, null);

                            ReqStruct analyticsReqStruct = new ReqStruct
                            {
                                request = analyticsRequest,
                                resp = new EventRequestResponse(),
                                eventName = EditorAnalyticsApi.k_ClientCreateEventName,
                            };

                            _requestQueue.Enqueue(analyticsReqStruct);
                        }
                        else if (request.method == UnityWebRequest.kHttpVerbPUT) // Updated Client
                        {
                            UnityWebRequest analyticsRequest =
                                EditorAnalyticsApi.ClientEvent(EditorAnalyticsApi.k_ClientUpdateEventName,
                                    ((UnityClientResponse) resp).client_id, null);

                            ReqStruct analyticsReqStruct = new ReqStruct
                            {
                                request = analyticsRequest,
                                resp = new EventRequestResponse(),
                                eventName = EditorAnalyticsApi.k_ClientUpdateEventName,
                            };

                            _requestQueue.Enqueue(analyticsReqStruct);
                        }

                        if (reqStruct.targetStep == "LinkProject")
                        {
                            UnityClientInfo unityClientInfo = new UnityClientInfo();
                            unityClientInfo.ClientId = _unityClientId.stringValue;
                            UnityWebRequest newRequest =
                                AppStoreOnboardApi.UpdateUnityClient(Application.cloudProjectId, unityClientInfo,
                                    _callbackUrlInMemory);
                            UnityClientResponse clientResp = new UnityClientResponse();
                            ReqStruct newReqStruct = new ReqStruct();
                            newReqStruct.request = newRequest;
                            newReqStruct.resp = clientResp;
                            newReqStruct.targetStep = "GetRole";
                            _requestQueue.Enqueue(newReqStruct);
                        }
                        else if (reqStruct.targetStep == "GetRole")
                        {
                            UnityWebRequest newRequest = AppStoreOnboardApi.GetUserId();
                            UserIdResponse userIdResp = new UserIdResponse();
                            ReqStruct newReqStruct = new ReqStruct();
                            newReqStruct.request = newRequest;
                            newReqStruct.resp = userIdResp;
                            newReqStruct.targetStep = STEP_GET_CLIENT;
                            _requestQueue.Enqueue(newReqStruct);
                        }
                        else
                        {
                            if (reqStruct.targetStep == STEP_UPDATE_CLIENT)
                            {
                                EditorUtility.DisplayDialog("Hint",
                                    "Unity Client updated successfully.",
                                    "OK");
                            }

                            if (_currentAppItem.status == "STAGE")
                            {
                                UnityWebRequest newRequest = AppStoreOnboardApi.UpdateAppItem(_currentAppItem);
                                AppItemResponse appItemResponse = new AppItemResponse();
                                ReqStruct newReqStruct = new ReqStruct();
                                newReqStruct.request = newRequest;
                                newReqStruct.resp = appItemResponse;
                                _requestQueue.Enqueue(newReqStruct);
                            }
                            else
                            {
                                UnityWebRequest newRequest = AppStoreOnboardApi.GetAppItem(_unityClientId.stringValue);
                                AppItemResponseWrapper appItemResponseWrapper = new AppItemResponseWrapper();
                                ReqStruct newReqStruct = new ReqStruct();
                                newReqStruct.request = newRequest;
                                newReqStruct.resp = appItemResponseWrapper;
                                _requestQueue.Enqueue(newReqStruct);
                            }
                        }
                    }
                    else if (resp.GetType() == typeof(AppItemResponse))
                    {
                        resp = JsonUtility.FromJson<AppItemResponse>(request.downloadHandler.text);
                        _appItemId.stringValue = ((AppItemResponse) resp).id;
                        _appName.stringValue = ((AppItemResponse) resp).name;
                        _appSlug.stringValue = ((AppItemResponse) resp).slug;
                        _currentAppItem.id = ((AppItemResponse) resp).id;
                        _currentAppItem.name = ((AppItemResponse) resp).name;
                        _currentAppItem.slug = ((AppItemResponse) resp).slug;
                        _currentAppItem.ownerId = ((AppItemResponse) resp).ownerId;
                        _currentAppItem.ownerType = ((AppItemResponse) resp).ownerType;
                        _currentAppItem.status = ((AppItemResponse) resp).status;
                        _currentAppItem.type = ((AppItemResponse) resp).type;
                        _currentAppItem.clientId = ((AppItemResponse) resp).clientId;
                        _currentAppItem.packageName = ((AppItemResponse) resp).packageName;
                        _currentAppItem.revision = ((AppItemResponse) resp).revision;
                        serializedObject.ApplyModifiedProperties();
                        AssetDatabase.SaveAssets();

                        #region Analytics

                        string eventName = null;
                        if (request.method == UnityWebRequest.kHttpVerbPOST)
                        {
                            eventName = EditorAnalyticsApi.k_AppCreateEventName;
                        }
                        else if (request.method == UnityWebRequest.kHttpVerbPUT)
                        {
                            eventName = EditorAnalyticsApi.k_AppUpdateEventName;
                        }

                        if (eventName != null)
                        {
                            ReqStruct analyticsReqStruct = new ReqStruct
                            {
                                eventName = eventName,
                                request = EditorAnalyticsApi.AppEvent(eventName, _currentAppItem.clientId,
                                    (AppItemResponse) resp, null),
                                resp = new EventRequestResponse(),
                            };

                            _requestQueue.Enqueue(analyticsReqStruct);
                        }

                        #endregion

                        if (reqStruct.targetStep == STEP_UPDATE_GAME_TITLE)
                        {
                            _gameTitleChanged = false;
                            RemovePushRequest(_currentAppItem.id);
                        }

                        this.Repaint();
                        PublishApp(_appItemId.stringValue, reqStruct.targetStep);
                    }
                    else if (resp.GetType() == typeof(AppItemPublishResponse))
                    {
                        AppStoreOnboardApi.loaded = true;
                        resp = JsonUtility.FromJson<AppItemPublishResponse>(request.downloadHandler.text);
                        _currentAppItem.revision = ((AppItemPublishResponse) resp).revision;
                        _currentAppItem.status = "PUBLIC";
                        if (!(reqStruct.targetStep == STEP_UPDATE_GAME_TITLE))
                        {
                            ListPlayers();
                        }

                        this.Repaint();
                    }
                    else if (resp.GetType() == typeof(AppItemResponseWrapper))
                    {
                        resp = JsonUtility.FromJson<AppItemResponseWrapper>(request.downloadHandler.text);
                        if (((AppItemResponseWrapper) resp).total < 1)
                        {
                            // generate app
                            _currentAppItem.clientId = _unityClientId.stringValue;
                            _currentAppItem.name = _unityProjectId.stringValue;
                            _currentAppItem.slug = Guid.NewGuid().ToString();
                            _currentAppItem.ownerId = AppStoreOnboardApi.orgId;
                            UnityWebRequest newRequest = AppStoreOnboardApi.CreateAppItem(_currentAppItem);
                            AppItemResponse appItemResponse = new AppItemResponse();
                            ReqStruct newReqStruct = new ReqStruct();
                            newReqStruct.request = newRequest;
                            newReqStruct.resp = appItemResponse;
                            _requestQueue.Enqueue(newReqStruct);
                        }
                        else
                        {
                            var appItemResp = ((AppItemResponseWrapper) resp).results[0];
                            _appName.stringValue = appItemResp.name;
                            _appSlug.stringValue = appItemResp.slug;
                            _appItemId.stringValue = appItemResp.id;
                            _currentAppItem.id = appItemResp.id;
                            _currentAppItem.name = appItemResp.name;
                            _currentAppItem.slug = appItemResp.slug;
                            _currentAppItem.ownerId = appItemResp.ownerId;
                            _currentAppItem.ownerType = appItemResp.ownerType;
                            _currentAppItem.status = appItemResp.status;
                            _currentAppItem.type = appItemResp.type;
                            _currentAppItem.clientId = appItemResp.clientId;
                            _currentAppItem.packageName = appItemResp.packageName;
                            _currentAppItem.revision = appItemResp.revision;
                            serializedObject.ApplyModifiedProperties();
                            AssetDatabase.SaveAssets();
                            this.Repaint();

                            if (appItemResp.status != "PUBLIC")
                            {
                                PublishApp(appItemResp.id, "");
                            }
                            else
                            {
                                AppStoreOnboardApi.loaded = true;
                                ListPlayers();
                            }
                        }
                    }
                    else if (resp.GetType() == typeof(PlayerResponse))
                    {
                        resp = JsonUtility.FromJson<PlayerResponse>(request.downloadHandler.text);

                        var playerId = ((PlayerResponse) resp).id;

                        _testAccounts[reqStruct.arrayPos].playerId = playerId;
                        _testAccounts[reqStruct.arrayPos].password = "******";
                        RemovePushRequest(_testAccounts[reqStruct.arrayPos].email);
                        _testAccountsDirty[reqStruct.arrayPos] = false;
                        this.Repaint();

                        UnityWebRequest newRequest = AppStoreOnboardApi.VerifyTestAccount(playerId);
                        PlayerVerifiedResponse playerVerifiedResponse = new PlayerVerifiedResponse();
                        ReqStruct newReqStruct = new ReqStruct();
                        newReqStruct.request = newRequest;
                        newReqStruct.resp = playerVerifiedResponse;
                        newReqStruct.targetStep = null;
                        _requestQueue.Enqueue(newReqStruct);
                    }
                    else if (resp.GetType() == typeof(PlayerResponseWrapper))
                    {
                        resp = JsonUtility.FromJson<PlayerResponseWrapper>(request.downloadHandler.text);
                        _testAccounts = new List<TestAccount>();
                        AppStoreStyles.kTestAccountBoxHeight = 25;
                        if (((PlayerResponseWrapper) resp).total > 0)
                        {
                            var exists = ((PlayerResponseWrapper) resp).results;
                            for (int i = 0; i < exists.Length; i++)
                            {
                                TestAccount existed = new TestAccount
                                {
                                    email = exists[i].nickName,
                                    password = "******",
                                    playerId = exists[i].id
                                };
                                _testAccounts.Add(existed);
                                _testAccountsDirty.Add(false);
                                _testAccountsValidationMsg.Add("");
                                AppStoreStyles.kTestAccountBoxHeight += 22;
                            }

                            this.Repaint();
                        }

                        PullIAPItems();
                    }
                    else if (resp.GetType() == typeof(PlayerVerifiedResponse))
                    {
                        // ListPlayers();
                    }
                    else if (resp.GetType() == typeof(PlayerChangePasswordResponse))
                    {
                        RemovePushRequest(_testAccounts[reqStruct.arrayPos].email);
                        _testAccounts[reqStruct.arrayPos].password = "******";
                        _testAccountsDirty[reqStruct.arrayPos] = false;
                        this.Repaint();
                        // ListPlayers();
                    }
                    else if (resp.GetType() == typeof(PlayerDeleteResponse))
                    {
                        _isTestAccountUpdating = false;
                        EditorUtility.DisplayDialog("Success",
                            "TestAccount " + reqStruct.currTestAccount.playerId + " has been Deleted.", "OK");
                        RemoveTestAccountLocally(reqStruct.arrayPos);
                        this.Repaint();
                    }
                    else if (resp.GetType() == typeof(TokenInfo))
                    {
                        resp = JsonUtility.FromJson<TokenInfo>(request.downloadHandler.text);
                        AppStoreOnboardApi.tokenInfo.access_token = ((TokenInfo) resp).access_token;
                        AppStoreOnboardApi.tokenInfo.refresh_token = ((TokenInfo) resp).refresh_token;
                        UnityWebRequest newRequest = AppStoreOnboardApi.GetUserId();
                        UserIdResponse userIdResp = new UserIdResponse();
                        ReqStruct newReqStruct = new ReqStruct();
                        newReqStruct.request = newRequest;
                        newReqStruct.resp = userIdResp;
                        newReqStruct.targetStep = reqStruct.targetStep;
                        _requestQueue.Enqueue(newReqStruct);
                    }
                    else if (resp.GetType() == typeof(UserIdResponse))
                    {
                        resp = JsonUtility.FromJson<UserIdResponse>(request.downloadHandler.text);
                        AppStoreOnboardApi.userId = ((UserIdResponse) resp).sub;
                        UnityWebRequest newRequest = AppStoreOnboardApi.GetOrgId(Application.cloudProjectId);
                        OrgIdResponse orgIdResp = new OrgIdResponse();
                        ReqStruct newReqStruct = new ReqStruct();
                        newReqStruct.request = newRequest;
                        newReqStruct.resp = orgIdResp;
                        newReqStruct.targetStep = reqStruct.targetStep;
                        _requestQueue.Enqueue(newReqStruct);
                    }
                    else if (resp.GetType() == typeof(OrgIdResponse))
                    {
                        resp = JsonUtility.FromJson<OrgIdResponse>(request.downloadHandler.text);
                        AppStoreOnboardApi.orgId = ((OrgIdResponse) resp).org_foreign_key;
                        UnityWebRequest newRequest = AppStoreOnboardApi.GetOrgRoles();
                        OrgRoleResponse orgRoleResp = new OrgRoleResponse();
                        ReqStruct newReqStruct = new ReqStruct();
                        newReqStruct.request = newRequest;
                        newReqStruct.resp = orgRoleResp;
                        newReqStruct.targetStep = reqStruct.targetStep;
                        _requestQueue.Enqueue(newReqStruct);
                    }
                    else if (resp.GetType() == typeof(IapItemSearchResponse))
                    {
                        IapItemSearchResponse response =
                            JsonUtility.FromJson<IapItemSearchResponse>(
                                HandleIapItemResponse(request.downloadHandler.text));

                        ClearIapItems();
                        for (int i = 0; i < response.total; i++)
                        {
                            AddIapItem(response.results[i], false);
                        }

                        _isOperationRunning = false;
                        _currentState = State.Success;
                        this.Repaint();
                    }
                    else if (resp.GetType() == typeof(IapItemDeleteResponse))
                    {
                        _isIapUpdating = false;
                        EditorUtility.DisplayDialog("Success",
                            "Product " + reqStruct.curIapItem.slug + " has been Deleted.", "OK");
                        RemoveIapItemLocally(reqStruct.arrayPos);
                        this.Repaint();
                    }
                    else if (resp.GetType() == typeof(UnityIapItemUpdateResponse))
                    {
                        ProcessIAPResponse(reqStruct, request, EditorAnalyticsApi.k_IapUpdateEventName);
                    }
                    else if (resp.GetType() == typeof(UnityIapItemCreateResponse))
                    {
                        ProcessIAPResponse(reqStruct, request, EditorAnalyticsApi.k_IapCreateEventName);
                    }
                }
            }
            else
            {
                _requestQueue.Enqueue(reqStruct);
            }
        }

        void InitializeSecrets()
        {
            // If client secret is in the memory, We do nothing
            if (!string.IsNullOrEmpty(_clientSecretInMemory))
            {
                return;
            }

            // If client ID is not serialized, it means this is a newly created GameSettings.asset
            // We provide a chance to link the project to an existing client
            if (string.IsNullOrEmpty(_unityClientId.stringValue))
            {
                _isOperationRunning = true;
                _targetStep = STEP_GET_CLIENT;
                CallApiAsync();
                return;
            }

            // Start initialization.
            _isOperationRunning = true;
            UnityWebRequest newRequest = AppStoreOnboardApi.GetUnityClientInfo(Application.cloudProjectId);
            UnityClientResponseWrapper clientRespWrapper = new UnityClientResponseWrapper();
            ReqStruct newReqStruct = new ReqStruct();
            newReqStruct.request = newRequest;
            newReqStruct.resp = clientRespWrapper;
            newReqStruct.targetStep =
                "CheckUpdate";
            _requestQueue.Enqueue(newReqStruct);
        }

        private string _cloudProjectId;
        private string _showingMsg = "";
        private bool _inAppPurchaseFoldout = true;
        private bool _udpClientSettingsFoldout = false;
        private bool _testAccountFoldout = false;
        private List<bool> _iapItemsFoldout = new List<bool>(); // TODO: Update this by API
        private List<IapItem> _iapItems = new List<IapItem>();
        private List<bool> _iapItemDirty = new List<bool>();
        private List<string> _iapValidationMsg = new List<string>();
        private readonly string[] _iapItemTypeOptions = {"Consumable", "Non consumable"};
        private string _clientIdToBeLinked = "UDP client ID";
        private bool _callbackUrlChanged = false;
        private string _updateClientErrorMsg = "";
        private bool _gameTitleChanged = false;
        private string _updateGameTitleErrorMsg = "";

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            GUIStyle labelStyle = new GUIStyle(GUI.skin.label) {wordWrap = true};
            EditorGUI.BeginDisabledGroup(_isOperationRunning || pushRequestList.Count > 0);

            switch (_currentState)
            {
                case State.CannotUseOAuth:
                    _showingMsg =
                        "UDP editor extension can only work on Unity 5.6.1+. Please check your Unity version and retry.";
                    EditorGUILayout.LabelField(_showingMsg, new GUIStyle(GUI.skin.label) {wordWrap = true});
                    break;
                case State.CannotGetCloudProjectId:
                    _showingMsg =
                        "To use the Unity distribution portal your project will need a Unity project ID. You can create a new project ID or link to an existing one in the Services window.";
                    EditorGUILayout.LabelField(_showingMsg, new GUIStyle(GUI.skin.label) {wordWrap = true});

                    if (GUILayout.Button("Go to the Services Window"))
                    {
#if UNITY_2018_2_OR_NEWER
                        EditorApplication.ExecuteMenuItem("Window/General/Services");
#else
                        EditorApplication.ExecuteMenuItem("Window/Services");
#endif
                        Selection.activeObject = null;
                    }

                    break;
                case State.LinkProject:
                    EditorGUILayout.LabelField("Your project must be linked to a UDP client.", labelStyle);
                    EditorGUILayout.LabelField(
                        "If you're starting your UDP project here, generate a new UDP client now.", labelStyle);
                    EditorGUILayout.LabelField(
                        "If your game client was created from the UDP portal, link it to your project using the client ID.",
                        labelStyle);

                    float labelWidth = Math.Max(EditorGUIUtility.currentViewWidth / 2, 180);
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("Generate new UDP client", GUILayout.Width(labelWidth)))
                    {
                        _isOperationRunning = true;
                        _targetStep = STEP_GET_CLIENT;
                        CallApiAsync();
                    }

                    GUILayout.FlexibleSpace();
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.LabelField("or",
                        new GUIStyle(GUI.skin.label) {alignment = TextAnchor.MiddleCenter});

                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    _clientIdToBeLinked = EditorGUILayout.TextField(_clientIdToBeLinked, GUILayout.Width(labelWidth));
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("Link to existing UDP client", GUILayout.Width(labelWidth)))
                    {
                        if (!string.IsNullOrEmpty(_clientIdToBeLinked))
                        {
                            _isOperationRunning = true;
                            UnityWebRequest newRequest =
                                AppStoreOnboardApi.GetUnityClientInfoByClientId(_clientIdToBeLinked);
                            UnityClientResponse unityClientResponse = new UnityClientResponse();
                            ReqStruct newReqStruct = new ReqStruct();
                            newReqStruct.request = newRequest;
                            newReqStruct.resp = unityClientResponse;
                            newReqStruct.targetStep = "LinkProject";
                            _requestQueue.Enqueue(newReqStruct);
                        }
                    }

                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();


                    break;
                case State.Success: // Main Interface
                    // Main Display
                    EditorGUILayout.LabelField("UDP Settings.asset DOES NOT store your changes locally.", labelStyle);
                    EditorGUILayout.LabelField("'Push' will save your changes to the UDP server.", labelStyle);
                    EditorGUILayout.LabelField("'Pull' will retrieve your settings from the UDP server.", labelStyle);
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();

                    if (GUILayout.Button("Pull", GUILayout.Width(AppStoreStyles.kAppStoreSettingsButtonWidth)))
                    {
                        GUI.FocusControl(null);
                        if (AnythingChanged())
                        {
                            if (EditorUtility.DisplayDialog("Local changes may be overwritten",
                                "There are pending local edits that will be lost if you pull.",
                                "Pull anyway", "Cancel"))
                            {
                                RefreshAllInformation();
                            }
                        }
                        else
                        {
                            RefreshAllInformation();
                        }
                    }

                    if (GUILayout.Button("Push", GUILayout.Width(AppStoreStyles.kAppStoreSettingsButtonWidth)))
                    {
                        // Slug check locally
                        var slugs = new HashSet<String>();

                        // Update IAP Items
                        for (int i = 0; i < _iapItemDirty.Count; i++)
                        {
                            int pos = i;
                            if (_iapItemDirty[pos])
                            {
                                //Check validation
                                _iapValidationMsg[pos] = _iapItems[pos].Validate();
                                if (_iapValidationMsg[pos] == "")
                                {
                                    _iapValidationMsg[pos] = _iapItems[pos].SlugValidate(slugs);
                                }

                                if (_iapValidationMsg[pos] == "")
                                {
                                    // If check succeeds
                                    if (!string.IsNullOrEmpty(_iapItems[pos].id))
                                    {
                                        UpdateIAPItem(_iapItems[pos], pos);
                                    }
                                    else
                                    {
                                        CreateIAPItem(_iapItems[pos], pos);
                                    }

                                    AddPushRequests(_iapItems[pos].slug);
                                }
                                else
                                {
                                    Debug.LogError(
                                        "[UDP] Iap:" + _iapItems[pos].slug + " " + _iapValidationMsg[pos]);
                                }
                            }
                        }

                        // Update UDP Client Settings
                        if (_callbackUrlChanged)
                        {
                            if (CheckURL(_callbackUrlLast))
                            {
                                _updateClientErrorMsg = "";
                                UpdateCallbackUrl();
                                AddPushRequests(_unityClientId.stringValue);
                            }
                            else
                            {
                                _updateClientErrorMsg = "Callback URL is invalid. (http/https is required)";
                            }
                        }

                        // Update Game Settings

                        if (_gameTitleChanged)
                        {
                            if (!string.IsNullOrEmpty(_currentAppItem.name))
                            {
                                _updateGameTitleErrorMsg = "";
                                UpdateGameTitle();
                                AddPushRequests(_currentAppItem.id);
                            }
                            else
                            {
                                _updateGameTitleErrorMsg = "Game title cannot be null";
                            }
                        }

                        // Update Test Accounts
                        for (int i = 0; i < _testAccounts.Count; i++)
                        {
                            int pos = i;
                            if (_testAccountsDirty[pos])
                            {
                                _testAccountsValidationMsg[pos] = _testAccounts[pos].Validate();
                                if (_testAccountsValidationMsg[pos] == "")
                                {
                                    if (string.IsNullOrEmpty(_testAccounts[pos].playerId))
                                    {
                                        CreateTestAccount(_testAccounts[pos], pos);
                                    }
                                    else
                                    {
                                        UpdateTestAccount(_testAccounts[pos], pos);
                                    }

                                    AddPushRequests(_testAccounts[pos].email);
                                }
                                else
                                {
                                    Debug.LogError(
                                        "[UDP] TestAccount:" + _testAccounts[pos].email + " " + _testAccountsValidationMsg[pos]);
                                }
                            }
                        }

                        this.Repaint();
                    }

                    EditorGUILayout.EndHorizontal();

                    GuiLine();

                    #region Title & ProjectID

                {
                    EditorGUILayout.LabelField("Game Title");
                    if (_updateGameTitleErrorMsg != "")
                    {
                        GUIStyle textStyle = new GUIStyle(GUI.skin.label);
                        textStyle.wordWrap = true;
                        textStyle.normal.textColor = Color.red;
                        EditorGUILayout.LabelField(_updateGameTitleErrorMsg, textStyle);
                    }

                    EditorGUI.BeginChangeCheck();
                    _currentAppItem.name = EditorGUILayout.TextField(_currentAppItem.name);

                    if (GUI.changed)
                    {
                        _gameTitleChanged = true;
                    }

                    EditorGUI.EndChangeCheck();
                }

                {
                    EditorGUILayout.LabelField("Unity Project ID");
                    EditorGUILayout.BeginHorizontal();
                    SelectableLabel(_cloudProjectId);
                    if (GUILayout.Button("Copy", GUILayout.Width(AppStoreStyles.kCopyButtonWidth)))
                    {
                        TextEditor te = new TextEditor();
                        te.text = _cloudProjectId;
                        te.SelectAll();
                        te.Copy();
                    }

                    EditorGUILayout.EndHorizontal();
                    GuiLine();
                }

                    #endregion

                    #region In App Purchase Configuration

#pragma warning disable CS0162
                    if (BuildConfig.IAP_VERSION)
                    {
                        EditorGUILayout.BeginVertical();
                        _inAppPurchaseFoldout = EditorGUILayout.Foldout(_inAppPurchaseFoldout, "IAP Catalog", true,
                            AppStoreStyles.KAppStoreSettingsHeaderGuiStyle);
                        if (_inAppPurchaseFoldout)
                        {
                            // Add New IAP Button (Center)
                            {
                                EditorGUILayout.BeginHorizontal();
                                GUILayout.FlexibleSpace();
                                if (GUILayout.Button("Open Catalog", new GUIStyle(GUI.skin.button)
                                    {
                                        fontSize = AppStoreStyles.kAddNewIapButtonFontSize
                                    },
                                    GUILayout.Height(EditorGUIUtility.singleLineHeight *
                                                     AppStoreStyles.kAddNewIapButtonRatio),
                                    GUILayout.Width(EditorGUIUtility.currentViewWidth / 2)))
                                {
                                    EditorApplication.ExecuteMenuItem("Window/Unity IAP/IAP Catalog");
                                }

                                GUILayout.FlexibleSpace();
                                EditorGUILayout.EndHorizontal();
                            }
                        }

                        EditorGUILayout.EndVertical();
                        GuiLine();
                    }
                    else
                    {
                        EditorGUI.BeginDisabledGroup(_isIapUpdating);
                        var currentRect = EditorGUILayout.BeginVertical();
                        _inAppPurchaseFoldout = EditorGUILayout.Foldout(_inAppPurchaseFoldout, "IAP Catalog", true,
                            AppStoreStyles.KAppStoreSettingsHeaderGuiStyle);
                        if (_inAppPurchaseFoldout)
                        {
                            EditorGUI.indentLevel++;
                            EditorGUI.LabelField(new Rect(currentRect.xMax - 120, currentRect.yMin, 120, 20),
                                string.Format("{0} total ({1} edited)", _iapItems.Count, EditedIAPCount()),
                                new GUIStyle(GUI.skin.label) {alignment = TextAnchor.MiddleRight});
                            for (int i = 0; i < _iapItemsFoldout.Count; i++)
                            {
                                currentRect = EditorGUILayout.BeginVertical();
                                int pos = i;

                                if (_iapItemDirty[pos])
                                {
                                    EditorGUI.LabelField(new Rect(currentRect.xMax - 95, currentRect.yMin, 80, 20),
                                        "(edited)", new GUIStyle(GUI.skin.label) {alignment = TextAnchor.UpperRight});
                                }

                                if (GUI.Button(new Rect(currentRect.xMax - 15, currentRect.yMin, 15, 15), "\u2261"))
                                {
                                    GenericMenu menu = new GenericMenu();
                                    menu.AddItem(new GUIContent("Push"), false, () =>
                                    {
                                        if (_iapItemDirty[pos])
                                        {
                                            _iapValidationMsg[pos] = _iapItems[pos].Validate();
                                            if (!string.IsNullOrEmpty(_iapValidationMsg[pos]))
                                            {
                                                Debug.LogError(
                                                    "[UDP] Iap:" + _iapItems[pos].slug + " " + _iapValidationMsg[pos]);
                                            }

                                            if (string.IsNullOrEmpty(_iapValidationMsg[pos]))
                                            {
                                                // If check suceeds
                                                if (!string.IsNullOrEmpty(_iapItems[pos].id))
                                                {
                                                    UpdateIAPItem(_iapItems[pos], pos);
                                                }
                                                else
                                                {
                                                    CreateIAPItem(_iapItems[pos], pos);
                                                }

                                                AddPushRequests(_iapItems[pos].slug);
                                            }
                                            else
                                            {
                                                Repaint();
                                            }
                                        }
                                    });
                                    menu.AddItem(new GUIContent("Delete"), false, () =>
                                    {
                                        if (string.IsNullOrEmpty(_iapItems[pos].id))
                                        {
                                            RemoveIapItemLocally(pos);
                                        }
                                        else
                                        {
                                            DeleteIAPItem(_iapItems[pos], pos);
                                        }
                                    });
                                    menu.ShowAsContext();
                                }

                                IapItem item = _iapItems[pos];
                                _iapItemsFoldout[pos] = EditorGUILayout.Foldout(_iapItemsFoldout[pos],
                                    "Product: " + (item.name), true,
                                    new GUIStyle(EditorStyles.foldout)
                                    {
                                        wordWrap = false, clipping = TextClipping.Clip,
                                        fixedWidth = currentRect.xMax - 95
                                    });
                                if (_iapItemsFoldout[pos])
                                {
                                    if (_iapValidationMsg[pos] != "")
                                    {
                                        GUIStyle textStyle = new GUIStyle(GUI.skin.label);
                                        textStyle.wordWrap = true;
                                        textStyle.normal.textColor = Color.red;

                                        EditorGUILayout.LabelField(_iapValidationMsg[pos], textStyle);
                                    }

                                    EditorGUI.BeginChangeCheck();
                                    item.slug = LabelWithTextField("Product ID", item.slug);
                                    item.name = LabelWithTextField("Name", item.name);

                                    GUILayout.BeginHorizontal();
                                    EditorGUILayout.LabelField("Type",
                                        GUILayout.Width(AppStoreStyles.kClientLabelWidth));
                                    int index = item.consumable ? 0 : 1;
                                    index = EditorGUILayout.Popup(index, _iapItemTypeOptions);
                                    item.consumable = index == 0;
                                    GUILayout.EndHorizontal();

                                    PriceDetail pd = ExtractUSDPrice(item);
                                    pd.price = LabelWithTextField("Price (USD)", pd.price);

                                    EditorGUILayout.BeginVertical();
                                    EditorGUILayout.LabelField("Description");

                                    item.properties.description = EditorGUILayout.TextArea(item.properties.description,
                                        GUILayout.Height(EditorGUIUtility.singleLineHeight * 4));
                                    EditorGUILayout.EndVertical();

                                    if (GUI.changed)
                                    {
                                        _iapItemDirty[pos] = true;
                                    }

                                    EditorGUI.EndChangeCheck();
                                }

                                EditorGUILayout.EndVertical();
                            }

                            EditorGUI.indentLevel--;
                            // Add New IAP Button (Center)
                            {
                                EditorGUILayout.BeginHorizontal();
                                GUILayout.FlexibleSpace();
                                if (GUILayout.Button("Add new IAP", new GUIStyle(GUI.skin.button)
                                    {
                                        fontSize = AppStoreStyles.kAddNewIapButtonFontSize
                                    },
                                    GUILayout.Height(EditorGUIUtility.singleLineHeight *
                                                     AppStoreStyles.kAddNewIapButtonRatio),
                                    GUILayout.Width(EditorGUIUtility.currentViewWidth / 2)))
                                {
                                    AddIapItem(new IapItem
                                    {
                                        masterItemSlug = _currentAppItem.slug,
                                    }, true, true);
                                }

                                GUILayout.FlexibleSpace();
                                EditorGUILayout.EndHorizontal();
                            }
                        }

                        EditorGUILayout.EndVertical();
                        EditorGUI.EndDisabledGroup();
                        GuiLine();
                    }

                    #endregion

                    #region UDP Client Settings

                    _udpClientSettingsFoldout = EditorGUILayout.Foldout(_udpClientSettingsFoldout, "Settings", true,
                        AppStoreStyles.KAppStoreSettingsHeaderGuiStyle);
                    if (_udpClientSettingsFoldout)
                    {
                        EditorGUI.indentLevel++;

                        if (_updateClientErrorMsg != "")
                        {
                            GUIStyle textStyle = new GUIStyle(GUI.skin.label);
                            textStyle.wordWrap = true;
                            textStyle.normal.textColor = Color.red;

                            EditorGUILayout.LabelField(_updateClientErrorMsg, textStyle);
                        }

                        LabelWithReadonlyTextField("Game ID", _currentAppItem.id);
                        LabelWithReadonlyTextField("Client ID", _unityClientId.stringValue);
                        LabelWithReadonlyTextField("Client Key", _unityClientKey.stringValue);
                        LabelWithReadonlyTextField("RSA Public Key", _unityClientRsaPublicKey.stringValue);
                        LabelWithReadonlyTextField("Client Secret", _clientSecretInMemory);

                        EditorGUI.BeginChangeCheck();
                        _callbackUrlLast = LabelWithTextField("Callback URL", _callbackUrlLast);

                        if (GUI.changed)
                        {
                            _callbackUrlChanged = true;
                        }

                        EditorGUI.EndChangeCheck();

                        EditorGUI.indentLevel--;
                    }

                    GuiLine();

                    #endregion

                    #region Test Accounts

                    EditorGUI.BeginDisabledGroup(_isTestAccountUpdating);

                    _testAccountFoldout = EditorGUILayout.Foldout(_testAccountFoldout, "UDP Sandbox Test Accounts",
                        true,
                        AppStoreStyles.KAppStoreSettingsHeaderGuiStyle);

                    if (_testAccountFoldout)
                    {
                        for (int i = 0; i < _testAccounts.Count; i++)
                        {
                            int pos = i;

                            if (_testAccountsValidationMsg[pos] != "")
                            {
                                GUIStyle textStyle = new GUIStyle(GUI.skin.label);
                                textStyle.wordWrap = true;
                                textStyle.normal.textColor = Color.red;

                                EditorGUILayout.LabelField(_testAccountsValidationMsg[pos], textStyle);
                            }

                            EditorGUILayout.BeginHorizontal();
                            EditorGUI.BeginChangeCheck();
                            _testAccounts[pos].email = EditorGUILayout.TextField(_testAccounts[pos].email);
                            _testAccounts[pos].password = EditorGUILayout.PasswordField(_testAccounts[pos].password);

                            if (GUI.changed)
                            {
                                _testAccountsDirty[pos] = true;
                            }

                            EditorGUI.EndChangeCheck();

                            //delete action
                            if (GUILayout.Button("\u2212", new GUIStyle(GUI.skin.button)
                                {
                                    fontSize = AppStoreStyles.kAddNewIapButtonFontSize,
                                    margin = new RectOffset(0, 0, 2, 0)
                                },
                                GUILayout.Height(15),
                                GUILayout.Width(15)))
                            {
                                if ((string.IsNullOrEmpty(_testAccounts[pos].playerId)))
                                {
                                    RemoveTestAccountLocally(pos);
                                }
                                else
                                {
                                    DeleteTestAccount(_testAccounts[pos], pos);
                                }
                            }

                            EditorGUILayout.EndHorizontal();
                        }


                        {
                            EditorGUILayout.BeginHorizontal();
                            GUILayout.FlexibleSpace();
                            if (GUILayout.Button("Add new test account", new GUIStyle(GUI.skin.button)
                                {
                                    fontSize = AppStoreStyles.kAddNewIapButtonFontSize
                                },
                                GUILayout.Height(EditorGUIUtility.singleLineHeight *
                                                 AppStoreStyles.kAddNewIapButtonRatio),
                                GUILayout.Width(EditorGUIUtility.currentViewWidth / 2)))
                            {
                                _testAccounts.Add(new TestAccount
                                {
                                    email = "Email",
                                    password = "Password",
                                    isUpdate = false,
                                });
                                _testAccountsDirty.Add(true);
                                _testAccountsValidationMsg.Add("");
                            }

                            GUILayout.FlexibleSpace();
                            EditorGUILayout.EndHorizontal();
                        }
                    }

                    GuiLine();
                    EditorGUI.EndDisabledGroup();

                    #endregion

                    #region Go to Portal

                {
                    EditorGUILayout.BeginHorizontal();

                    if (GUILayout.Button("Go to UDP console", GUILayout.Width(AppStoreStyles.kGoToPortalButtonWidth)))
                    {
                        Application.OpenURL(BuildConfig.CONSOLE_URL);
                    }

                    EditorGUILayout.EndHorizontal();
                }

                    #endregion

                    break;
            }

            EditorGUI.EndDisabledGroup();
        }

        private void OnDestroy()
        {
            EditorApplication.update -= CheckRequestUpdate;
        }

        #region helper method

        void CallApiAsync()
        {
            if (AppStoreOnboardApi.tokenInfo.access_token == null)
            {
                Type unityOAuthType = Utils.FindTypeByName("UnityEditor.Connect.UnityOAuth");
                Type authCodeResponseType = unityOAuthType.GetNestedType("AuthCodeResponse", BindingFlags.Public);
                var performMethodInfo =
                    typeof(AppStoreSettingsEditor).GetMethod("Perform").MakeGenericMethod(authCodeResponseType);
                var actionT =
                    typeof(Action<>).MakeGenericType(authCodeResponseType); // Action<UnityOAuth.AuthCodeResponse>
                var getAuthorizationCodeAsyncMethodInfo = unityOAuthType.GetMethod("GetAuthorizationCodeAsync");
                var performDelegate = Delegate.CreateDelegate(actionT, this, performMethodInfo);
                try
                {
                    getAuthorizationCodeAsyncMethodInfo.Invoke(null,
                        new object[] {AppStoreOnboardApi.oauthClientId, performDelegate});
                }
                catch (TargetInvocationException ex)
                {
                    if (ex.InnerException is InvalidOperationException)
                    {
                        Debug.LogError("[UDP] You must login with Unity ID first.");
                        EditorUtility.DisplayDialog("Error", "You must login with Unity ID first.", "OK");
                        _currentState = State.CannotGetCloudProjectId;
                        _isOperationRunning = false;
                        this.Repaint();
                    }
                }
            }
            else
            {
                UnityWebRequest request = AppStoreOnboardApi.GetUserId();
                UserIdResponse userIdResp = new UserIdResponse();
                ReqStruct reqStruct = new ReqStruct();
                reqStruct.request = request;
                reqStruct.resp = userIdResp;
                reqStruct.targetStep = _targetStep;
                _requestQueue.Enqueue(reqStruct);
            }
        }

        public void Perform<T>(T response)
        {
            var authCodePropertyInfo = response.GetType().GetProperty("AuthCode");
            var exceptionPropertyInfo = response.GetType().GetProperty("Exception");
            string authCode = (string) authCodePropertyInfo.GetValue(response, null);
            Exception exception = (Exception) exceptionPropertyInfo.GetValue(response, null);

            if (authCode != null)
            {
                UnityWebRequest request = AppStoreOnboardApi.GetAccessToken(authCode);
                TokenInfo tokenInfoResp = new TokenInfo();
                ReqStruct reqStruct = new ReqStruct();
                reqStruct.request = request;
                reqStruct.resp = tokenInfoResp;
                reqStruct.targetStep = _targetStep;
                _requestQueue.Enqueue(reqStruct);
            }
            else
            {
                Debug.LogError("[UDP] " + "Failed: " + exception.ToString());
                EditorUtility.DisplayDialog("Error", "Failed: " + exception.ToString(), "OK");
                _currentState = State.CannotGetCloudProjectId;
                _isOperationRunning = false;
                Repaint();
            }
        }

        private void saveGameSettingsProps(String clientId)
        {
            if (!Directory.Exists(AppStoreSettings.appStoreSettingsPropFolder))
                Directory.CreateDirectory(AppStoreSettings.appStoreSettingsPropFolder);
            StreamWriter writter = new StreamWriter(AppStoreSettings.appStoreSettingsPropPath, false);
            String warningMessage = "*** DO NOT DELETE OR MODIFY THIS FILE !! ***";
            writter.WriteLine(warningMessage);
            writter.WriteLine(clientId);
            writter.WriteLine(warningMessage);
            writter.Close();
        }

        void GuiLine(int i_height = 1)
        {
            Rect rect = EditorGUILayout.GetControlRect(false, i_height);
            rect.height = i_height;
            EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));
        }

        string LabelWithTextField(string labelText, string defaultText = "",
            float labelWidth = AppStoreStyles.kClientLabelWidth)
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(labelText, GUILayout.Width(labelWidth));
            string text = EditorGUILayout.TextField(defaultText);
            GUILayout.EndHorizontal();
            return text;
        }

        void SelectableLabel(string labelText)
        {
            EditorGUILayout.SelectableLabel(labelText, EditorStyles.textField,
                GUILayout.Height(EditorGUIUtility.singleLineHeight));
        }

        void LabelWithReadonlyTextField(string labelText, string defaultText = "",
            float labelWidth = AppStoreStyles.kClientLabelWidth)
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(labelText, GUILayout.Width(labelWidth));
            EditorGUILayout.SelectableLabel(defaultText, EditorStyles.textField,
                GUILayout.Height(EditorGUIUtility.singleLineHeight));
            GUILayout.EndHorizontal();
        }

        private void PublishApp(String appItemId, string targetStep)
        {
            UnityWebRequest newRequest = AppStoreOnboardApi.PublishAppItem(appItemId);
            AppItemPublishResponse appItemPublishResponse = new AppItemPublishResponse();
            ReqStruct newReqStruct = new ReqStruct();
            newReqStruct.request = newRequest;
            newReqStruct.resp = appItemPublishResponse;
            newReqStruct.targetStep = targetStep;
            _requestQueue.Enqueue(newReqStruct);
        }

        private void ListPlayers()
        {
            UnityWebRequest newRequest = AppStoreOnboardApi.GetTestAccount(_unityClientId.stringValue);
            PlayerResponseWrapper playerResponseWrapper = new PlayerResponseWrapper();
            ReqStruct newReqStruct = new ReqStruct();
            newReqStruct.request = newRequest;
            newReqStruct.resp = playerResponseWrapper;
            newReqStruct.targetStep = null;
            _requestQueue.Enqueue(newReqStruct);
        }

        private void PullIAPItems()
        {
            UnityWebRequest request = AppStoreOnboardApi.SearchStoreItem(_currentAppItem.slug);
            IapItemSearchResponse clientResp = new IapItemSearchResponse();
            ReqStruct reqStruct = new ReqStruct();
            reqStruct.request = request;
            reqStruct.resp = clientResp;
            reqStruct.curIapItem = null;
            _requestQueue.Enqueue(reqStruct);
        }

        private void ProcessIAPResponse(ReqStruct reqStruct, UnityWebRequest request, string eventName)
        {
            reqStruct.curIapItem = JsonUtility.FromJson<IapItem>(HandleIapItemResponse(request.downloadHandler.text));
            RemovePushRequest(reqStruct.curIapItem.slug);
            _iapItemDirty[reqStruct.arrayPos] = false;
            ReqStruct analyticsReq = new ReqStruct
            {
                eventName = eventName,
                request = EditorAnalyticsApi.IapEvent(eventName, _unityClientId.stringValue,
                    reqStruct.curIapItem, null),
                resp = new EventRequestResponse(),
            };

            _iapItems[reqStruct.arrayPos] = reqStruct.curIapItem; // add id information

            _requestQueue.Enqueue(analyticsReq);
            Repaint();
        }

        private string HandleIapItemResponse(string oldData)
        {
            string newData = oldData.Replace("en-US", "thisShouldBeENHyphenUS");
            newData = newData.Replace("zh-CN", "thisShouldBeZHHyphenCN");
            return newData;
        }

        private PriceDetail ExtractUSDPrice(IapItem iapItem)
        {
            List<PriceDetail> prices = iapItem.priceSets.PurchaseFee.priceMap.DEFAULT;
            foreach (var price in prices)
            {
                if (price.currency == "USD")
                {
                    return price;
                }
            }

            PriceDetail newUSDPrice = new PriceDetail();
            newUSDPrice.currency = "USD";
            prices.Add(newUSDPrice);
            return newUSDPrice;
        }

        private void AddIapItem(IapItem item, bool dirty = true, bool foldout = false)
        {
            _iapItems.Add(item);
            _iapItemDirty.Add(dirty);
            _iapItemsFoldout.Add(foldout);
            _iapValidationMsg.Add("");
        }

        private void RemoveIapItemLocally(int pos)
        {
            _iapItems.RemoveAt(pos);
            _iapItemDirty.RemoveAt(pos);
            _iapItemsFoldout.RemoveAt(pos);
            _iapValidationMsg.RemoveAt(pos);
        }

        private void RemoveTestAccountLocally(int pos)
        {
            _testAccounts.RemoveAt(pos);
            _testAccountsDirty.RemoveAt(pos);
            _testAccountsValidationMsg.RemoveAt(pos);
        }

        private void ClearIapItems()
        {
            _iapItems = new List<IapItem>();
            _iapItemDirty = new List<bool>();
            _iapItemsFoldout = new List<bool>();
            _iapValidationMsg = new List<string>();
        }

        private void RefreshAllInformation()
        {
            ClearIapItems();
            _isOperationRunning = true;
            _targetStep = STEP_GET_CLIENT;
            CallApiAsync();
        }

        // product id, client id or game id or test account email
        // For IAPs, we add product id to the list
        // For ClientSettings, we add client id to the list
        // For Game Settings, we add game id to the list
        // For Test Accounts, we add the email.
        private void AddPushRequests(string id)
        {
            pushRequestList.Add(id);
        }

        private void RemovePushRequest(string id)
        {
            pushRequestList.Remove(id);
        }

        private void DeleteIAPItem(IapItem iapItem, int pos)
        {
            _isIapUpdating = true;
            UnityWebRequest request = AppStoreOnboardApi.DeleteStoreItem(iapItem.id);
            IapItemDeleteResponse clientResp = new IapItemDeleteResponse();
            ReqStruct reqStruct = new ReqStruct
            {
                request = request,
                resp = clientResp,
                arrayPos = pos,
                curIapItem = iapItem
            };
            _requestQueue.Enqueue(reqStruct);
        }

        private void DeleteTestAccount(TestAccount account, int pos)
        {
            _isTestAccountUpdating = true;
            UnityWebRequest request = AppStoreOnboardApi.DeleteTestAccount(account.playerId);
            PlayerDeleteResponse response = new PlayerDeleteResponse();
            ReqStruct reqStruct = new ReqStruct
            {
                request = request,
                resp = response,
                arrayPos = pos,
                currTestAccount = account
            };
            _requestQueue.Enqueue(reqStruct);
        }

        private void UpdateIAPItem(IapItem iapItem, int pos)
        {
            iapItem.status = "STAGE";
            UnityWebRequest request = AppStoreOnboardApi.UpdateStoreItem(iapItem);
            UnityIapItemUpdateResponse resp = new UnityIapItemUpdateResponse();
            ReqStruct reqStruct = new ReqStruct
            {
                request = request,
                resp = resp,
                arrayPos = pos,
                curIapItem = iapItem
            };
            _requestQueue.Enqueue(reqStruct);
        }

        public void CreateIAPItem(IapItem iapItem, int pos)
        {
            UnityWebRequest request = AppStoreOnboardApi.CreateStoreItem(iapItem);
            UnityIapItemCreateResponse clientResp = new UnityIapItemCreateResponse();
            ReqStruct reqStruct = new ReqStruct();
            reqStruct.request = request;
            reqStruct.resp = clientResp;
            reqStruct.curIapItem = iapItem;
            reqStruct.arrayPos = pos;
            reqStruct.targetStep = "DIALOG";
            _requestQueue.Enqueue(reqStruct);
        }

        private int EditedIAPCount()
        {
            if (_iapItemDirty == null)
                return 0;

            int count = 0;

            foreach (bool dirty in _iapItemDirty)
            {
                if (dirty)
                {
                    count++;
                }
            }

            return count;
        }

        bool CheckURL(String URL)
        {
            string pattern =
                @"^(https?://[\w\-]+(\.[\w\-]+)+(:\d+)?((/[\w\-]*)?)*(\?[\w\-]+=[\w\-]+((&[\w\-]+=[\w\-]+)?)*)?)?$";
            return new Regex(pattern, RegexOptions.IgnoreCase).IsMatch(URL);
        }

        private void UpdateCallbackUrl()
        {
            UnityClientInfo unityClientInfo = new UnityClientInfo();
            unityClientInfo.ClientId = _unityClientId.stringValue;
            string callbackUrl = _callbackUrlLast;
            UnityWebRequest newRequest =
                AppStoreOnboardApi.UpdateUnityClient(Application.cloudProjectId, unityClientInfo,
                    callbackUrl);
            UnityClientResponse clientResp = new UnityClientResponse();
            ReqStruct newReqStruct = new ReqStruct();
            newReqStruct.request = newRequest;
            newReqStruct.resp = clientResp;
            newReqStruct.targetStep = STEP_UPDATE_CLIENT;
            _requestQueue.Enqueue(newReqStruct);
        }

        private void UpdateGameTitle()
        {
            _currentAppItem.status = "STAGE";
            UnityWebRequest newRequest = AppStoreOnboardApi.UpdateAppItem(_currentAppItem);
            AppItemResponse appItemResponse = new AppItemResponse();
            ReqStruct newReqStruct = new ReqStruct();
            newReqStruct.request = newRequest;
            newReqStruct.resp = appItemResponse;
            newReqStruct.targetStep = STEP_UPDATE_GAME_TITLE;
            _requestQueue.Enqueue(newReqStruct);
        }

        // Remove failure entries from pushRequstList
        private void ProcessErrorRequest(ReqStruct reqStruct)
        {
            if (reqStruct.curIapItem != null)
            {
                RemovePushRequest(reqStruct.curIapItem.slug);
            }

            else if (reqStruct.resp.GetType() == typeof(UnityClientResponse))
            {
                RemovePushRequest(_unityClientId.stringValue);
            }

            else if (reqStruct.resp.GetType() == typeof(AppItemResponse))
            {
                RemovePushRequest(_currentAppItem.id);
            }

            else if (reqStruct.resp.GetType() == typeof(PlayerResponse) ||
                     reqStruct.resp.GetType() == typeof(PlayerChangePasswordResponse))
            {
                RemovePushRequest(_testAccounts[reqStruct.arrayPos].email);
            }
        }

        private void CreateTestAccount(TestAccount testAccount, int pos)
        {
            Player player = new Player();
            player.email = testAccount.email;
            player.password = testAccount.password;
            UnityWebRequest request =
                AppStoreOnboardApi.SaveTestAccount(player, _unityClientId.stringValue);
            PlayerResponse playerResponse = new PlayerResponse();
            ReqStruct reqStruct = new ReqStruct();
            reqStruct.request = request;
            reqStruct.resp = playerResponse;
            reqStruct.targetStep = null;
            reqStruct.arrayPos = pos;
            _requestQueue.Enqueue(reqStruct);
        }

        private void UpdateTestAccount(TestAccount testAccount, int pos)
        {
            PlayerChangePasswordRequest player = new PlayerChangePasswordRequest();
            player.password = testAccount.password;
            player.playerId = testAccount.playerId;
            UnityWebRequest request = AppStoreOnboardApi.UpdateTestAccount(player);
            PlayerChangePasswordResponse playerChangePasswordResponse = new PlayerChangePasswordResponse();
            ReqStruct reqStruct = new ReqStruct();
            reqStruct.request = request;
            reqStruct.resp = playerChangePasswordResponse;
            reqStruct.targetStep = null;
            reqStruct.arrayPos = pos;
            _requestQueue.Enqueue(reqStruct);
        }

        private bool AnythingChanged()
        {
            if (_gameTitleChanged || _callbackUrlChanged)
            {
                return true;
            }

            foreach (bool dirty in _iapItemDirty)
            {
                if (dirty)
                {
                    return true;
                }
            }

            foreach (bool dirty in _testAccountsDirty)
            {
                if (dirty)
                {
                    return true;
                }
            }

            return false;
        }

        #endregion
    }
#endif
}