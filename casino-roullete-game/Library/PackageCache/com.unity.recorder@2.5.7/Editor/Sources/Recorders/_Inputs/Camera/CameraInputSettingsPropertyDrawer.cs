using UnityEngine;

namespace UnityEditor.Recorder.Input
{
    [CustomPropertyDrawer(typeof(CameraInputSettings))]
    class CameraInputSettingsPropertyDrawer : InputPropertyDrawer<CameraInputSettings>
    {
        static ImageSource m_SupportedSources = ImageSource.MainCamera | ImageSource.ActiveCamera | ImageSource.TaggedCamera;
        string[] m_MaskedSourceNames;

        SerializedProperty m_Source;
        SerializedProperty m_CameraTag;
        SerializedProperty m_FlipFinalOutput;
        SerializedProperty m_IncludeUI;
        SerializedProperty m_OutputResolution;

        bool m_Initialized;

        static class Styles
        {
            internal static readonly GUIContent CameraLabel  = new GUIContent("Camera", "The camera to use for the recording.");
            internal static readonly GUIContent TagLabel  = new GUIContent("Tag", "The Tag identifying the camera to use.");
            internal static readonly GUIContent IncludeUILabel = new GUIContent("Include UI", "To include UI GameObjects in the recording.");
            internal static readonly GUIContent FlipVerticalLabel = new GUIContent("Flip Vertical", "To flip the recorded output image vertically.");
            internal static readonly GUIContent OutputResolutionLabel = new GUIContent("Output Resolution", "Allows you to set the dimensions of the recorded output using different methods.\n\nIf you select a predefined vertical resolution, you must also select a specific Aspect Ratio.");
        }

        protected override void Initialize(SerializedProperty property)
        {
            if (m_Initialized)
                return;

            base.Initialize(property);

            m_Source = property.FindPropertyRelative("source");
            m_CameraTag = property.FindPropertyRelative("cameraTag");
            m_OutputResolution = property.FindPropertyRelative("m_OutputResolution");
            m_FlipFinalOutput = property.FindPropertyRelative("flipFinalOutput");
            m_IncludeUI = property.FindPropertyRelative("captureUI");

            m_Initialized = true;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            --EditorGUI.indentLevel;
            Initialize(property);
            if (CameraInputSettings.UsingHDRP())
            {
                m_SupportedSources = ImageSource.MainCamera | ImageSource.TaggedCamera;
            }


            using (var check = new EditorGUI.ChangeCheckScope())
            {
                if (m_MaskedSourceNames == null)
                    m_MaskedSourceNames = EnumHelper.MaskOutEnumNames<ImageSource>((int)m_SupportedSources);

                var index = EnumHelper.GetMaskedIndexFromEnumValue<ImageSource>(m_Source.intValue, (int)m_SupportedSources);
                index = EditorGUILayout.Popup(Styles.CameraLabel, index, m_MaskedSourceNames);

                if (check.changed)
                    m_Source.intValue = EnumHelper.GetEnumValueFromMaskedIndex<ImageSource>(index, (int)m_SupportedSources);
            }

            var inputType = (ImageSource)m_Source.intValue;
            if ((ImageSource)m_Source.intValue == ImageSource.TaggedCamera)
            {
                ++EditorGUI.indentLevel;
                EditorGUILayout.PropertyField(m_CameraTag, Styles.TagLabel);
                --EditorGUI.indentLevel;
            }

            EditorGUILayout.PropertyField(m_OutputResolution, Styles.OutputResolutionLabel);

            if (inputType == ImageSource.ActiveCamera)
            {
                EditorGUILayout.PropertyField(m_IncludeUI, Styles.IncludeUILabel);
            }

            EditorGUILayout.PropertyField(m_FlipFinalOutput, Styles.FlipVerticalLabel);
            ++EditorGUI.indentLevel;
        }
    }
}
