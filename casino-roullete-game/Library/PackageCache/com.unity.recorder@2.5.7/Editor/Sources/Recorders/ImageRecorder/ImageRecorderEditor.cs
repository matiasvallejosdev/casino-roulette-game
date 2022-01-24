using UnityEditor.Recorder.Input;
using UnityEngine;

namespace UnityEditor.Recorder
{
    [CustomEditor(typeof(ImageRecorderSettings))]
    class ImageRecorderEditor : RecorderEditor
    {
        SerializedProperty m_OutputFormat;
        SerializedProperty m_CaptureAlpha;
        SerializedProperty m_ColorSpace;

        static class Styles
        {
            internal static readonly GUIContent FormatLabel = new GUIContent("Media File Format", "The file encoding format of the recorded output.");
            internal static readonly GUIContent CaptureAlphaLabel = new GUIContent("Include Alpha", "To Include the alpha channel in the recording.");
            internal static readonly GUIContent ColorSpace = new GUIContent("Color Space", "The color space (gamma curve, gamut) to use in the output images.\n\nIf you select an option to get unclamped values, you must:\n- Use High Definition Render Pipeline (HDRP).\n- Disable any Tonemapping in your Scene.\n- Disable Dithering on the selected Camera.");
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            if (target == null)
                return;

            var pf = new PropertyFinder<ImageRecorderSettings>(serializedObject);
            m_OutputFormat = pf.Find(w => w.OutputFormat);

            m_OutputFormat = serializedObject.FindProperty("outputFormat");
            m_CaptureAlpha = serializedObject.FindProperty("captureAlpha");
            m_ColorSpace = serializedObject.FindProperty("m_ColorSpace");
        }

        protected override void FileTypeAndFormatGUI()
        {
            EditorGUILayout.PropertyField(m_OutputFormat, Styles.FormatLabel);
            var imageSettings = (ImageRecorderSettings)target;
            if (!CameraInputSettings.UsingHDRP())
            {
                using (new EditorGUI.DisabledScope(!imageSettings.CanCaptureAlpha()))
                {
                    EditorGUILayout.PropertyField(m_CaptureAlpha, Styles.CaptureAlphaLabel);
                }
            }

            string[] list_of_colorspaces = new[] {"sRGB, sRGB", "Linear, sRGB (unclamped)"};

            if (imageSettings.CanCaptureHDRFrames())
            {
                m_ColorSpace.intValue =
                    EditorGUILayout.Popup(Styles.ColorSpace, m_ColorSpace.intValue, list_of_colorspaces);
            }
            else
            {
                using (new EditorGUI.DisabledScope(!imageSettings.CanCaptureHDRFrames()))
                {
                    EditorGUILayout.Popup(Styles.ColorSpace, 0, list_of_colorspaces);
                }
            }
        }
    }
}
