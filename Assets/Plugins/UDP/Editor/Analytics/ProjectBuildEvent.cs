#if (UNITY_5_6_OR_NEWER && !UNITY_5_6_0)

using UnityEditor;
using UnityEditor.Callbacks;

namespace UnityEngine.UDP.Editor.Analytics
{
    public static class ProjectBuildEvent
    {
        [PostProcessBuildAttribute]
        public static void OnPostProcessBuild(BuildTarget target, string pathToBuildProject)
        {
            if (target == BuildTarget.Android && Common.TargetUDP())
            {
                // Send to Analytics
                EditorAnalyticsReqStruct reqStruct = new EditorAnalyticsReqStruct
                {
                    eventName = EditorAnalyticsApi.k_ProjectBuildEventName,
                    webRequest = EditorAnalyticsApi.ProjectBuildEvent()
                };

                WebRequestQueue.Enqueue(reqStruct);
            }
        }
    }
}

#endif