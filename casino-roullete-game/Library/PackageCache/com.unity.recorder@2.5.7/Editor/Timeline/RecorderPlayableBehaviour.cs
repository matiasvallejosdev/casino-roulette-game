using UnityEditor.Recorder.Input;
using UnityEngine.Playables;

namespace UnityEditor.Recorder.Timeline
{
    class RecorderPlayableBehaviour : PlayableBehaviour
    {
        PlayState m_PlayState = PlayState.Paused;
        public RecordingSession session { get; set; }
        WaitForEndOfFrameComponent endOfFrameComp;
        bool m_FirstOneSkipped;

        public override void OnGraphStart(Playable playable)
        {
            if (session != null)
            {
                // does not support multiple starts...
                session.SessionCreated();
                m_PlayState = PlayState.Paused;
            }
        }

        public override void OnGraphStop(Playable playable)
        {
            if (session != null && session.isRecording)
            {
                session.EndRecording();
                session.Dispose();
                session = null;
            }
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            if (session != null)
            {
                if (endOfFrameComp == null)
                {
                    endOfFrameComp = session.recorderGameObject.AddComponent<WaitForEndOfFrameComponent>();
                    endOfFrameComp.m_playable = this;
                }
            }

            if (session != null && session.isRecording)
            {
                session.PrepareNewFrame();
            }
        }

        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            if (session == null)
                return;

            // Assumption: OnPlayStateChanged( PlayState.Playing ) ONLY EVER CALLED ONCE for this type of playable.
            m_PlayState = PlayState.Playing;
            var res = session.BeginRecording();
#if UNITY_EDITOR
            RecorderAnalytics.SendStartEvent(session);
            if (!res)
            {
                RecorderAnalytics.SendStopEvent(session, true, false);
            }
#endif
        }

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            if (session == null)
                return;

            if (session.isRecording && m_PlayState == PlayState.Playing)
            {
#if UNITY_EDITOR
                const double eps = 1e-5; // end is never evaluated
                RecorderAnalytics.SendStopEvent(session, false, playable.GetTime() >= playable.GetDuration() - eps);
#endif
                session.Dispose();
                session = null;
            }

            m_PlayState = PlayState.Paused;
        }

        public void FrameEnded()
        {
            if (session != null && session.isRecording)
                session.RecordFrame();
        }
    }
}
