using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEditor.Media;
using UnityEditor.Recorder;
using UnityEngine;
using static UnityEditor.Recorder.MovieRecorderSettings;

namespace Unity.Media
{
#pragma warning disable 660, 661  // We do not want Equals(object) nor GetHashCode()
    internal struct VersionedHandle
    {
        public int Version, Index;

        public static bool operator==(VersionedHandle left, VersionedHandle right)
        {
            return left.Version == right.Version && left.Index == right.Index;
        }

        public static bool operator!=(VersionedHandle left, VersionedHandle right)
        {
            return !(left == right);
        }
    }
#pragma warning restore 660, 661

    internal interface IMediaEncoderAttribute
    {
        string GetName();
    }

    internal struct AudioTrackMediaEncoderAttribute : IMediaEncoderAttribute
    {
        private string name;
        public AudioTrackAttributes Value;

        public AudioTrackMediaEncoderAttribute(string pname, AudioTrackAttributes aAttr)
        {
            name = pname;
            Value = aAttr;
        }

        public string GetName()
        {
            return name;
        }
    }

    internal struct VideoTrackMediaEncoderAttribute : IMediaEncoderAttribute
    {
        private string name;
        public VideoTrackAttributes Value;

        public VideoTrackMediaEncoderAttribute(string pname, VideoTrackAttributes vAttr)
        {
            name = pname;
            Value = vAttr;
        }

        public string GetName()
        {
            return name;
        }
    }

    internal struct IntAttribute : IMediaEncoderAttribute
    {
        private string name;

        public int Value;

        public IntAttribute(string pname, int val)
        {
            name = pname;
            Value = val;
        }

        public string GetName()
        {
            return name;
        }
    }

    internal struct FloatAttribute : IMediaEncoderAttribute
    {
        private string name;
        public float Value;

        public FloatAttribute(string pname, float val)
        {
            name = pname;
            Value = val;
        }

        public string GetName()
        {
            return name;
        }
    }

    internal struct StringAttribute : IMediaEncoderAttribute
    {
        private string name;
        public string Value;

        public StringAttribute(string pname, string pval)
        {
            name = pname;
            Value = pval;
        }

        public string GetName()
        {
            return name;
        }
    }

    internal struct Vector2Attribute : IMediaEncoderAttribute
    {
        private string name;
        public Vector2 Value;

        public Vector2Attribute(string pname, Vector2 size)
        {
            name = pname;
            Value = size;
        }

        public string GetName()
        {
            return name;
        }
    }

    /// <summary>
    /// A media preset for an encoder.
    /// </summary>
    [Serializable]
    struct MediaPreset
    {
        /// <summary>
        /// The name of the preset.
        /// </summary>
        public string name;
        /// <summary>
        /// The human-readable name of a preset.
        /// </summary>
        public string displayName;
        /// <summary>
        /// The additional options for a media preset.
        /// </summary>
        public string options;
        /// <summary>
        /// The suffix associated to a media preset.
        /// </summary>
        public string suffix;
    }

    internal struct MediaPresetAttribute : IMediaEncoderAttribute
    {
        private string name;
        private string label;
        public List<MediaPreset> Value;

        public MediaPresetAttribute(string pname, string plabel, List<MediaPreset> def)
        {
            name = pname;
            label = plabel;
            Value = new List<MediaPreset>();
            Value = def;
        }

        public string GetName()
        {
            return name;
        }

        public string GetLabel()
        {
            return label;
        }
    }

    /// <summary>
    /// A class for encoders to register themselves with the Recorder.
    /// </summary>
    abstract class MediaEncoderRegister
    {
        internal int PresetSelected = 0;
        internal virtual bool PerformsVerticalFlip { get; } // whether or not the encoder will flip its input vertically
        internal virtual VideoRecorderOutputFormat[] SupportedFormats { get; set; }

        internal abstract unsafe MediaEncoderHandle Register(MediaEncoderManager mgr);
        internal abstract string GetName();
        internal virtual void GetPresets(out List<string> presetNames, out List<string> presetOptions)
        {
            presetNames = new List<string>();
            presetOptions = new List<string>();
        }

        /// <summary>
        /// Gets the list of output formats that are supported by this encoder.
        /// </summary>
        /// <returns></returns>
        internal virtual ReadOnlyCollection<VideoRecorderOutputFormat> GetSupportedFormats()
        {
            return new ReadOnlyCollection<VideoRecorderOutputFormat>(SupportedFormats);
        }

