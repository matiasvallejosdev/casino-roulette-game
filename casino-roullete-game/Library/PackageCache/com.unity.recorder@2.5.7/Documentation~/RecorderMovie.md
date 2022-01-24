# Movie Recorder properties

The **Movie Recorder** generates videos in the H.264 MP4, VP9 WebM, or ProRes QuickTime format. It does not support variable frame rates.

This page covers all properties specific to the Movie Recorder type.

> **Note:** To fully configure any Recorder, you must also set the general recording properties according to the recording interface you are using: the [Recorder window](RecorderWindowRecordingProperties.md) or a [Recorder Clip](RecordingTimelineTrack.md#recorder-clip-properties).

![](Images/RecorderMovie.png)

The Movie Recorder properties fall into three main categories:
* [Capture](#capture)
* [Format](#format)
* [Output File](#output-file)

## Capture

Use this section to define the source and the content of your recording.

|Property||Function|
|:---|:---|:---|
| **Source** ||Specifies the input for the recording.|
|| Game View |Records frames rendered in the Game View.<br/><br/>Selecting this option displays the [Game View capture properties](#game-view). |
|| Targeted Camera |Records frames captured by a specific camera, even if the Game View does not use that camera.<br/><br/>Selecting this option displays the [Targeted Camera capture properties](#targeted-camera).|
|| 360 View |Records a 360-degree video.<br/><br/>Selecting this option displays the [360 View capture properties](#360-view).|
|| Render Texture Asset |Records frames rendered in a Render Texture.<br/><br/>Selecting this option displays the [Render Texture Asset capture properties](#render-texture-asset).|
|| Texture Sampling |Supersamples the source camera during the capture to generate anti-aliased images in the recording.<br/><br/>Selecting this option displays the [Texture Sampling capture properties](#texture-sampling).|
| **Flip Vertical** ||When you enable this option, the Recorder flips the output image vertically.<br />This is useful to correct for systems that output video upside down.<br /><br />This option is not available when you record the Game View.|
| **Include Audio** ||Enable this option to include audio in the recording.|

### Game View
[!include[](InclCaptureOptionsGameview.md)]

### Targeted Camera
[!include[](InclCaptureOptionsTargetedCamera.md)]

### 360 View
[!include[](InclCaptureOptions360View.md)]

### Render Texture Asset
[!include[](InclCaptureOptionsRenderTextureAsset.md)]

### Texture Sampling
[!include[](InclCaptureOptionsTextureSampling.md)]

## Format

Use this section to set up the media format you need to save the recorded images in.

|Property|Function|
|:---|:---|
| **Media File Format** | The encoding format of the Recorder's output. Choose **H.264 MP4**, **VP9 WebM** or **ProRes QuickTime**. |
| **Include Alpha** | Enable this property to include the alpha channel in the recording. Disable it to only record the RGB channels.<br/><br/>This property is not available when the selected **Media File Format** or **Codec Format** doesn't support transparency, or when **Capture** is set to **Game View**. |
| **Quality** | The quality of the output movie: **Low**, **Medium**, or **High**. The lower the quality, the smaller the file size.<br/><br/>This property is only available when you set the **Media File Format** to **H.264 MP4** or **VP9 WebM**. |
| **Codec Format** | The video codec to use to encode the output file. Choose among a variety of [**Apple ProRes**](https://en.wikipedia.org/wiki/Apple_ProRes) codecs: **ProRes 4444 XQ**, **ProRes 4444**, **ProRes 422 HQ**, **ProRes 422**, **ProRes 422 LT** or **ProRes 422 Proxy**.<br/><br/>This property is only available when you set the **Media File Format** to **ProRes QuickTime**. |
| **Color Definition** | The video color space to use for the recording. Currently, the only available option is **HD Rec. 709**, which refers to HDTV standard.<br/><br/>This property is only available when you set the **Media File Format** to **ProRes QuickTime**. |

## Output File

Use this section to specify the output **Path** and **File Name** pattern to save the recorded animation clip.

> **Note:** [Output File properties](OutputFileProperties.md) work the same for all types of recorders.
