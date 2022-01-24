using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using ProResOut;
using Unity.Media;
using UnityEditor.Recorder.Input;
using UnityEngine;
using UnityEngine.Serialization;

[assembly: InternalsVisibleTo("Unity.Recorder.TestsCodebase")]
namespace UnityEditor.Recorder
{
    internal static class Extensions
    {
        internal static MovieRecorderSettings.VideoRecorderOutputFormat NameToFormat(this string format)
        {
            foreach (var v in Enum.GetValues(typeof(MovieRecorderSettings.VideoRecorderOutputFormat)))
            {
                var value = (MovieRecorderSettings.VideoRecorderOutputFormat)v;
                if (value.FormatToName() == format)
                    return value;
            }
            throw new Exception($"The video recorder output format '{format}' was not found in the list of supported formats");
        }

        internal static string FormatToName(this MovieRecorderSettings.VideoRecorderOutputFormat format)
        {
            switch (format)
            {
                case MovieRecorderSettings.VideoRecorderOutputFormat.MP4:
                    return "H.264 MP4";
                case MovieRecorderSettings.VideoRecorderOutputFormat.WebM:
                    return "VP9 WebM";
                case MovieRecorderSettings.VideoRecorderOutputFormat.MOV:
                    return "ProRes QuickTime";
                default:
                    throw new ArgumentOutOfRangeException($"Unexpected video format {format}");
            }
        }
    }

    /// <summary>
    /// A class that represents the settings of a Movie Recorder.
    /// </summary>
    [RecorderSettings(typeof(MovieRecorder), "Movie", "movie_16")]
    public class MovieRecorderSettings : RecorderSettings
    {
        /// <summary>
        /// Available options for encoders to register the formats they support.
        /// </summary>
        public enum VideoRecorderOutputFormat
        {
            /// <summary>
            /// Output the recording with the H.264 codec in an MP4 container.
            /// </summary>
            MP4,
            /// <summary>
            /// Output the recording with the VP9 codec in a WebM container.
            /// </summary>
            WebM,
            /// <summary>
            /// Output the recording with the ProRes codec in a MOV container.
            /// </summary>
            MOV,
        }
        /// <summary>
        /// Indicates the output video format currently used for this Recorder.
        /// </summary>
        public VideoRecorderOutputFormat OutputFormat
        {
            get { return outputFormat; }
            set { outputFormat = value; }
        }

        [SerializeField] VideoRecorderOutputFormat outputFormat = VideoRecorderOutputFormat.MP4;

        /// <summary>
        /// Indicates the video bit rate preset currently used for this Recorder.
        /// </summary>
        public VideoBitrateMode VideoBitRateMode
        {
            get { return videoBitRateMode; }
            set { videoBitRateMode = value; }
        }

        [SerializeField] private VideoBitrateMode videoBitRateMode = VideoBitrateMode.High;

        /// <summary>
        /// Use this property to capture the alpha channel (True) or not (False) in the output.
        /// </summary>
        /// <remarks>
        /// Alpha channel will be captured only if the output image format supports it.
        /// </remarks>
        public bool CaptureAlpha
        {
            get { return captureAlpha; }
            set { captureAlpha = value; }
        }

        [SerializeField] private bool captureAlpha;

        /// <summary>
        /// The list of registered encoders.
        /// </summary>
        [SerializeReference]
        internal List<MediaEncoderRegister> encodersRegistered;

        /// <summary>
        /// Gets the preset names and options of the encoder at the specified index.
        /// </summary>
        /// <param name="indexEncoder">The index of the encoder to query</param>
        /// <param name="presetNames">The list of preset names</param>
        /// <param name="presetOptions">The list of preset options</param>
        public void GetPresetsForEncoder(int indexEncoder, out List<string> presetNames, out List<string> presetOptions)
        {
            encodersRegistered[indexEncoder].GetPresets(out presetNames, out presetOptions);
        }

        /// <summary>
        /// Gets the currently selected encoder.
        /// </summary>
        /// <returns></returns>
        internal MediaEncoderRegister GetCurrentEncoder()
        {
            return encodersRegistered[encoderSelected];
        }

