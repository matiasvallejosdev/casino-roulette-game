using UnityEditor.Recorder.FrameCapturer;
using UnityEngine;

namespace UnityEditor.Recorder
{
    /// <summary>
    /// A class that represents the settings of a GIF Recorder.
    /// </summary>
    [RecorderSettings(typeof(GIFRecorder), "GIF Animation", "imagesequence_16")]
    public class GIFRecorderSettings : BaseFCRecorderSettings
    {
        [SerializeField] internal fcAPI.fcGifConfig gifEncoderSettings = fcAPI.fcGifConfig.default_value;

        /// <summary>
        /// The number of colors used in the GIF palette for the output. Maximum is 256.
        /// </summary>
        public int NumColors
        {
            get { return gifEncoderSettings.numColors; }
            set { gifEncoderSettings.numColors = Mathf.Clamp(value, 1, 256); }
        }

        /// <summary>
        /// The frame interval between keyframes (value between 1 and 120). This defines how many frames share the same color palette.
        /// </summary>
        public int KeyframeInterval
        {
            get { return gifEncoderSettings.keyframeInterval; }
            set { gifEncoderSettings.keyframeInterval = Mathf.Clamp(value, 1, 120); }
        }


        /// <summary>
        /// The maximum number of threads to use to encode output frames (value between 1 and 32).
        /// </summary>
        public int MaxTasks
        {
            get { return gifEncoderSettings.maxTasks; }
            set { gifEncoderSettings.maxTasks = Mathf.Clamp(value, 1, 32); }
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public GIFRecorderSettings()
        {
            fileNameGenerator.FileName = "gif_animation_" + DefaultWildcard.Take;

            m_ImageInputSelector.cameraInputSettings.FlipFinalOutput = true;
            m_ImageInputSelector.renderTextureInputSettings.FlipFinalOutput = true;
            m_ImageInputSelector.renderTextureSamplerSettings.FlipFinalOutput = true;
        }

        /// <inheritdoc/>
        protected internal override string Extension
        {
            get { return "gif"; }
        }
    }
}
