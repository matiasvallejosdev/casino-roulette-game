using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Unity.Collections;
using System.IO;
using ProResOut;
#if UNITY_EDITOR_OSX
using System.Text;
#endif
using UnityEngine;
using UnityEditor.Recorder;
using UnityEditor.Recorder.Input;
using static UnityEditor.Recorder.MovieRecorderSettings;

namespace Unity.Media
{
    /// <summary>
    /// A list of presets with a label.
    /// </summary>
    [Serializable]
    internal class LabeledMediaPresetList
    {
        public string listLabel = "N/A";
        public List<MediaPreset> _presetList = new List<MediaPreset>();

        public void Add(MediaPreset preset)
        {
            _presetList.Add(preset);
        }
    }

    [Serializable]
    internal class ProResPresets
    {
        public LabeledMediaPresetList _presetsCodecFormat = new LabeledMediaPresetList();
        public LabeledMediaPresetList _presetsColorDefinition = new LabeledMediaPresetList();
    }

    /// <summary>
    /// The ProRes encoder's representation for the recorder settings.
    /// </summary>
    [Serializable]
    internal class ProResEncoderRegister : MediaEncoderRegister
    {
        internal ProResPresets _presets;

        internal static readonly string CodecFormatLabel = "Codec Format";
        internal static readonly string ColorDefinitionLabel = "Color Definition";

        internal sealed override VideoRecorderOutputFormat[] SupportedFormats { get; set; }

        internal override bool PerformsVerticalFlip => false; // the ProRes wrappers do not perform a VFlip

        internal override TextureFormat GetTextureFormat(MovieRecorderSettings settings)
        {
            return settings.WillIncludeAlpha() ? TextureFormat.ARGB32 : TextureFormat.RGB24;
        }

        static string GetString(byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }

        public ProResEncoderRegister()
        {
            CreatePresets();
        }

        void CreatePresets()
        {
            if (_presets != null)
                return;

            _presets = new ProResPresets
            {
                _presetsCodecFormat = new LabeledMediaPresetList
                {
                    listLabel = "Codec Format", _presetList = new List<MediaPreset>
                    {
                        new MediaPreset
                        {
                            name = "ProRes4444XQ",
                            displayName = "Apple ProRes 4444 XQ (ap4x)",
                            suffix = ".mov",
                            options = ""
                        },
                        new MediaPreset
                        {
                            name = "ProRes4444",
                            displayName = "Apple ProRes 4444 (ap4h)",
                            suffix = ".mov",
                            options = ""
                        },
                        new MediaPreset
                        {
                            name = "ProRes422HQ",
                            displayName = "Apple ProRes 422 HQ (apch)",
                            suffix = ".mov",
                            options = ""
                        },
                        new MediaPreset
                        {
                            name = "ProRes422",
                            displayName = "Apple ProRes 422 (apcn)",
                            suffix = ".mov",
                            options = ""
                        },
                        new MediaPreset
                        {
                            name = "ProRes422LT",
                            displayName = "Apple ProRes 422 LT (apcs)",
                            suffix = ".mov",
                            options = ""
                        },
                        new MediaPreset
                        {
                            name = "ProRes422Proxy",
                            displayName = "Apple ProRes 422 Proxy (apco)",
                            suffix = ".mov",
                            options = ""
                        },
                    }
                },
                _presetsColorDefinition = new LabeledMediaPresetList
                {
                    listLabel = "Codec Format", _presetList = new List<MediaPreset>
                    {
                        new MediaPreset
                        {
                            name = "HD_Rec709",
                            displayName = "HD Rec. 709",
                            options = ""
                        }
                    }
                }
            };

            SupportedFormats = new[] { VideoRecorderOutputFormat.MOV };
        }

        internal override MediaEncoderHandle Register(MediaEncoderManager mgr)
        {
            return mgr.Instantiate<ProResEncoder>();
        }

        internal override string GetName()
        {
            return "ProResMediaEncoder";
        }

        string GetAssetPath(string filename)
        {
            return string.Format("Assets/{0}", filename);
        }

