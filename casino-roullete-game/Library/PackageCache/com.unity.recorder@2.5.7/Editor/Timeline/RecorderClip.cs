using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace UnityEditor.Recorder.Timeline
{
    /// <summary>
    /// Use this class to manage Recorder Clip Timeline integration.
    /// </summary>
    [DisplayName("Recorder Clip")]
    public class RecorderClip : PlayableAsset, ITimelineClipAsset, ISerializationCallbackReceiver
    {
        /// <summary>
        /// Indicates the Recorder Settings instance used for this Clip.
        /// </summary>
        [SerializeField]
        public RecorderSettings settings;


        readonly SceneHook m_SceneHook = new SceneHook(Guid.NewGuid().ToString());

        Type recorderType
        {
            get { return settings == null ? null : RecordersInventory.GetRecorderInfo(settings.GetType()).recorderType; }
        }

        /// <summary>
        /// Unity Recorder does not support any clip features.
        /// For more information see: https://docs.unity3d.com/2018.1/Documentation/ScriptReference/Timeline.ClipCaps.html
        /// </summary>
        public ClipCaps clipCaps
        {
            get { return ClipCaps.None; }
        }

        /// <summary>
        /// For more information see: https://docs.unity3d.com/ScriptReference/Playables.PlayableAsset.CreatePlayable.html
        /// </summary>
        /// <param name="graph">The Playable Graph.</param>
        /// <param name="owner">The GameObject containing the PlayableDirector.</param>
        /// <returns>The playable that drives the AlembicStreamPlayer.</returns>
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<RecorderPlayableBehaviour>.Create(graph);
            var behaviour = playable.GetBehaviour();
            if (recorderType != null && UnityHelpers.IsPlaying())
            {
                behaviour.session = m_SceneHook.CreateRecorderSession(settings);
            }
            return playable;
        }

        /// <summary>
        /// This is called when the Recorder Clip is being destroyed.
        /// For more information see: https://docs.unity3d.com/ScriptReference/ScriptableObject.OnDestroy.html
        /// </summary>
        public void OnDestroy()
        {
#if UNITY_EDITOR

            var path = AssetDatabase.GetAssetPath(this);
            if (string.IsNullOrEmpty(path))
                return;

            var objs = AssetDatabase.LoadAllAssetsAtPath(path);

            foreach (var obj in objs)
            {
                if (obj is RecorderClip rc && rc != this && rc.settings == settings)
                {
                    return;
                }
            }
            UnityHelpers.Destroy(settings, true);
#endif
        }

        internal TimelineAsset FindTimelineAsset()
        {
            if (!AssetDatabase.Contains(this))
                return null;

            var path = AssetDatabase.GetAssetPath(this);
            var objs = AssetDatabase.LoadAllAssetsAtPath(path);

            foreach (var obj in objs)
            {
                if (obj != null && AssetDatabase.IsMainAsset(obj))
                    return obj as TimelineAsset;
            }
            return null;
        }

        internal void PushTimelineIntoRecorder(TimelineAsset timelineAsset)
        {
            if (settings == null || timelineAsset == null)
                return;
            settings.FrameRate = timelineAsset.editorSettings.fps;
            settings.FrameRatePlayback = FrameRatePlayback.Constant;
            settings.CapFrameRate = true;
        }

        private void OnEnable()
        {
            PushTimelineIntoRecorder(FindTimelineAsset());
        }

        /// <summary>
        /// This is called before the Recorder Clip object is serialized.
        /// For more information see: https://docs.unity3d.com/ScriptReference/ISerializationCallbackReceiver.OnBeforeSerialize.html
        /// </summary>
        public void OnBeforeSerialize()
        {
            // Nothing, we need to keep these methods to avoid changing the signature of RecorderClip for the Public API
        }

        /// <summary>
        /// This is called after the Recorder Clip object has been deserialized.
        /// For more information see: https://docs.unity3d.com/ScriptReference/ISerializationCallbackReceiver.OnAfterDeserialize.html
        /// </summary>
        public void OnAfterDeserialize()
        {
            // Nothing, we need to keep these methods to avoid changing the signature of RecorderClip for the Public API
        }
    }
}
