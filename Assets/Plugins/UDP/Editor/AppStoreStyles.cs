using UnityEditor;

namespace UnityEngine.UDP.Editor
{
    public static class AppStoreStyles
    {
        public const string kNoUnityProjectIDErrorMessage =
            "Unity Project ID doesn't exist, please go to Window/Services to create one.";

        public const int kAppStoreSettingsButtonWidth = 80;
        private static GUIStyle kAppStoreSettingsHeaderGuiStyle;

        public static GUIStyle KAppStoreSettingsHeaderGuiStyle
        {
            get
            {
                if (kAppStoreSettingsHeaderGuiStyle == null)
                {
                    kAppStoreSettingsHeaderGuiStyle = new GUIStyle(EditorStyles.foldout)
                    {
                        fontSize = 12,
                        fontStyle = FontStyle.Bold
                    };
                }

                return kAppStoreSettingsHeaderGuiStyle;
            }
        }

//TODO: this will cause a NPE error, figure out why            
//            public static readonly GUIStyle kAppStoreSettingsHeaderGuiStyle2 = new GUIStyle(EditorStyles.foldout)
//            {
//                fontSize = 12,
//                fontStyle = FontStyle.Bold
//            };

        public const int kClientLabelWidth = 140;
        public static int kTestAccountBoxHeight = 25;
        public const float kAddNewIapButtonRatio = 1.5f;
        public const int kAddNewIapButtonFontSize = 12;
        public const int kCopyButtonWidth = 80;
        public const int kGoToPortalButtonWidth = 120;
    }
}