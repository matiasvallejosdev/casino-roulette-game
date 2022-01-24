#if UNITY_EDITOR

using System.ComponentModel;
using System.IO;
using UnityEditor;
using UnityEditor.Recorder;
using UnityEditor.Recorder.Input;

namespace UnityEngine.Recorder.Examples
{
    /// <summary>
    /// This example shows how to set up a recording session via script, for an MP4 file.
    /// To use this example, add the MultipleRecordingsExample component to a GameObject.
    ///
    /// Enter the Play Mode to start the recording.
    /// The recording automatically stops when you exit the Play Mode or when you disable the component.
    ///
    /// This script saves the recording outputs in [Project Folder]/SampleRecordings.
    /// </summary>
    public class MovieRecorderExample : MonoBehaviour
    {
        RecorderController m_RecorderController;
        public bool m_RecordAudio = true;
        internal MovieRecorderSettings m_Settings = null;

        public FileInfo OutputFile
        {
            get
            {
                var fileName = m_Settings.OutputFile + ".mp4";
                return new FileInfo(fileName);
            }
        }

        void OnEnable()
        {
            Initialize();
        }

        internal void Initialize()
        {
            var controllerSettings = ScriptableObject.CreateInstance<RecorderControllerSettings>();
            m_RecorderController = new RecorderController(controllerSettings);

            var mediaOutputFolder = new DirectoryInfo(Path.Combine(Application.dataPath, "..", "SampleRecordings"));

            // Video
            m_Settings = ScriptableObject.CreateInstance<MovieRecorderSettings>();
            m_Settings.name = "My Video Recorder";
            m_Settings.Enabled = true;

            // This example performs an MP4 recording
            m_Settings.OutputFormat = MovieRecorderSettings.VideoRecorderOutputFormat.MP4;
            m_Settings.VideoBitRateMode = VideoBitrateMode.High;

            m_Settings.ImageInputSettings = new GameViewInputSettings
            {
                OutputWidth = 1920,
                OutputHeight = 1080
            };

            m_Settings.AudioInputSettings.PreserveAudio = m_RecordAudio;

            // Simple file name (no wildcards) so that FileInfo constructor works in OutputFile getter.
            m_Settings.OutputFile = mediaOutputFolder.FullName + "/" + "video";

            // Setup Recording
            controllerSettings.AddRecorderSettings(m_Settings);
            controllerSettings.SetRecordModeToManual();
            controllerSettings.FrameRate = 60.0f;

            RecorderOptions.VerboseMode = false;
            m_RecorderController.PrepareRecording();
            m_RecorderController.StartRecording();

            Debug.Log($"Started recording for file {OutputFile.FullName}");
        }

        void OnDisable()
        {
            m_RecorderController.StopRecording();
        }
    }
}

#endif
