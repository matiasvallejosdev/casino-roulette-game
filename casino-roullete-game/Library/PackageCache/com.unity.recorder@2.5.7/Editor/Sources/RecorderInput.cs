using System;

namespace UnityEditor.Recorder
{
    /// <summary>
    /// This class represents the different types of input that can receive frame data from a <see cref="RecordingSession"/> object.
    /// </summary>
    /// <seealso cref="RecorderInputSettings"/>
    public class RecorderInput : IDisposable
    {
        /// <summary>
        /// The settings of the Recorder input data.
        /// These can be of type <see cref="Input.AnimationInputSettings"/>, <see cref="Input.ImageInputSettings"/>,
        /// or <see cref="Input.AudioInputSettings"/>.
        /// </summary>
        public RecorderInputSettings settings { get; set; }

        /// <summary>
        /// The finalizer of the class.
        /// </summary>
        ~RecorderInput()
        {
            Dispose(false);
        }

        /// <summary>
        /// Cleans up the Recorder input's resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Cleans up the Recorder input's resources.
        /// </summary>
        /// <param name="disposing">This flag is True when the method is being called by a user's <see cref="Dispose"/>
        /// method, otherwise it means that it has been called by a finalizer and you should only dispose of unmanaged
        /// resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            GC.SuppressFinalize(this);
        }

        protected internal virtual void SessionCreated(RecordingSession session) {}

        protected internal virtual void BeginRecording(RecordingSession session) {}

        protected internal virtual void NewFrameStarting(RecordingSession session) {}

        protected internal virtual void NewFrameReady(RecordingSession session) {}

        protected internal virtual void FrameDone(RecordingSession session) {}

        protected internal virtual void EndRecording(RecordingSession session) {}
    }
}
