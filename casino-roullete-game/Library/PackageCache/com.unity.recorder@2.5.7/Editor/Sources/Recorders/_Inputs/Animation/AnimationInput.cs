using UnityEditor.Animations;

namespace UnityEditor.Recorder.Input
{
    /// <summary>
    /// Use this class to record animations in the scene in the Unity native format.
    /// </summary>
    public class AnimationInput : RecorderInput
    {
        /// <summary>
        /// Indicates the internal GameObject Recorder to use for the capture.
        /// </summary>
        public GameObjectRecorder GameObjectRecorder { get; private set; }
        double m_Time;

        /// <inheritdoc/>
        protected internal override void BeginRecording(RecordingSession session)
        {
            var aniSettings = (AnimationInputSettings)settings;

            var srcGO = aniSettings.gameObject;

            if (srcGO == null)
                throw new System.NullReferenceException("srcGO");

            GameObjectRecorder = new GameObjectRecorder(srcGO);

            foreach (var binding in aniSettings.bindingType)
            {
                GameObjectRecorder.BindComponentsOfType(srcGO, binding, aniSettings.Recursive);
            }

            m_Time = session.currentFrameStartTS;
        }

        /// <inheritdoc/>
        protected internal override void NewFrameReady(RecordingSession session)
        {
            if (GameObjectRecorder != null && session.isRecording)
            {
                var dt = (float)(session.currentFrameStartTS - m_Time);
                GameObjectRecorder.TakeSnapshot(dt);
                m_Time = session.currentFrameStartTS;
            }
        }
    }
}
