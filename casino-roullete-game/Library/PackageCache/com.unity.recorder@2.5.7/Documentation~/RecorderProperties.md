# Recorder properties

The way to set up the properties of a Recorder is the same whether you do it for a Recorder selected in the list of the [Recorder window](RecordingRecorderWindow.md) or in the Inspector of a [Timeline Recorder Clip](RecordingTimelineTrack.md).

## Generic properties

All types of Recorders have [**Output File properties**](OutputFileProperties.md).

These specify the name, path, and other data for the files the Recorder outputs.

## Recorder-specific properties

Each type of Recorder has specific **Capture** and **Format** properties you must set to specify what you want to record, and how. For example, a Recorder that captures via a Camera needs to know which Camera to use.

- [**Animation Clip Recorder properties**](RecorderAnimation.md)

- [**Movie Recorder properties**](RecorderMovie.md)

- [**Image Sequence Recorder properties**](RecorderImage.md)

- [**GIF Animation Recorder properties**](RecorderGif.md)

- [**Audio Recorder properties**](RecorderAudio.md)

## Recorder Presets

After you set Recorder properties, you can save them as a [Preset](https://docs.unity3d.com/Manual/Presets.html): use the Preset icon ![](Images/IconPreset.png) in the Recorder properties pane.

If you've already saved the configuration you want as a Recorder Preset, you can load it instead of setting the Recorder properties.
