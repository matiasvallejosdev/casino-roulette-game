//#define DEBUG_ANALYTICS
using System.Linq;
using System.Runtime.CompilerServices;
using System;
using System.Collections.Generic;
using System.Text;
using ProResOut;
using Unity.Media;
using UnityEditor.Recorder.Input;
using UnityEngine;
using UnityEngine.Rendering;
[assembly: InternalsVisibleTo("Unity.Recorder.Tests")]

namespace UnityEditor.Recorder
{
    static class RecorderAnalytics
    {
        // The 2 dictionaries help the analytics correlate the data between the start and end event without scattering data all over the RecorderControllers/Settings.
        static readonly Dictionary<RecorderController, string> controller2guid = new Dictionary<RecorderController, string>(); // RecorderWindow
        static readonly Dictionary<RecorderSettings, string> Settings2Guid = new Dictionary<RecorderSettings, string>(); // From the timeline

        const int maxEventsPerHour = 1000;
        const int maxNumberOfElements = 1000;

        const string vendorKey = "unity.recorder";
        const string startEventName = "recorder_session_start";
        const string completeEventName = "recorder_session_complete";
        [Serializable]
        internal struct RecorderSessionStartEvent
        {
            public string recorder_session_guid;
            public bool exit_play_mode;
            public string recording_mode;
            public string playback_mode;
            public float target_fps;
            public bool cap_fps;
            public string triggered_by;
            public string render_pipeline;

            public List<RecorderInfo> recorder_info;
            public List<AnimationRecorderInfo> animation_recorder_info;
            public List<ImageRecorderInfo> image_recorder_info;
            public List<MovieRecorderInfo> movie_recorder_info;
            public List<GifRecorderInfo> gif_recorder_info;
        }

        [Serializable]
        internal struct RecorderSessionEndEvent
        {
            public string recorder_session_guid;
            public string outcome;
            public int elapsed_ms;
            public int  frames_recorded;

            internal enum Outcome
            {
                Complete,
                UserStopped,
                Error
            }
        }


        [Serializable]
        internal struct RecorderInfo
        {
            public string type;
            public string record_guid;

            public static RecorderInfo FromRecorder(Recorder r)
            {
                return new RecorderInfo
                {
                    type = r.GetType().FullName,
                    record_guid = r.GetInstanceID().ToString(),
                };
            }
        }

        [Serializable]
        internal struct AnimationRecorderInfo
        {
            public string type;
            public string record_guid;
            public bool record_hierarchy;
            public bool  clamped_tangets;
            public string anim_compression;
            public static AnimationRecorderInfo FromRecorder(AnimationRecorder r)
            {
                return new AnimationRecorderInfo
                {
                    type = r.GetType().FullName,
                    record_guid = r.GetInstanceID().ToString(),
                    anim_compression =  r.Settings.AnimationInputSettings.SimplyCurves.ConvertToString(),
                    record_hierarchy = r.Settings.AnimationInputSettings.Recursive,
                    clamped_tangets = r.Settings.AnimationInputSettings.ClampedTangents
                };
            }
        }

        [Serializable]
        internal struct ImageRecorderInfo
        {
            public string type;
            public string record_guid;
            public string source;
            public int output_resolution_w;
            public int output_resolution_h;
            public string media_format;
            public string color_space;
            public static ImageRecorderInfo FromRecorder(ImageRecorder r)
            {
                return new ImageRecorderInfo()
                {
                    type = r.GetType().FullName,
                    record_guid = r.GetInstanceID().ToString(),
                    color_space = r.Settings.m_ColorSpace.ConvertToString(),
                    media_format = r.Settings.OutputFormat.ConvertToString(),
                    output_resolution_h = r.Settings.imageInputSettings.OutputHeight,
                    output_resolution_w = r.Settings.imageInputSettings.OutputWidth,
                    source = r.Settings.imageInputSettings.ConvertToString()
                };
            }
        }

        [Serializable]
        internal struct MovieRecorderInfo
        {
            public string type;
            public string record_guid;
            public string source;
            public int output_resolution_w;
            public int output_resolution_h;
            public string media_format;
            public bool include_audio;
            public string codec_format;
            public string quality;
            public static MovieRecorderInfo FromRecorder(MovieRecorder r)
            {
                var ret = new MovieRecorderInfo()
                {
                    type = r.GetType().FullName,
                    record_guid = r.GetInstanceID().ToString(),
                    output_resolution_h = r.Settings.ImageInputSettings.OutputHeight,
                    output_resolution_w = r.Settings.ImageInputSettings.OutputWidth,
                    source = r.Settings.ImageInputSettings.ConvertToString(),
                    include_audio = ((AudioInputSettings)(r.Settings.InputsSettings.First(x => x is AudioInputSettings))).PreserveAudio,
                };

                if (r.Settings.GetCurrentEncoder().GetType() == typeof(ProResEncoderRegister))
                {
                    ret.codec_format = ((ProResCodecFormat)r.Settings.encoderPresetSelected).ConvertToString();
                    ret.media_format = MovieRecorderSettings.VideoRecorderOutputFormat.MOV.ConvertToString();
                }
                else
                {
                    ret.media_format = r.Settings.OutputFormat.ConvertToString();
                    ret.quality = r.Settings.VideoBitRateMode.ConvertToString();
                }

                return ret;
            }
        }