        internal override void GetAttributes(out List<IMediaEncoderAttribute> attr)
        {
            attr = new List<IMediaEncoderAttribute>();
            attr.Add(new MediaPresetAttribute(AttributeLabels[MovieRecorderSettingsAttributes.CodecFormat], CodecFormatLabel, _presets._presetsCodecFormat._presetList));
            attr.Add(new MediaPresetAttribute(AttributeLabels[MovieRecorderSettingsAttributes.ColorDefinition], ColorDefinitionLabel, _presets._presetsColorDefinition._presetList));
        }

        internal override void GetPresets(out List<string> presetNames, out List<string> presetOptions)
        {
            presetNames = new List<string>();
            presetOptions = new List<string>();
            CreatePresets();

            foreach (var p in _presets._presetsCodecFormat._presetList)
            {
                presetNames.Add(p.name);
                presetOptions.Add(p.options);
            }
        }

        internal override bool SupportsTransparency(MovieRecorderSettings settings, out string errorMessage)
        {
            if (!base.SupportsTransparency(settings, out errorMessage))
            {
                return false;
            }

            ProResOut.ProResCodecFormat selectedFormat = (ProResOut.ProResCodecFormat)settings.encoderPresetSelected;
            if (!ProResOut.ProResPresetExtensions.CodecFormatSupportsTransparency(selectedFormat))
            {
                errorMessage = string.Format("Codec format '{0}' does not support transparency.", ProResOut.ProResPresetExtensions.GetDisplayName(selectedFormat));
                return false;
            }

            errorMessage = "";
            return settings.ImageInputSettings.SupportsTransparent;
        }

        internal override string GetDefaultExtension()
        {
            return "mov";
        }
    }

    internal class ProResEncoderCore
    {
        private Dictionary<string, IMediaEncoderAttribute> m_Attributes =
            new Dictionary<string, IMediaEncoderAttribute>();

        private string _rawVideoFilename;

        private bool _hasAudio;
        private bool _hasAlpha;
        private IntPtr _encoderPtr = IntPtr.Zero; // a pointer to the encoder instance created by the ProRes wrapper

        public ProResEncoderCore(string path, List<IMediaEncoderAttribute> attributes)
        {
            foreach (var a in attributes)
            {
                Type t = a.GetType();
                if (t == typeof(VideoTrackMediaEncoderAttribute))
                {
                    var vmAttr = (VideoTrackMediaEncoderAttribute)a;
                    m_Attributes.Add(a.GetName(), new VideoTrackMediaEncoderAttribute(a.GetName(), vmAttr.Value));
                }
                else if (t == typeof(AudioTrackMediaEncoderAttribute))
                {
                    var amAttr = (AudioTrackMediaEncoderAttribute)a;
                    m_Attributes.Add(a.GetName(), new AudioTrackMediaEncoderAttribute(a.GetName(), amAttr.Value));
                }
                else if (t == typeof(IntAttribute))
                {
                    var intAttr = (IntAttribute)a;
                    m_Attributes.Add(a.GetName(), intAttr);
                }
            }

            m_Attributes.Add("Filename", new StringAttribute("Filename", path));
            StartEncoding();
        }

        /// <summary>
        /// Gets the matching color definition for the dropdown UI index. The return value must match the
        /// ProResCommon::ColorDescription enum in ProResCommon.h in the ProRes wrapper libraries.
        /// <summary>
        static internal ProResColorDefinition GetProResColorDefinitionFromExposedIndex(int exposedIndex)
        {
            ProResColorDefinition result = ProResColorDefinition.HD_Rec709;
            switch (exposedIndex)
            {
                case 0:
                    return ProResColorDefinition.HD_Rec709;
                default:
                    Debug.LogWarning(
                        $"Color definition value {exposedIndex} is not recognized. Falling back to {result}");
                    return result;
            }
        }

