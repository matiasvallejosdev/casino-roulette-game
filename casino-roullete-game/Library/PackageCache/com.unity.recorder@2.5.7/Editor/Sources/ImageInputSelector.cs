using System;
using UnityEditor.Recorder.Input;
using UnityEngine;

namespace UnityEditor.Recorder
{
    /// <summary>
    /// This class is a specialization of InputSettingsSelector for image input types.
    /// </summary>
    [Serializable]
    public class ImageInputSelector : InputSettingsSelector
    {
        /// <summary>
        /// The settings when the input is set to Game View.
        /// </summary>
        [SerializeField] public GameViewInputSettings gameViewInputSettings = new GameViewInputSettings();

        /// <summary>
        /// The settings when the input is set to Targeted Camera.
        /// </summary>
        [SerializeField] public CameraInputSettings cameraInputSettings = new CameraInputSettings();

        /// <summary>
        /// The settings when the input is set to 360 View.
        /// </summary>
        [SerializeField] public Camera360InputSettings camera360InputSettings = new Camera360InputSettings();

        /// <summary>
        /// The settings when the input is set to Render Texture Asset.
        /// </summary>
        [SerializeField] public RenderTextureInputSettings renderTextureInputSettings = new RenderTextureInputSettings();

        /// <summary>
        /// The settings when the input is set to Texture Sampling.
        /// </summary>
        [SerializeField] public RenderTextureSamplerSettings renderTextureSamplerSettings = new RenderTextureSamplerSettings();

        /// <summary>
        /// Use this property to set and retrieve the input settings of the currently selected image.
        /// Supported input types are: CameraInputSettings, GameViewInputSettings, Camera360InputSettings, RenderTextureInputSettings, RenderTextureSamplerSettings.
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public ImageInputSettings ImageInputSettings
        {
            get { return (ImageInputSettings)Selected; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                if (value is CameraInputSettings ||
                    value is GameViewInputSettings ||
                    value is Camera360InputSettings ||
                    value is RenderTextureInputSettings ||
                    value is RenderTextureSamplerSettings)
                {
                    Selected = value;
                }
                else
                {
                    throw new ArgumentException("Video input type not supported: '" + value.GetType() + "'");
                }
            }
        }

        internal void ForceEvenResolution(bool value)
        {
            gameViewInputSettings.forceEvenSize = value;
            cameraInputSettings.forceEvenSize = value;
        }
    }

    [Serializable]
    class UTJImageInputSelector : InputSettingsSelector
    {
        [SerializeField] public CameraInputSettings cameraInputSettings = new CameraInputSettings();
        [SerializeField] public RenderTextureInputSettings renderTextureInputSettings = new RenderTextureInputSettings();
        [SerializeField] public RenderTextureSamplerSettings renderTextureSamplerSettings = new RenderTextureSamplerSettings();

        public ImageInputSettings imageInputSettings
        {
            get { return (ImageInputSettings)Selected; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                if (value is CameraInputSettings ||
                    value is RenderTextureInputSettings ||
                    value is RenderTextureSamplerSettings)
                {
                    Selected = value;
                }
                else
                {
                    throw new ArgumentException("Video input type not supported: '" + value.GetType() + "'");
                }
            }
        }
    }
}