        [Serializable]
        internal struct GifRecorderInfo
        {
            public string type;
            public string record_guid;
            public string source;
            public int output_resolution_w;
            public int output_resolution_h;

            public static GifRecorderInfo FromRecorder(GIFRecorder r)
            {
                return new GifRecorderInfo()
                {
                    type = r.GetType().FullName,
                    record_guid = r.GetInstanceID().ToString(),
                    output_resolution_h = r.Settings.imageInputSettings.OutputHeight,
                    output_resolution_w = r.Settings.imageInputSettings.OutputWidth,
                    source = r.Settings.imageInputSettings.ConvertToString()
                };
            }
        }

        // Used by the RecorderWindow
        public static void SendStartEvent(RecorderController controller)
        {
            if (!EditorAnalytics.enabled)
                return;

            EditorAnalytics.RegisterEventWithLimit(startEventName, maxEventsPerHour, maxNumberOfElements, vendorKey);

            var data = CreateSessionStartEvent(controller);
            EditorAnalytics.SendEventWithLimit(startEventName, data);
        }

        public static void SendStopEvent(RecorderController controller, bool error)
        {
            if (!EditorAnalytics.enabled)
                return;

            EditorAnalytics.RegisterEventWithLimit(completeEventName, maxEventsPerHour, maxNumberOfElements, vendorKey);

            var data = CreateStopEvent(controller, error);
            EditorAnalytics.SendEventWithLimit(completeEventName, data);
        }

        // Used by the Timeline
        public static void SendStartEvent(RecordingSession session)
        {
            if (!EditorAnalytics.enabled)
                return;

            EditorAnalytics.RegisterEventWithLimit(startEventName, maxEventsPerHour, maxNumberOfElements, vendorKey);
            var data = CreateSessionStartEvent(session);
            // Send the data to the database
            EditorAnalytics.SendEventWithLimit(startEventName, data);
        }

        public static void SendStopEvent(RecordingSession session, bool error, bool complete)
        {
            if (!EditorAnalytics.enabled)
                return;

            EditorAnalytics.RegisterEventWithLimit(completeEventName, maxEventsPerHour, maxNumberOfElements, vendorKey);

            var data = CreateStopEvent(session, error, complete);

            // Send the data to the database
            EditorAnalytics.SendEventWithLimit(completeEventName, data);
        }

        internal static RecorderSessionEndEvent CreateStopEvent(RecorderController controller, bool error)
        {
            if (!controller2guid.TryGetValue(controller, out var guid))
            {
                return new RecorderSessionEndEvent(); // Should never happen
            }

            controller2guid.Remove(controller);
            var session = controller.m_RecordingSessions.FirstOrDefault(x => x.settings.Enabled);

            RecorderSessionEndEvent data;

            if (session == null)
            {
                data = new RecorderSessionEndEvent {outcome = RecorderSessionEndEvent.Outcome.Error.ConvertToString()};
            }
            else
            {
                data = new RecorderSessionEndEvent
                {
                    recorder_session_guid = guid,
                    elapsed_ms = Mathf.Max(0, (int)(session.currentFrameStartTS * 1000)),
                    frames_recorded = session.recorder.RecordedFramesCount,
                    outcome = (error ? RecorderSessionEndEvent.Outcome.Error : GetOutcome(session)).ConvertToString()
                };
            }
#if DEBUG_ANALYTICS
            var json = JsonUtility.ToJson(data, prettyPrint: true);
            Debug.Log(json);
#endif

            return data;
        }

        static RecorderSessionEndEvent CreateStopEvent(RecordingSession session, bool error, bool complete)
        {
            if (!Settings2Guid.TryGetValue(session.settings, out var guid))
            {
                return new RecorderSessionEndEvent(); // Should never happen
            }

            Settings2Guid.Remove(session.settings);

            RecorderSessionEndEvent data;

            var outcome = error ? RecorderSessionEndEvent.Outcome.Error : (complete
                ? RecorderSessionEndEvent.Outcome.Complete
                : RecorderSessionEndEvent.Outcome.UserStopped);
            data = new RecorderSessionEndEvent
            {
                recorder_session_guid = guid,
                elapsed_ms = (int)((session.currentFrameStartTS) * 1000),
                frames_recorded = session.recorder.RecordedFramesCount,
                outcome = outcome.ConvertToString()
            };

#if DEBUG_ANALYTICS
            var json = JsonUtility.ToJson(data, prettyPrint: true);
            Debug.Log(json);
#endif
            return data;
        }

        static RecorderSessionEndEvent.Outcome GetOutcome(this RecordingSession session)
        {
            if (session == null)
                return RecorderSessionEndEvent.Outcome.Error;

            if (session.settings.RecordMode == RecordMode.TimeInterval && session.currentFrameStartTS < session.settings.EndTime ||
                session.settings.RecordMode == RecordMode.FrameInterval && session.frameIndex < session.settings.EndFrame)
            {
                return RecorderSessionEndEvent.Outcome.UserStopped;
            }

            return RecorderSessionEndEvent.Outcome.Complete;
        }

