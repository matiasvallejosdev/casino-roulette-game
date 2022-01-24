using UnityEditor.Animations;
using UnityEditor.Recorder.Input;
using UnityEngine;

namespace UnityEditor.Recorder
{
    class AnimationRecorder : GenericRecorder<AnimationRecorderSettings>
    {
        protected internal override void RecordFrame(RecordingSession session)
        {
        }

        protected internal override void EndRecording(RecordingSession session)
        {
            var ars = (AnimationRecorderSettings)session.settings;

            foreach (var input in m_Inputs)
            {
                var aInput = (AnimationInput)input;

                if (aInput.GameObjectRecorder == null)
                    continue;

                var clip = new AnimationClip();

                ars.fileNameGenerator.CreateDirectory(session);

                var absolutePath = FileNameGenerator.SanitizePath(ars.fileNameGenerator.BuildAbsolutePath(session));
                var clipName = absolutePath.Replace(FileNameGenerator.SanitizePath(Application.dataPath), "Assets");

                AssetDatabase.CreateAsset(clip, clipName);
                var aniSettings = (aInput.settings as AnimationInputSettings);
                aInput.GameObjectRecorder.SaveToClip(clip, ars.FrameRate, aniSettings.CurveFilterOptions);
                if (aniSettings.ClampedTangents)
                {
                    FilterClip(clip);
                }

                aInput.GameObjectRecorder.ResetRecording();
            }

            base.EndRecording(session);
            UnityEditor.AssetDatabase.SaveAssets();
        }

        void FilterClip(AnimationClip clip)
        {
            foreach (var bind in AnimationUtility.GetCurveBindings(clip))
            {
                var curve = AnimationUtility.GetEditorCurve(clip, bind);
                for (var i = 0; i < curve.keys.Length; ++i)
                {
                    AnimationUtility.SetKeyLeftTangentMode(curve, i, AnimationUtility.TangentMode.ClampedAuto);
                    AnimationUtility.SetKeyRightTangentMode(curve, i, AnimationUtility.TangentMode.ClampedAuto);
                }
                AnimationUtility.SetEditorCurve(clip, bind , curve);
            }
        }
    }
}
