using UnityEngine;
using UnityEngine.Timeline;
using UnityEditor.Timeline;

namespace UnityEditor.Recorder.Timeline
{
    [CustomTimelineEditor(typeof(RecorderClip))]
    internal class RecorderClipTimelineEditor : ClipEditor
    {
        public override void OnClipChanged(TimelineClip clip)
        {
            // When the Timeline clip change name change the associated
            // RecordClip settings name.
            var rClip = clip.asset as RecorderClip;
            if (rClip.settings != null &&  rClip.settings.name != clip.displayName)
            {
                rClip.settings.name = clip.displayName;
                Undo.RegisterCompleteObjectUndo(rClip.settings, "Recorder Setting Changed");
            }
        }

        public override void OnCreate(TimelineClip clip, TrackAsset track, TimelineClip clonedFrom)
        {
            var rClip = clip.asset as RecorderClip;

            var rDuplicateClip = clonedFrom == null ? null : clonedFrom.asset as RecorderClip;
            if (rDuplicateClip != null &&  rDuplicateClip.settings != null)
            {
                // Clone by doing a deepcopy
                rClip.settings = UnityEngine.Object.Instantiate(rDuplicateClip.settings);
                rClip.settings.name = clip.displayName;
                AssetDatabase.AddObjectToAsset(rClip.settings, rClip);
            }
            else
            {
                // Create a new setting
                rClip.settings = RecordersInventory.CreateDefaultRecorderSettings(typeof(MovieRecorderSettings));
                rClip.settings.name = clip.displayName;
                AssetDatabase.AddObjectToAsset(rClip.settings, rClip);
                rClip.PushTimelineIntoRecorder(rClip.FindTimelineAsset());
            }
            Undo.RegisterCreatedObjectUndo(rClip.settings, "Recorded Settings Created");
            AssetDatabase.SaveAssets();
        }
    }
}
