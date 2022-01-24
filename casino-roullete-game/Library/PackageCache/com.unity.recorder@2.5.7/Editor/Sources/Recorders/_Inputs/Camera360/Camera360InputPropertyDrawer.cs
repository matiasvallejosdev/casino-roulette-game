using UnityEngine;

namespace UnityEditor.Recorder.Input
{
    [CustomPropertyDrawer(typeof(Camera360InputSettings))]
    class Camera360InputPropertyDrawer : InputPropertyDrawer<Camera360InputSettings>
    {
        static ImageSource m_SupportedSources = ImageSource.MainCamera | ImageSource.TaggedCamera;
        string[] m_MaskedSourceNames;

        SerializedProperty m_Source;
        SerializedProperty m_CameraTag;
        SerializedProperty m_FlipFinalOutput;
        SerializedProperty m_StereoSeparation;
        SerializedProperty m_CubeMapSz;
        SerializedProperty m_OutputWidth;
        SerializedProperty m_OutputHeight;
        SerializedProperty m_RenderStereo;

        static class Styles
        {
            static readonly GUIContent s_WidthLabel = new GUIContent("W");
            static readonly GUIContent s_HeightLabel = new GUIContent("H");

            internal static readonly GUIContent TagLabel  = new GUIContent("Tag", "The Tag identifying the camera to use.");
            internal static readonly GUIContent OutputLabel = new GUIContent("Output Dimensions", "The width and height, in pixels, of the 360-degree video.");
            internal static readonly GUIContent[] DimensionLabels = { s_WidthLabel, s_HeightLabel };
            internal static readonly GUIContent CubeMapLabel = new GUIContent("Cube Map Size", "The side length of the cube map, in pixels, for the 360-degree video.");
            internal static readonly GUIContent[] CubeDimensionLabel = { s_WidthLabel };
            internal static readonly GUIContent StereoLabel = new GUIContent("Record in Stereo", "To record separate left and right stereoscopic views of the 360-degree video.\n\nThe Recorder includes both views in the same image: left view at the top and right view at the bottom.");
            internal static readonly GUIContent StereoSeparationLabel = new GUIContent("Stereo Separation", "The angle between the left and right views on the selected Camera's Y axis.");
            internal static readonly GUIContent FlipVerticalLabel = new GUIContent("Flip Vertical", "To flip the recorded output image vertically.");
        }

        protected override void Initialize(SerializedProperty property)
        {
            base.Initialize(property);

            m_Source = property.FindPropertyRelative("source");
            m_CameraTag = property.FindPropertyRelative("cameraTag");

            m_StereoSeparation = property.FindPropertyRelative("stereoSeparation");
            m_FlipFinalOutput = property.FindPropertyRelative("flipFinalOutput");
            m_CubeMapSz = property.FindPropertyRelative("mapSize");
            m_OutputWidth = property.FindPropertyRelative("m_OutputWidth");
            m_OutputHeight = property.FindPropertyRelative("m_OutputHeight");
            m_RenderStereo = property.FindPropertyRelative("renderStereo");
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Initialize(property);
            --EditorGUI.indentLevel;
            using (var check = new EditorGUI.ChangeCheckScope())
            {
                if (m_MaskedSourceNames == null)
                    m_MaskedSourceNames = EnumHelper.MaskOutEnumNames<ImageSource>((int)m_SupportedSources);

                var index = EnumHelper.GetMaskedIndexFromEnumValue<ImageSource>(m_Source.intValue, (int)m_SupportedSources);
                index = EditorGUILayout.Popup(new GUIContent("Camera", "The camera to use for the recording."), index, m_MaskedSourceNames);

                if (check.changed)
                    m_Source.intValue = EnumHelper.GetEnumValueFromMaskedIndex<ImageSource>(index, (int)m_SupportedSources);
            }

            if ((ImageSource)m_Source.intValue == ImageSource.TaggedCamera)
            {
                ++EditorGUI.indentLevel;
                EditorGUILayout.PropertyField(m_CameraTag, Styles.TagLabel);
                --EditorGUI.indentLevel;
            }

            var outputDimensions = new int[2];
            outputDimensions[0] = m_OutputWidth.intValue;
            outputDimensions[1] = m_OutputHeight.intValue;

            if (UIElementHelper.MultiIntField(Styles.OutputLabel, Styles.DimensionLabels, outputDimensions))
            {
                m_OutputWidth.intValue = outputDimensions[0];
                m_OutputHeight.intValue = outputDimensions[1];
            }

            var cubeMapWidth = new int[1];
            cubeMapWidth[0] = m_CubeMapSz.intValue;
            outputDimensions[1] = m_OutputHeight.intValue;

            if (UIElementHelper.MultiIntField(Styles.CubeMapLabel, Styles.CubeDimensionLabel, cubeMapWidth))
            {
                m_CubeMapSz.intValue = cubeMapWidth[0];
            }

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(m_RenderStereo, Styles.StereoLabel);

            ++EditorGUI.indentLevel;
            using (new EditorGUI.DisabledScope(!m_RenderStereo.boolValue))
            {
                EditorGUILayout.PropertyField(m_StereoSeparation, Styles.StereoSeparationLabel);
            }
            --EditorGUI.indentLevel;

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(m_FlipFinalOutput, Styles.FlipVerticalLabel);
            ++EditorGUI.indentLevel;
        }
    }
}