        /// <summary>
        /// Gets the matching color definition for the dropdown UI index. The return value must match the
        /// ProResCommon::ColorDescription enum in ProResCommon.h in the ProRes wrapper libraries.
        /// <summary>
        static internal int GetProResWrapperColorDefinition(ProResColorDefinition colorDefinition)
        {
            bool found = false;
            int result = 0;
            foreach (var v in ProResColorDefinition.GetValues(typeof(ProResColorDefinition)))
            {
                var currEnum = (ProResColorDefinition)v;
                if (currEnum == colorDefinition)
                {
                    result = (int)v;
                    found = true;
                    break;
                }
            }

            return found ? result : 0;
        }

        public void StartEncoding()
        {
            Vector2 imageSize = new Vector2(1920, 1080);
            float fps = 30.0f;
            float fAudioSamplingRate = 44100.0f;
            _hasAudio = false;
            _hasAlpha = false;
            ProResOut.ProResCodecFormat codecFormat = ProResOut.ProResCodecFormat.ProRes4444XQ;
            int colorDefinition = 0; // a value that matches ProResCommon::ColorDescription enum in C++ wrappers
            UnityEditor.Media.VideoTrackAttributes vAttr = new UnityEditor.Media.VideoTrackAttributes();
            UnityEditor.Media.AudioTrackAttributes aAttr = new UnityEditor.Media.AudioTrackAttributes();
            IMediaEncoderAttribute attr;

            if (m_Attributes.TryGetValue(AttributeLabels[MovieRecorderSettingsAttributes.CodecFormat], out attr))
            {
                var presetSelectedAttr = (IntAttribute)(attr);
                codecFormat = (ProResOut.ProResCodecFormat)presetSelectedAttr.Value;
            }

            if (m_Attributes.TryGetValue(AttributeLabels[MovieRecorderSettingsAttributes.ColorDefinition], out attr))
            {
                var presetSelectedAttr = (IntAttribute)(attr);
                // Map UI value to wrapper enum values for the ProRes wrapper libraries.
                // We need to do this because the final value for the wrapper call most likely does not
                // match the UI dropdown index (depends what enum values are exposed).
                var enumValue = GetProResColorDefinitionFromExposedIndex(presetSelectedAttr.Value);
                colorDefinition = GetProResWrapperColorDefinition(enumValue);
            }

            if (m_Attributes.TryGetValue("ImageSize", out attr))
            {
                var imageSizeAttribute = (Vector2Attribute)(attr);
                imageSize = imageSizeAttribute.Value;
            }

            if (m_Attributes.TryGetValue("Filename", out attr))
            {
                var stringAttribute = (StringAttribute)(attr);
                _rawVideoFilename = stringAttribute.Value;
            }

            if (m_Attributes.TryGetValue("VideoAttributes", out attr))
            {
                var vmAttr = (VideoTrackMediaEncoderAttribute)(attr);
                var vidAttr = vmAttr.Value;
                vAttr = vidAttr;
                imageSize.x = vAttr.width;
                imageSize.y = vAttr.height;
                fps = vAttr.frameRate.numerator / (float)vAttr.frameRate.denominator;
                _hasAlpha = vAttr.includeAlpha;
            }

            if (m_Attributes.TryGetValue("AudioAttributes", out attr))
            {
                var amAttr = (AudioTrackMediaEncoderAttribute)(attr);
                var audioAttr = amAttr.Value;
                fAudioSamplingRate = audioAttr.sampleRate.numerator / (float)audioAttr.sampleRate.denominator;
                aAttr = audioAttr;
                _hasAudio = true;
            }

            int nCodecFormat = (int)codecFormat;

#if UNITY_EDITOR_OSX
            // Ensure that this codec format is supported, because on macOS we depend on AVFoundation in the OS
            System.Text.StringBuilder sb = new System.Text.StringBuilder(128);
            bool supported = ProResWrapperHelpers.SupportsCodecFormat(nCodecFormat, sb, sb.Capacity);
            string sSupported = sb.ToString();

            if (!supported)
            {
                Debug.LogError(string.Format("Could not create file {0}: {1}", _rawVideoFilename, sb.ToString()));
                _encoderPtr = IntPtr.Zero;
                return;
            }
#endif

            // Prepare the file
            _encoderPtr = ProResWrapper.Create(UnityHelpers.PackageDescription, _rawVideoFilename, (int)imageSize.x, (int)imageSize.y, fps, _hasAudio, fAudioSamplingRate, nCodecFormat, _hasAlpha, colorDefinition);
            if (_encoderPtr == IntPtr.Zero)
            {
                Debug.LogError(string.Format("Could not create file {0}", _rawVideoFilename));
            }
        }

