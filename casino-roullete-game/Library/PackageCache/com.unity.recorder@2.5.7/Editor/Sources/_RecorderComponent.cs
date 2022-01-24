using UnityEngine;

namespace UnityEditor.Recorder
{
    class RecorderComponent : _FrameRequestComponent
    {
        public RecordingSession session { get; set; }

        public void LateUpdate()
        {
            if (session != null && session.isRecording)
            {
                session.PrepareNewFrame();
            }

            if (session != null && session.isRecording)
            {
                RequestNewFrame();
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (session != null)
                session.Dispose();
        }

        protected override void FrameReady()
        {
#if DEBUG_RECORDER_TIMING
            Debug.LogFormat("FrameReady Out at frame # {0} - {1} - {2} ", Time.renderedFrameCount, Time.time, Time.deltaTime);
#endif
#if DEBUG_RECORDER_TIMING
            Debug.LogFormat("FrameReady IN at frame # {0} - {1} - {2} ", Time.renderedFrameCount, Time.time, Time.deltaTime);
#endif
            session.RecordFrame();

            switch (session.recorder.settings.RecordMode)
            {
                case RecordMode.Manual:
                    break;
                case RecordMode.SingleFrame:
                {
                    if (session.recorder.RecordedFramesCount == 1)
                        Destroy(this);
                    break;
                }
                case RecordMode.FrameInterval:
                {
                    if (session.frameIndex > session.settings.EndFrame)
                        Destroy(this);
                    break;
                }
                case RecordMode.TimeInterval:
                {
                    if (session.settings.FrameRatePlayback == FrameRatePlayback.Variable)
                    {
                        if (session.currentFrameStartTS >= session.settings.EndTime)
                            Destroy(this);
                    }
                    else
                    {
                        var expectedFrames = (session.settings.EndTime - session.settings.StartTime) *
                            session.settings.FrameRate;
                        if (session.RecordedFrameSpan >= expectedFrames)
                            Destroy(this);
                    }

                    break;
                }
            }
        }
    }
}