        /// <summary>
        /// Gets the texture format that is required for this encoder, in the given context.
        /// The returned value might depend on support for transparency and source image bit depth, for instance.
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        internal virtual TextureFormat GetTextureFormat(MovieRecorderSettings settings)
        {
            return TextureFormat.RGBA32;
        }

        internal virtual void GetAttributes(out List<IMediaEncoderAttribute> attr)
        {
            attr = new List<IMediaEncoderAttribute>();
        }

        internal virtual bool SupportsResolution(MovieRecorderSettings settings, int width, int height, out string errorMessage)
        {
            if (width <= 0 || height <= 0)
            {
                errorMessage = string.Format("Invalid input resolution {0} x {1}.", width, height);
                return false;
            }

            errorMessage = "";
            return true;
        }

        internal virtual bool SupportsTransparency(MovieRecorderSettings settings, out string errorMessage)
        {
            errorMessage = "";
            return true;
        }

        /// <summary>
        /// Get the default extension of the encoder.
        /// </summary>
        /// <returns></returns>
        internal virtual string GetDefaultExtension()
        {
            throw new NotImplementedException();
        }
    }

    internal interface IMediaEncoder : IDisposable
    {
        void Construct(string path, VideoTrackAttributes vAttr);
        void Construct(string path, List<IMediaEncoderAttribute> attributes);
        void Construct(string path, VideoTrackAttributes vAttr, AudioTrackAttributes aAttr);
        void Construct(string path, VideoTrackAttributes vAttr, NativeArray<AudioTrackAttributes> aAttr);

        bool AddFrame(Texture2D texture);
        bool AddFrame(int width, int height, int rowBytes, TextureFormat format, NativeArray<byte> data);
        bool AddFrame(int width, int height, int rowBytes, TextureFormat format, NativeArray<byte> data, MediaTime time);
        bool AddFrame(Texture2D texture, MediaTime time);
        bool AddSamples(ushort trackIndex, NativeArray<float> interleavedSamples);
        bool AddSamples(NativeArray<float> interleavedSamples);
    }

    internal unsafe struct InternalEncoderData
    {
        public unsafe bool IsCreated => m_encoderInterface != null;
        public VersionedHandle VHandle;
        internal int m_TypeIndex;
        internal IMediaEncoder m_encoderInterface;
    }

    internal class MediaEncoderManager : IDisposable
    {
        List<InternalEncoderData> m_Encoders = new List<InternalEncoderData>();
        List<Type> m_Types = new List<Type>();

        public void Dispose()
        {
        }

        internal bool Is<T>(MediaEncoderHandle handle)
            where T : struct, IMediaEncoder
        {
            if (Exists(handle))
            {
                var index = m_Encoders[handle.m_VersionHandle.Index].m_TypeIndex;
                if (index < m_Types.Count && index >= 0)
                    return m_Types[index] == typeof(T);
            }
            return false;
        }

        public bool Exists(MediaEncoderHandle handle)
        {
            return handle.m_VersionHandle.Index >= 0 && handle.m_VersionHandle.Index < m_Encoders.Count &&
                handle.m_VersionHandle.Version == m_Encoders[handle.m_VersionHandle.Index].VHandle.Version;
        }

        public bool IsCreated(MediaEncoderHandle handle)
        {
            VersionCheck(handle.m_VersionHandle);
            return m_Encoders[handle.m_VersionHandle.Index].IsCreated;
        }

        internal unsafe MediaEncoderHandle Instantiate<T>()
            where T : class, IMediaEncoder, new()
        {
            InternalEncoderData encoder = new InternalEncoderData();
            encoder.VHandle.Index = m_Encoders.Count;
            encoder.VHandle.Version = 1;
            encoder.m_TypeIndex = -1;
            encoder.m_encoderInterface = new T();
            m_Encoders.Add(encoder);

            var versionedHandler = m_Encoders[encoder.VHandle.Index];
            var handle = new MediaEncoderHandle(versionedHandler.VHandle);

            var mediaEncoderType = typeof(T);
            for (var i = 0; i < m_Types.Count; ++i)
            {
                if (m_Types[i] == mediaEncoderType)
                {
                    encoder.m_TypeIndex = i;
                    break;
                }
            }

            if (encoder.m_TypeIndex == -1)
            {
                encoder.m_TypeIndex = m_Types.Count;
                m_Types.Add(mediaEncoderType);
            }

            return handle;
        }