        public bool AddFrame(Texture tex)
        {
            throw new NotImplementedException();
        }

        public bool AddFrame(Texture2D tex)
        {
            if (_encoderPtr == IntPtr.Zero)
            {
                // Error will have been triggered earlier
                return false;
            }

            // Validate the pixel format
            var expectedFormat = _hasAlpha ? TextureFormat.ARGB32 : TextureFormat.RGB24;
            if (tex.format != expectedFormat)
            {
                Debug.LogError($"Unexpected pixel format {tex.format} (expected {expectedFormat})");
            }

            var pixels = tex.GetRawTextureData();
            bool success = ProResWrapper.AddVideoFrame(_encoderPtr, pixels);
            if (!success)
            {
                Debug.LogError("Failed to add video frame to ProRes encoder");
            }
            return success;
        }

        public bool AddFrame(Texture2D tex, UnityEditor.Media.MediaTime t)
        {
            throw new NotImplementedException();
        }

        public bool AddFrame(
            int width, int height, int rowBytes, TextureFormat format, NativeArray<byte> data)
        {
            if (_encoderPtr == IntPtr.Zero)
            {
                // Error will have been triggered earlier
                return false;
            }

            // Validate the pixel format
            var expectedFormat = _hasAlpha ? TextureFormat.ARGB32 : TextureFormat.RGB24;
            if (format != expectedFormat)
            {
                Debug.LogError($"Unexpected pixel format {format} (expected {expectedFormat})");
            }

            bool success = ProResWrapper.AddVideoFrame(_encoderPtr, data.ToArray());
            if (!success)
            {
                Debug.LogError("Failed to add video frame to ProRes encoder");
            }
            return success;
        }

        public bool AddFrame(
            int width, int height, int rowBytes, TextureFormat format, NativeArray<byte> data, UnityEditor.Media.MediaTime time)
        {
            throw new NotImplementedException();
        }

        public bool AddSamples(ushort trackIndex, NativeArray<float> interleavedSamples)
        {
            throw new NotImplementedException();
        }

        public bool AddSamples(NativeArray<float> interleavedSamples)
        {
            if (_encoderPtr == IntPtr.Zero)
            {
                // Error will have been triggered earlier
                return false;
            }

            bool success = false;
            // Recorder clips may send empty buffers, in which case success will still be true.
            success = ProResWrapper.AddAudioSamples(_encoderPtr, interleavedSamples.ToArray(), interleavedSamples.Count());
            if (!success)
            {
                Debug.LogError("Failed to add audio samples to ProRes encoder");
            }
            return success;
        }

        bool IsFileLocked(string path)
        {
            FileStream stream = null;

            try
            {
                stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.None);
            }
            catch (IOException)
            {
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            return false;
        }

        void Cleanup(string backupFileName)
        {
            try
            {
                File.Delete(backupFileName);
            }
            catch (IOException ex)
            {
                Debug.LogError(ex.Data);
            }
        }

        void PostProcessAudioRemuxing(string videoPath, string audioFileName)
        {
        }

        public void StopEncoding()
        {
        }

        void Log(string log)
        {
#if PRORESMEDIAENCODER_TRACE_ENABLED
            Debug.Log("ProRes : " + log);
#endif
        }

        internal IEnumerable<IMediaEncoderAttribute> GetAttributes()
        {
            return m_Attributes.Values.ToArray();
        }

        public void Dispose()
        {
            StopEncoding();

            if (_encoderPtr == IntPtr.Zero)
            {
                // Error will have been triggered earlier
                return;
            }
            bool success = false;
            success = ProResWrapper.Close(_encoderPtr);
            if (!success)
            {
                Debug.LogError("Failed to close ProRes encoder");
            }
        }
    }