        /// <summary>
        /// Returns true if and only if the settings mean to capture transparency, the input source supports it, and the
        /// codec preset also allows it.
        /// </summary>
        /// <returns></returns>
        internal bool WillIncludeAlpha()
        {
            // GameViewInput does not support transparency
            var codecFormat = (ProResOut.ProResCodecFormat)encoderPresetSelected;
            bool codecFormatSupportsTransparency = ProResPresetExtensions.CodecFormatSupportsTransparency(codecFormat);
            return CaptureAlpha && !(ImageInputSettings is GameViewInputSettings) && codecFormatSupportsTransparency;
        }

        /// <summary>
        /// Destroy the specified handle if it is already present.
        /// </summary>
        /// <param name="handle"></param>
        internal void DestroyIfExists(MediaEncoderHandle handle)
        {
            if (m_EncoderManager.Exists(handle))
                m_EncoderManager.Destroy(handle);
        }

        internal MediaEncoderManager m_EncoderManager = new MediaEncoderManager();

        /// <summary>
        /// The index of the currently selected container format in the list of formats that the registered encoders support.
        /// </summary>
        [SerializeField, UsedImplicitly] internal int containerFormatSelected = 0;

        /// <summary>
        /// The index of the currently selected encoder in the list of registered encoders.
        /// </summary>
        [SerializeField, UsedImplicitly] internal int encoderSelected = 0;

        /// <summary>
        /// The index of the preset selected for the current encoder, when the encoder supports several presets (i.e., codec formats).
        /// </summary>
        [SerializeField, UsedImplicitly] internal int encoderPresetSelected = 0;

        /// <summary>
        /// The name of the currently selected encoder preset.
        /// </summary>
        [SerializeField, UsedImplicitly] internal string encoderPresetSelectedName = "";

        /// <summary>
        /// The custom options of the currently selected encoder preset.
        /// </summary>
        [SerializeField, UsedImplicitly] internal string encoderPresetSelectedOptions = "";

        /// <summary>
        /// The extension (without leading dot) of the files created by the currently selected encoder preset.
        /// </summary>
        [SerializeField, UsedImplicitly] internal string encoderPresetSelectedSuffixes = "";

        /// <summary>
        /// The index of the color definition selected for the current encoder, when the encoder supports color definition.
        /// </summary>
        [SerializeField, UsedImplicitly] internal int encoderColorDefinitionSelected = 0;

        /// <summary>
        /// Some custom options that are specified for the currently selected encoder.
        /// </summary>
        [SerializeField, UsedImplicitly] internal string encoderCustomOptions = "";

        [SerializeField] ImageInputSelector m_ImageInputSelector = new ImageInputSelector();
        [SerializeField] AudioInputSettings m_AudioInputSettings = new AudioInputSettings();

        /// <summary>
        /// These are attributes that are exposed to the Recorder, for customization.
        /// </summary>
        internal enum MovieRecorderSettingsAttributes
        {
            CodecFormat,        // for encoders that support multiple formats (e.g. ProRes 4444XQ vs ProRes 422)
            CustomOptions,      // for encoders that can have additional options (e.g. command-line arguments)
            ColorDefinition,    // for encoders that support different color definitions
        }

        internal static readonly Dictionary<MovieRecorderSettingsAttributes, string> AttributeLabels = new Dictionary<MovieRecorderSettingsAttributes, string>()
        {
            { MovieRecorderSettingsAttributes.CodecFormat, "CodecFormat" },
            { MovieRecorderSettingsAttributes.CustomOptions, "CustomOptions" },
            { MovieRecorderSettingsAttributes.ColorDefinition, "ColorDefinition" }
        };

        /// <summary>
        /// Default constructor.
        /// </summary>
        public MovieRecorderSettings()
        {
            fileNameGenerator.FileName = "movie_" + DefaultWildcard.Take;
            FrameRate = 30;

            var iis = m_ImageInputSelector.Selected as StandardImageInputSettings;
            if (iis != null)
                iis.maxSupportedSize = ImageHeight.x2160p_4K;

            m_ImageInputSelector.ForceEvenResolution(OutputFormat == VideoRecorderOutputFormat.MP4);
            RegisterAllEncoders();
        }

