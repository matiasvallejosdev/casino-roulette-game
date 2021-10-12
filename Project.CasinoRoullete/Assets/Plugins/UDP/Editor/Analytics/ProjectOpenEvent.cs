using UnityEditor;
using UnityEngine.Networking;

#if (UNITY_5_6_OR_NEWER && !UNITY_5_6_0)
namespace UnityEngine.UDP.Editor.Analytics
{
    [InitializeOnLoad]
    public static class ProjectOpenEvent
    {
        private const string k_ProjectOpened = "UdpProjectOpened";

        static ProjectOpenEvent()
        {
            if (!SessionState.GetBool(k_ProjectOpened, false))
            {
                if (EditorUserBuildSettings.activeBuildTarget.Equals(BuildTarget.Android) && Common.TargetUDP())
                {
                    SessionState.SetBool(k_ProjectOpened, true);

                    UnityWebRequest request = EditorAnalyticsApi.ProjectOpened();

                    EditorAnalyticsReqStruct reqStruct = new EditorAnalyticsReqStruct
                    {
                        eventName = EditorAnalyticsApi.k_ProjectOpenEventName,
                        webRequest = request,
                    };

                    WebRequestQueue.Enqueue(reqStruct);
                }
            }
        }
    }
}
#endif