        public void Destroy(MediaEncoderHandle handle)
        {
            VersionCheck(handle.m_VersionHandle);
            var encoder = m_Encoders[handle.m_VersionHandle.Index];
            encoder.m_encoderInterface.Dispose();
            encoder.m_encoderInterface = null;
            encoder.VHandle.Version++;
        }

        private void DisposeCheck(MediaEncoderHandle handle)
        {
            if (!IsCreated(handle))
                throw new ObjectDisposedException("Media Encoder handle already disposed!");
        }

        public void Construct(MediaEncoderHandle handle, string path, VideoTrackAttributes vAttr)
        {
            DisposeCheck(handle);
            m_Encoders[handle.m_VersionHandle.Index].m_encoderInterface.Construct(path, vAttr);
        }

        public void Construct(MediaEncoderHandle handle, string path, List<IMediaEncoderAttribute> attributes)
        {
            DisposeCheck(handle);
            m_Encoders[handle.m_VersionHandle.Index].m_encoderInterface.Construct(path, attributes);
        }

        public void Construct(MediaEncoderHandle handle, string path, VideoTrackAttributes vAttr, AudioTrackAttributes aAttr)
        {
            DisposeCheck(handle);
            m_Encoders[handle.m_VersionHandle.Index].m_encoderInterface.Construct(path, vAttr, aAttr);
        }

        public void Construct(MediaEncoderHandle handle, string path, VideoTrackAttributes vAttr, NativeArray<AudioTrackAttributes> aAttr)
        {
            DisposeCheck(handle);
            m_Encoders[handle.m_VersionHandle.Index].m_encoderInterface.Construct(path, vAttr, aAttr);
        }

        public bool AddFrame(MediaEncoderHandle handle, int width, int height, int rowBytes, TextureFormat format, NativeArray<byte> data)
        {
            DisposeCheck(handle);
            return m_Encoders[handle.m_VersionHandle.Index].m_encoderInterface.AddFrame(width, height, rowBytes, format, data);
        }

        public bool AddFrame(MediaEncoderHandle handle, int width, int height, int rowBytes, TextureFormat format, NativeArray<byte> data, MediaTime time)
        {
            DisposeCheck(handle);
            return m_Encoders[handle.m_VersionHandle.Index].m_encoderInterface.AddFrame(width, height, rowBytes, format, data, time);
        }

        public bool AddFrame(MediaEncoderHandle handle, Texture2D texture)
        {
            DisposeCheck(handle);
            return m_Encoders[handle.m_VersionHandle.Index].m_encoderInterface.AddFrame(texture);
        }

        public bool AddFrame(MediaEncoderHandle handle, Texture2D texture, MediaTime time)
        {
            DisposeCheck(handle);
            return m_Encoders[handle.m_VersionHandle.Index].m_encoderInterface.AddFrame(texture, time);
        }

        public bool AddSamples(MediaEncoderHandle handle, ushort trackIndex, NativeArray<float> interleavedSamples)
        {
            DisposeCheck(handle);
            return m_Encoders[handle.m_VersionHandle.Index].m_encoderInterface.AddSamples(trackIndex, interleavedSamples);
        }

        public bool AddSamples(MediaEncoderHandle handle, NativeArray<float> interleavedSamples)
        {
            DisposeCheck(handle);
            return m_Encoders[handle.m_VersionHandle.Index].m_encoderInterface.AddSamples(interleavedSamples);
        }

        internal void VersionCheck(VersionedHandle handle)
        {
            if (handle.Index < 0 || handle.Index >= m_Encoders.Count || handle.Version != m_Encoders[handle.Index].VHandle.Version)
                throw new ObjectDisposedException("Encoder is disposed or invalid");
        }
    }

    internal class MediaEncoderHandle : IEquatable<MediaEncoderHandle>
    {
        internal readonly VersionedHandle m_VersionHandle;

        internal MediaEncoderHandle()
        {
        }

        internal MediaEncoderHandle(VersionedHandle v)
        {
            m_VersionHandle = v;
        }

        public static bool operator==(MediaEncoderHandle self, MediaEncoderHandle other)
        {
            return self.m_VersionHandle == other.m_VersionHandle;
        }

        public static bool operator!=(MediaEncoderHandle self, MediaEncoderHandle other)
        {
            return self.m_VersionHandle != other.m_VersionHandle;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            return obj is MediaEncoderHandle handle && Equals(handle);
        }

        public override int GetHashCode()
        {
            return m_VersionHandle.Index;
        }

        public bool Equals(MediaEncoderHandle other)
        {
            return this == other;
        }
    }
}