        /// <summary>
        /// Find all the encoders by looking at the content of the current assemblies.
        /// </summary>
        private void RegisterAllEncoders()
        {
            encodersRegistered = new List<MediaEncoderRegister>();
            // For all assemblies find all MediaEncoderRegister
            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
            {
                var allTypes = a.GetTypes();
                var encoders = allTypes.Where(
                    type => type.IsSubclassOf(typeof(MediaEncoderRegister))
                );
#if UNITY_EDITOR_LINUX
                // Ignore ProRes
                encoders = encoders.Where(
                    type => type != typeof(ProResEncoderRegister)
                );
#endif
                foreach (var e in encoders)
                {
                    var o = Activator.CreateInstance(e);
                    var mr = o as MediaEncoderRegister;
                    encodersRegistered.Add(mr);
                }
            }
            // Enforce the alphabetical order of encoders so that CoreMediaEncoder is first and ProRes second, so that
            // their formats are processed in that order by the MovieRecorderEditor class
            encodersRegistered = encodersRegistered.OrderBy(a => a.GetName()).ToList();
        }

        /// <summary>
        /// Indicates the Image Input Settings currently used for this Recorder.
        /// </summary>
        public ImageInputSettings ImageInputSettings
        {
            get { return m_ImageInputSelector.ImageInputSettings; }
            set { m_ImageInputSelector.ImageInputSettings = value; }
        }

        /// <summary>
        /// Indicates the Audio Input Settings currently used for this Recorder.
        /// </summary>
        public AudioInputSettings AudioInputSettings
        {
            get { return m_AudioInputSettings; }
        }

        /// <inheritdoc/>
        public override IEnumerable<RecorderInputSettings> InputsSettings
        {
            get
            {
                yield return m_ImageInputSelector.Selected;
                yield return m_AudioInputSettings;
            }
        }

        /// <inheritdoc/>
        protected internal override string Extension
        {
            get
            {
                var encoders = encodersRegistered.ToArray();
                if (encoders[encoderSelected].GetType() == typeof(CoreMediaEncoderRegister))
                {
                    return OutputFormat.ToString().ToLower();
                }
                else
                {
                    return encoders[encoderSelected].GetDefaultExtension();
                }
            }
        }

        /// <inheritdoc/>
        protected internal override bool ValidityCheck(List<string> errors)
        {
            var ok = base.ValidityCheck(errors);

            if (FrameRatePlayback == FrameRatePlayback.Variable)
            {
                errors.Add("Movie recorder does not properly support Variable frame rate playback. Please consider using Constant frame rate instead");
                ok = false;
            }

            var iis = m_ImageInputSelector.Selected as ImageInputSettings;
            if (iis != null)
            {
                string errorMsg;
                if (!encodersRegistered[encoderSelected]
                    .SupportsResolution(this, iis.OutputWidth, iis.OutputHeight, out errorMsg))
                {
                    errors.Add(errorMsg);
                    ok = false;
                }
            }

            return ok;
        }

        internal override void SelfAdjustSettings()
        {
            var selectedInput = m_ImageInputSelector.Selected;
            if (selectedInput == null)
                return;

            var iis = selectedInput as StandardImageInputSettings;

            if (iis != null)
            {
                iis.maxSupportedSize = OutputFormat == VideoRecorderOutputFormat.MP4
                    ? ImageHeight.x2160p_4K
                    : ImageHeight.x4320p_8K;

                if (iis.outputImageHeight != ImageHeight.Window && iis.outputImageHeight != ImageHeight.Custom)
                {
                    if (iis.outputImageHeight > iis.maxSupportedSize)
                        iis.outputImageHeight = iis.maxSupportedSize;
                }
            }

            var cbis = selectedInput as ImageInputSettings;
            if (cbis != null)
            {
                var encoder = encodersRegistered[encoderSelected];
                if (encoder is ProResEncoderRegister p)
                {
                    var codecFormat = (ProResOut.ProResCodecFormat)encoderPresetSelected;
                    bool codecFormatSupportsTransparency = ProResPresetExtensions.CodecFormatSupportsTransparency(codecFormat);
                    cbis.RecordTransparency = CaptureAlpha && codecFormatSupportsTransparency;
                }
                else
                {
                    switch (OutputFormat)
                    {
                        case VideoRecorderOutputFormat.WebM:
                            cbis.RecordTransparency = CaptureAlpha;
                            break;
                        case VideoRecorderOutputFormat.MP4:
                        default:
                            cbis.RecordTransparency = false;
                            break;
                    }
                }
            }

            var gis = selectedInput as GameViewInputSettings;
            if (gis != null)
                gis.FlipFinalOutput = SystemInfo.supportsAsyncGPUReadback;

            m_ImageInputSelector.ForceEvenResolution(OutputFormat == VideoRecorderOutputFormat.MP4);
        }
    }
}
