#if (UNITY_5_6_OR_NEWER && !UNITY_5_6_0)

using System.Collections.Generic;
using UnityEditor;
using UnityEngine.Networking;

namespace UnityEngine.UDP.Editor.Analytics
{
    public static class WebRequestQueue
    {
        static Queue<EditorAnalyticsReqStruct> requestQueue = new Queue<EditorAnalyticsReqStruct>();
        private static bool attachedDelegate = false;

        public static void WebRequestUpdate()
        {
            if (requestQueue.Count == 0)
            {
                return;
            }

            EditorAnalyticsReqStruct reqStruct = requestQueue.Dequeue();
            UnityWebRequest request = reqStruct.webRequest;

            if (request != null && request.isDone)
            {
            }
            else
            {
                requestQueue.Enqueue(reqStruct);
            }
        }

        internal static void Enqueue(EditorAnalyticsReqStruct request)
        {
            if (!attachedDelegate)
            {
                EditorApplication.update += WebRequestUpdate;
            }

            requestQueue.Enqueue(request);
        }
    }

    struct EditorAnalyticsReqStruct
    {
        public string eventName;
        public UnityWebRequest webRequest;
    }
}

#endif