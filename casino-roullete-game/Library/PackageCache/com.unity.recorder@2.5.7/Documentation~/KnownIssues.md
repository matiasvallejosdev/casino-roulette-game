# Known issues and limitations

This page lists some known issues and limitations that you might experience with the Unity Recorder. It also gives basic instructions to help you work around them.

#### Recording slowdown with concurrent Movie Recorders

**Limitation:** The Unity Editor playback process might substantially slow down if you perform concurrent recordings with multiple Movie Recorders, particularly with large image resolutions (full HD or higher).

**Workaround:** The recommended use case is to limit yourself to one Movie recording at a time. Ensure that you have only one active Movie Recorder in the Recorder window and no Movie Recorder Clips in Timeline, or vice-versa. If you need to keep concurrent recordings for some reason, you can still set up lower resolutions or try different encoders (for instance, the MP4 encoding step is much faster than the ProRes one).

#### Audio recording limited support

**Limitation:** The Recorder currently supports only the recording of samples from the Unity built-in audio engine. As such, it cannot record audio from third-party audio engines such as FMOD Studio or Wwise.

**Workaround:** For a movie, you can record the third-party audio output in WAV format through another application, reimport this recorded file into the Unity Timeline, and then use the Unity Recorder to create the final movie with audio. Alternatively, you can use any video editing software to recompose audio and video.
