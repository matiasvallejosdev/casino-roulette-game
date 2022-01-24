//#define COREMEDIAENCODER_TRACE_ENABLED
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Unity.Collections;
using UnityEditor.Media;
using UnityEditor.Recorder;
using UnityEngine;
using static UnityEditor.Recorder.MovieRecorderSettings;

namespace Unity.Media
{
    internal class CoreMediaEncoderRegister : MediaEncoderRegister
    {
        internal override bool PerformsVerticalFlip => true; // the Core Media Encoder performs a VFlip in Unity Corethe encoder will flip its input vertically

        internal sealed override VideoRecorderOutputFormat[] SupportedFormats { get; set; }

        internal override unsafe MediaEncoderHandle Register(MediaEncoderManager mgr)
        {
            return mgr.Instantiate<CoreMediaEncoder>();
        }

        internal override string GetName()
        {
            return "CoreMediaEncoder";
        }

        public CoreMediaEncoderRegister()
        {
            SupportedFormats = new[] { VideoRecorderOutputFormat.MP4, VideoRecorderOutputFormat.WebM };
        }

        internal override bool SupportsResolution(MovieRecorderSettings settings, int width, int height, out string errorMessage)
        {
            if (!base.SupportsResolution(settings, width, height, out errorMessage))
            {
                return false;
            }

            errorMessage = "";
            if (settings.OutputFormat == VideoRecorderOutputFormat.MP4)
            {
                if (height % 2 != 0 || width % 2 != 0)
                {
                    errorMessage = "The MP4 format does not support odd values in resolution";
                    return false;
                }

                if (width > 4096 || height > 4096)
                {
                    Debug.LogWarning(string.Format("MP4 format might not support resolutions bigger than 4096. Current resolution: {0} x {1}.", width, height));
                }
            }

            return true;
        }

        internal override bool SupportsTransparency(MovieRecorderSettings settings, out string errorMessage)
        {
            if (!base.SupportsTransparency(settings, out errorMessage))
            {
                return false;
            }

            errorMessage = "";
            if (settings.OutputFormat == VideoRecorderOutputFormat.MP4)
            {
                errorMessage = "MP4 format does not support alpha.";
            }
            return settings.OutputFormat == VideoRecorderOutputFormat.WebM && settings.ImageInputSettings.SupportsTransparent;
        }

        internal override string GetDefaultExtension()
        {
            return "mp4";
        }
    }

    internal class CoreMediaEncoder : IMediaEncoder
    {
        RefHandle<MediaEncoder> Encoder = new RefHandle<MediaEncoder>();

        public void Construct(string path, List<IMediaEncoderAttribute> attributes)
        {
            VideoTrackAttributes vAttr = new VideoTrackAttributes();
            List<AudioTrackAttributes> aAttrs = new List<AudioTrackAttributes>();

            int nVideoTracks = 0;
            foreach (var a in attributes)
            {
                Type t = a.GetType();
                if (t == typeof(VideoTrackMediaEncoderAttribute))
                {
                    nVideoTracks++;
                    var vmAttr = (VideoTrackMediaEncoderAttribute)a;
                    vAttr = vmAttr.Value;
                }
                else if (t == typeof(AudioTrackMediaEncoderAttribute))
                {
                    var amAttr = (AudioTrackMediaEncoderAttribute)a;
                    aAttrs.Add(amAttr.Value);
                }
            }

            Debug.Assert(nVideoTracks > 0, "No video track");
            if (aAttrs.Count == 0)
            {
                Construct(path, vAttr);
            }
            else
            {
                Construct(path, vAttr, aAttrs.ToArray()[0]);
            }
        }

        public void Construct(string path, VideoTrackAttributes vAttr)
        {
            Construct(path, vAttr, new AudioTrackAttributes[0]);
        }

        public void Construct(string path, VideoTrackAttributes vAttr, AudioTrackAttributes aAttr)
        {
            Construct(path, vAttr, new[] { aAttr });
        }

        public void Construct(string path, VideoTrackAttributes vAttr, NativeArray<AudioTrackAttributes> aAttr)
        {
            Construct(path, vAttr, aAttr.ToArray());
        }

        void Construct(string path, VideoTrackAttributes vAttr, AudioTrackAttributes[] aAttr)
        {
            CoreMediaEncoderLog("Construct()");
            if (Encoder.IsCreated)
                throw new InvalidOperationException("CoreMediaEncoder already instantiated");

            Encoder.Target = new MediaEncoder(path, vAttr, aAttr);
        }

        public void Dispose()
        {
            CoreMediaEncoderLog("Dispose()");
            if (Encoder.IsCreated)
            {
                Encoder.Target.Dispose();
                Encoder.Dispose();
            }
        }

        public bool AddFrame(int width, int height, int rowBytes, TextureFormat format, NativeArray<byte> data)
        {
            CoreMediaEncoderLog("AddFrame(w,h,r,f,d)");
            if (Encoder.IsCreated)
                return Encoder.Target.AddFrame(width, height, rowBytes, format, data);
            return false;
        }

        public bool AddFrame(int width, int height, int rowBytes, TextureFormat format, NativeArray<byte> data, MediaTime time)
        {
            CoreMediaEncoderLog("AddFrame(w,h,r,f,d,t)");
            if (Encoder.IsCreated)
                return Encoder.Target.AddFrame(width, height, rowBytes, format, data, time);
            return false;
        }

        public bool AddFrame(Texture2D texture)
        {
            CoreMediaEncoderLog("AddFrame(tex)");
            if (Encoder.IsCreated)
                return Encoder.Target.AddFrame(texture);
            return false;
        }

        public bool AddFrame(Texture2D texture, MediaTime time)
        {
            CoreMediaEncoderLog("AddFrame(tex, time)");
            if (Encoder.IsCreated)
                return Encoder.Target.AddFrame(texture, time);
            return false;
        }

        public bool AddSamples(ushort trackIndex, NativeArray<float> interleavedSamples)
        {
            CoreMediaEncoderLog("AddSamples(t, samples)");
            if (Encoder.IsCreated)
                return Encoder.Target.AddSamples(trackIndex, interleavedSamples);
            return false;
        }

        public bool AddSamples(NativeArray<float> interleavedSamples)
        {
            CoreMediaEncoderLog("AddSamples(samples)");
            if (Encoder.IsCreated)
                return Encoder.Target.AddSamples(interleavedSamples);
            return false;
        }

        void CoreMediaEncoderLog(string log)
        {
#if COREMEDIAENCODER_TRACE_ENABLED
            Debug.Log("CoreMediaEncoder : " + log);
#endif
        }
    }
}
