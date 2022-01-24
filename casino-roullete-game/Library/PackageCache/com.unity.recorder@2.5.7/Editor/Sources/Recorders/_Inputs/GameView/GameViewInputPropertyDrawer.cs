using UnityEngine;

namespace UnityEditor.Recorder.Input
{
    [CustomPropertyDrawer(typeof(GameViewInputSettings))]
    class GameViewInputPropertyDrawer : InputPropertyDrawer<GameViewInputSettings>
    {
        SerializedProperty m_OutputResolution;

        static class Styles
        {
            internal static readonly GUIContent OutputResolutionLabel = new GUIContent("Output Resolution", "Allows you to set the dimensions of the recorded output using different methods.\n\nIf you select a predefined vertical resolution, you must also select a specific Aspect Ratio.");
        }

        protected override void Initialize(SerializedProperty property)
        {
            if (target != null)
                return;

            base.Initialize(property);

            m_OutputResolution = property.FindPropertyRelative("m_OutputResolution");
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Initialize(property);
            --EditorGUI.indentLevel;
            EditorGUILayout.PropertyField(m_OutputResolution, Styles.OutputResolutionLabel);
            ++EditorGUI.indentLevel;
        }
    }
}
