using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace UnityEditor.Recorder
{
    /// <summary>
    /// Sets which source camera to use for recording (by some specific Recorders).
    /// </summary>
    [Flags]
    public enum ImageSource
    {
        /// <summary>
        /// Use the current active camera.
        /// </summary>
        ActiveCamera = 1,
        /// <summary>
        /// Use the main camera.
        /// </summary>
        MainCamera = 2,
        /// <summary>
        /// Use the first camera that matches a GameObject tag.
        /// </summary>
        TaggedCamera = 4
    }

    /// <summary>
    /// Sets which frame rate type to use during recording.
    /// </summary>
    public enum FrameRatePlayback
    {
        /// <summary>
        /// The frame rate doesn't vary during recording, even if the actual frame rate is lower or higher.
        /// </summary>
        Constant,

        /// <summary>
        /// Use the application's frame rate, which might vary during recording. This option is not supported by all Recorders.
        /// </summary>
        Variable,
    }

    /// <summary>
    /// The mode that defines the way to manage the starting point and duration of the recording.
    /// </summary>
    public enum RecordMode
    {
        /// <summary>
        /// Record every frame between when the recording is started and when it is stopped (either using the UI or through API methods).
        /// </summary>
        Manual,

        /// <summary>
        /// Record one single frame according to the specified frame number.
        /// </summary>
        SingleFrame,

        /// <summary>
        /// Record all frames within an interval of frames according to the specified Start and End frame numbers.
        /// </summary>
        FrameInterval,

        /// <summary>
        /// Record all frames within a time interval according to the specified Start time and End time.
        /// </summary>
        TimeInterval
    }

    /// <summary>
    /// Main base class for a Recorder settings.
    /// Each Recorder needs to have its corresponding settings properly configured.
    /// </summary>
    public abstract class RecorderSettings : ScriptableObject
    {
        private static string s_OutputFileErrorMessage = "Recorder output file cannot be empty";
        /// <summary>
        /// Stores the path this Recorder will use to generate the output file.
        /// It can be either an absolute or a relative path.
        /// The file extension is automatically added.
        /// Wildcards such as <c>DefaultWildcard.Time</c> are supported.
        /// <seealso cref="DefaultWildcard"/>
        /// </summary>
        public string OutputFile
        {
            get { return fileNameGenerator.ToPath(); }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentException(s_OutputFileErrorMessage);

                fileNameGenerator.FromPath(value);
            }
        }

        /// <summary>
        /// Indicates if this Recorder is active when starting the recording. If false, the Recorder is ignored and generates no output.
        /// </summary>
        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }

        [SerializeField] private bool enabled = true;

        /// <summary>
        /// Stores the current Take number. Automatically incremented after each recording session.
        /// </summary>
        public int Take
        {
            get { return take; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException($"The take number must be positive");
                take = value;
            }
        }

        [SerializeField] internal int take = 1;

        /// <summary>
        /// Stores the file extension used by this Recorder (without the dot).
        /// </summary>
        protected internal abstract string Extension { get; }

        [SerializeField] internal int captureEveryNthFrame = 1;

        [SerializeField] internal FileNameGenerator fileNameGenerator;

        /// <summary>
        /// The object that resolves wildcards into a final path for output files.
        /// </summary>
        public FileNameGenerator FileNameGenerator => fileNameGenerator;

        /// <summary>
        /// The mode that defines the way to manage the starting point and duration of the recording: either manually
        /// or within a specific time or frame interval.
        /// </summary>
        public RecordMode RecordMode { get; set; }

        /// <summary>
        /// The type of frame rate to use in the recording: constant or variable.
        /// </summary>
        public FrameRatePlayback FrameRatePlayback { get; set; }

        float frameRate = 30;

        /// <summary>
        /// The number of recorded frames per second. In constant frame rate mode, this represent a target value, while
        /// in variable frame rate mode, this represents a maximum value.
        /// </summary>
        /// <seealso cref="FrameRatePlayback"/>
        public float FrameRate
        {
            get => frameRate;
            set => frameRate = value;
        }

        /// <summary>
        /// The start frame of the recording.
        /// </summary>
        public int StartFrame { get; set; }

        /// <summary>
        /// The end frame of the recording.
        /// </summary>
        public int EndFrame { get; set; }

        /// <summary>
        /// The start time of the recording.
        /// </summary>
        public float StartTime { get; set; }

        /// <summary>
        /// The end time of the recording.
        /// </summary>
        public float EndTime { get; set; }

        /// <summary>
        /// Specifies whether or not to limit the frame rate when it is above the target frame rate.
        /// </summary>
        public bool CapFrameRate { get; set; }

        /// <summary>
        /// The constructor of the class.
        /// </summary>
        protected RecorderSettings()
        {
            fileNameGenerator = new FileNameGenerator(this)
            {
                Root = OutputPath.Root.Project,
                Leaf = "Recordings"
            };
        }

        /// <summary>
        /// Tests if the Recorder is correctly configured.
        /// </summary>
        /// <param name="errors">List of errors encountered.</param>
        /// <returns>True if there are no errors, False otherwise.</returns>
        protected internal virtual bool ValidityCheck(List<string> errors)
        {
            var ok = true;

            if (InputsSettings != null)
            {
                var inputErrors = new List<string>();

                var valid = InputsSettings.All(x => x.ValidityCheck(inputErrors));

                if (!valid)
                {
                    errors.AddRange(inputErrors);
                    ok = false;
                }
            }

            if (string.IsNullOrEmpty(fileNameGenerator.FileName))
            {
                errors.Add("Missing file name");
                ok = false;
            }

            if (Math.Abs(FrameRate) <= float.Epsilon)
            {
                ok = false;
                errors.Add("Invalid frame rate");
            }

            if (!IsPlatformSupported)
            {
                errors.Add("Current platform is not supported");
                ok  = false;
            }

            return ok;
        }

        /// <summary>
        /// Indicates if the current platform is supported (True) or not (False).
        /// </summary>
        public virtual bool IsPlatformSupported
        {
            get { return true; }
        }

        /// <summary>
        /// Stores the list of Input settings required by this Recorder.
        /// </summary>
        public abstract IEnumerable<RecorderInputSettings> InputsSettings { get; }

        /// <summary>
        /// Performs additional changes on the selected input's settings based on the context of the recording,
        /// such as enforcing limitations due to file formats.
        /// </summary>
        internal virtual void SelfAdjustSettings()
        {
        }

        /// <summary>
        /// Override this method if any post treatment needs to be done after this Recorder is duplicated in the Recorder Window.
        /// </summary>
        public virtual void OnAfterDuplicate()
        {
        }

        protected internal virtual bool HasErrors()
        {
            return false;
        }

        internal virtual bool HasWarnings()
        {
            return !ValidityCheck(new List<string>());
        }

        /// <summary>
        /// Validation of serialized value.
        /// </summary>
        internal void OnValidate()
        {
            captureEveryNthFrame = Mathf.Max(1, captureEveryNthFrame);
            take = Mathf.Max(0, take);
        }
    }
}