    internal class ProResEncoder : IMediaEncoder
    {
        RefHandle<ProResEncoderCore> Encoder = new RefHandle<ProResEncoderCore>();

        public void Construct(string path, List<IMediaEncoderAttribute> attributes)
        {
            Debug.Assert(attributes != null, nameof(attributes) + " != null");
            Log("Construct()");
            if (Encoder.IsCreated)
                throw new InvalidOperationException("ProResMediaEncoder already instantiated");

            Encoder.Target = new ProResEncoderCore(path, attributes);
        }

        public void Construct(string path, UnityEditor.Media.VideoTrackAttributes vAttr)
        {
            List<IMediaEncoderAttribute> attr = new List<IMediaEncoderAttribute>();
            attr.Add(new VideoTrackMediaEncoderAttribute("VideoAttributes", vAttr));
            Construct(path, attr);
        }

        public void Construct(string path, UnityEditor.Media.VideoTrackAttributes vAttr, UnityEditor.Media.AudioTrackAttributes aAttr)
        {
            List<IMediaEncoderAttribute> attr = new List<IMediaEncoderAttribute>();
            attr.Add(new VideoTrackMediaEncoderAttribute("VideoAttributes", vAttr));
            attr.Add(new AudioTrackMediaEncoderAttribute("AudioAttributes", aAttr));
            Construct(path, attr);
        }

        public void Construct(string path, UnityEditor.Media.VideoTrackAttributes vAttr, NativeArray<UnityEditor.Media.AudioTrackAttributes> aAttr)
        {
            List<IMediaEncoderAttribute> attr = new List<IMediaEncoderAttribute>();
            attr.Add(new VideoTrackMediaEncoderAttribute("VideoAttributes", vAttr));
            foreach (var a in aAttr)
            {
                attr.Add(new AudioTrackMediaEncoderAttribute("VideoAttributes", a));
            }

            Construct(path, attr);
        }

        void Log(string log)
        {
#if PRORESMEDIAENCODER_TRACE_ENABLED
            Debug.Log("ProResEncoder : " + log);
#endif
        }

        public bool AddFrame(Texture tex)
        {
            Log("AddFrame(t)");
            if (Encoder.IsCreated)
                return Encoder.Target.AddFrame(tex);
            return false;
        }

        public bool AddFrame(Texture2D tex)
        {
            Log("AddFrame(t2d)");
            if (Encoder.IsCreated)
                return Encoder.Target.AddFrame(tex);
            return false;
        }

        public bool AddFrame(Texture2D tex, UnityEditor.Media.MediaTime t)
        {
            Log("AddFrame(t2d, t)");
            if (Encoder.IsCreated)
                return Encoder.Target.AddFrame(tex, t);
            return false;
        }

        public bool AddFrame(
            int width, int height, int rowBytes, TextureFormat format, NativeArray<byte> data)
        {
            Log("AddFrame(w,h,r,t,d)");
            if (Encoder.IsCreated)
                return Encoder.Target.AddFrame(width, height, rowBytes, format, data);
            return false;
        }

        public bool AddFrame(
            int width, int height, int rowBytes, TextureFormat format, NativeArray<byte> data, UnityEditor.Media.MediaTime time)
        {
            Log("AddFrame(w,h,r,t,d,t)");
            if (Encoder.IsCreated)
                return Encoder.Target.AddFrame(width, height, rowBytes, format, data, time);
            return false;
        }

        public bool AddSamples(ushort trackIndex, NativeArray<float> interleavedSamples)
        {
            Log("AddSample(t, samples)");
            if (Encoder.IsCreated)
                return Encoder.Target.AddSamples(trackIndex, interleavedSamples);
            return false;
        }

        public bool AddSamples(NativeArray<float> interleavedSamples)
        {
            Log("AddSample(t, samples)");

            if (Encoder.IsCreated)
                return Encoder.Target.AddSamples(interleavedSamples);
            return false;
        }

        public void Dispose()
        {
            Log("Dispose()");
            if (Encoder.IsCreated)
            {
                Encoder.Target.Dispose();
                Encoder.Dispose();
            }
        }
    }
}