        static RecorderSessionStartEvent CreateSessionStartEvent(RecordingSession session)
        {
            var guid = GUID.Generate().ToString();
            Settings2Guid[session.settings] = guid;

            var data = new RecorderSessionStartEvent
            {
                recorder_session_guid = guid,
                exit_play_mode = RecorderOptions.exitPlayMode,
                target_fps = session.settings.FrameRate,
                triggered_by = "timeline",
                render_pipeline = GetCurrentRenderPipeline(),
            };

            GetSpecificRecorderInfos(
                new[] {session.recorder},
                out data.animation_recorder_info,
                out data.image_recorder_info,
                out data.movie_recorder_info,
                out data.gif_recorder_info,
                out data.recorder_info);
#if DEBUG_ANALYTICS
            var json = JsonUtility.ToJson(data, prettyPrint: true);
            Debug.Log(json);
#endif

            return data;
        }

        internal static RecorderSessionStartEvent CreateSessionStartEvent(RecorderController controller)
        {
            var guid = GUID.Generate().ToString();
            controller2guid[controller] = guid;

            var controllerSettings = controller.Settings;

            var data = new RecorderSessionStartEvent
            {
                recorder_session_guid = guid,
                exit_play_mode = RecorderOptions.exitPlayMode,
                recording_mode = controllerSettings.RecordMode.ConvertToString(),
                playback_mode = controllerSettings.FrameRatePlayback.ConvertToString(),
                target_fps = controllerSettings.FrameRate,
                cap_fps = controllerSettings.CapFrameRate,
                triggered_by = "recorder",
                render_pipeline = GetCurrentRenderPipeline(),
            };

            GetSpecificRecorderInfos(
                controller.m_RecordingSessions.Select(x => x.recorder).Where(x => x.settings.Enabled),
                out data.animation_recorder_info,
                out data.image_recorder_info,
                out data.movie_recorder_info,
                out data.gif_recorder_info,
                out data.recorder_info);

#if DEBUG_ANALYTICS
            var json = JsonUtility.ToJson(data, prettyPrint: true);
            Debug.Log(json);
#endif
            return data;
        }

        static void GetSpecificRecorderInfos(IEnumerable<Recorder> recorders, out List<AnimationRecorderInfo> anim,
            out List<ImageRecorderInfo> image, out List<MovieRecorderInfo> movie, out List<GifRecorderInfo> gif, out List<RecorderInfo> recorder)
        {
            anim = new List<AnimationRecorderInfo>();
            image = new List<ImageRecorderInfo>();
            movie = new List<MovieRecorderInfo>();
            gif = new List<GifRecorderInfo>();
            recorder = new List<RecorderInfo>();

            foreach (var reco in recorders)
            {
                switch (reco)
                {
                    case AnimationRecorder r:
                        anim.Add(AnimationRecorderInfo.FromRecorder(r));
                        break;
                    case ImageRecorder r:
                        image.Add(ImageRecorderInfo.FromRecorder(r));
                        break;
                    case MovieRecorder r:
                        movie.Add(MovieRecorderInfo.FromRecorder(r));
                        break;
                    case GIFRecorder r:
                        gif.Add(GifRecorderInfo.FromRecorder(r));
                        break;
                    default:
                        recorder.Add(RecorderInfo.FromRecorder(reco));
                        break;
                }
            }

            if (anim.Count == 0)
                anim = null;
            if (image.Count == 0)
                image = null;
            if (movie.Count == 0)
                movie = null;
            if (gif.Count == 0)
                gif = null;
            if (recorder.Count == 0)
                recorder = null;
        }

        internal static string ConvertToString<T>(this T e) where T : Enum
        {
            return Enum.GetName(typeof(T), e).ToSnakeCase();
        }

        static string ToSnakeCase(this string str)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < str.Length - 1; ++i)
            {
                var ch = str[i];
                var nCh = str[i + 1];
                if (char.IsUpper(ch) && char.IsLower(nCh))
                {
                    sb.Append("_");
                }

                sb.Append(ch.ToString().ToLower());
            }

            sb.Append(str[str.Length - 1].ToString().ToLower());

            return sb.ToString().TrimStart('_');
        }

        static string ConvertToString(this ImageInputSettings i)
        {
            switch (i)
            {
                case GameViewInputSettings _:
                    return "game_view";
                case CameraInputSettings _:
                    return "target_camera";
                case Camera360InputSettings _:
                    return "view_360";
                case RenderTextureInputSettings _:
                    return "texture_asset";
                case RenderTextureSamplerSettings _:
                    return "texture_sampling";
                default:
                    return "unknown";
            }
        }

        static string GetCurrentRenderPipeline()
        {
            return GraphicsSettings.currentRenderPipeline == null ? "legacy" : GraphicsSettings.currentRenderPipeline.GetType().FullName;
        }
    }
}
