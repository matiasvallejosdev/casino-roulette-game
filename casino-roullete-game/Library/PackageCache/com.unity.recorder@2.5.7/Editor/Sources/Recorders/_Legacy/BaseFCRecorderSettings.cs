using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Recorder.Input;

namespace UnityEditor.Recorder.FrameCapturer
{
    /// <summary>
    /// The settings common to all recordings that capture image data.
    /// </summary>
    public abstract class BaseFCRecorderSettings : RecorderSettings
    {
        [SerializeField] internal UTJImageInputSelector m_ImageInputSelector = new UTJImageInputSelector();

        protected internal override bool ValidityCheck(List<string> errors)
        {
            var ok = base.ValidityCheck(errors);
            return ok;
        }

        /// <summary>
        /// The properties of the image input.
        /// </summary>
        public ImageInputSettings imageInputSettings
        {
            get { return m_ImageInputSelector.imageInputSettings; }
            set { m_ImageInputSelector.imageInputSettings = value; }
        }

        /// <summary>
        /// Specifies whether this class is supported on the current platform or not.
        /// </summary>
        public override bool IsPlatformSupported
        {
            get
            {
                return Application.platform == RuntimePlatform.WindowsEditor ||
                    Application.platform == RuntimePlatform.WindowsPlayer ||
                    Application.platform == RuntimePlatform.OSXEditor ||
                    Application.platform == RuntimePlatform.OSXPlayer ||
                    Application.platform == RuntimePlatform.LinuxEditor ||
                    Application.platform == RuntimePlatform.LinuxPlayer;
            }
        }

        /// <summary>
        /// The list of settings of the Recorder inputs.
        /// </summary>
        public override IEnumerable<RecorderInputSettings> InputsSettings
        {
            get { yield return m_ImageInputSelector.Selected; }
        }
    }
}
