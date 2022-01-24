using UnityEngine;

namespace UnityEditor.Recorder.Input
{
    [CustomPropertyDrawer(typeof(RenderTextureInputSettings))]
    class RenderTextureInputSettingsPropertyDrawer : InputPropertyDrawer<RenderTextureInputSettings>
    {
        SerializedProperty m_SourceRTxtr;
        SerializedProperty m_FlipFinalOutput;

        static class Styles
        {
            internal static readonly GUIContent RenderTextureLabel = new GUIContent("Render Texture", "The reference to the Render Texture to capture.");
            internal static readonly GUIContent FlipVerticalLabel = new GUIContent("Flip Vertical", "To flip the recorded output image vertically.");
        }

        protected override void Initialize(SerializedProperty property)
        {
            base.Initialize(property);

            if (m_SourceRTxtr == null)
                m_SourceRTxtr = property.FindPropertyRelative("renderTexture");

            if (m_FlipFinalOutput == null)
                m_FlipFinalOutput = property.FindPropertyRelative("flipFinalOutput");
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Initialize(property);
            --EditorGUI.indentLevel;
            EditorGUILayout.PropertyField(m_SourceRTxtr, Styles.RenderTextureLabel);

            var res = "N/A";
            if (m_SourceRTxtr.objectReferenceValue != null)
            {
                var renderTexture = (RenderTexture)m_SourceRTxtr.objectReferenceValue;
                res = string.Format("{0}x{1}", renderTexture.width, renderTexture.height);
            }
            EditorGUILayout.LabelField(new GUIContent("Size", "The size (width x height) of the targeted Render Texture."), new GUIContent(res));

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(m_FlipFinalOutput, Styles.FlipVerticalLabel);
            ++EditorGUI.indentLevel;
        }
    }
}
