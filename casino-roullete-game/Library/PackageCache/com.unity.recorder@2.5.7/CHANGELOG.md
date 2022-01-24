# Changelog
All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [2.5.7] - 2021-09-26
### Fixed
- Fixed a synchronization error between the MovideRecorder's video container and codec.

## [2.5.5] - 2021-02-26
### Fixed
- Fixed an error that occurred when setting the build target to macOS standalone from the Editor in Windows.

## [2.5.4] - 2021-01-25
### Fixed
- Correctly update the absolute path when choosing a path with the folder picker in the Recorder Window.
- Removed a console error message displayed when the Recorder cannot find a custom Recorder icon.
- Fixed an exception that would occur when Recording with Unity 2021.2.
- Fixed a rounding error in the delta time when recording with non-integer frame rates.

## [2.5.2] - 2020-12-16
### Fixed
- Fixed an issue where changing the active camera would be recorded one frame too late.
- Fixed an exception that would occur when undoing a Timeline Recorder Clip copy-paste action while the Inspector is active.
- Made the Animation Clip Recorder respect the Start Frame/Time setting instead of always recording from the first frame.
- Fixed Game View recording to get the expected rendering resolution, regardless of the current Game View dimensions.
- Fixed missing descriptions in Scripting API documentation.

## [2.5.0-pre.1] - 2020-11-02
### Added
- Added a new sample showing how to set up a movie recording session via script.

### Changed
- Removed support for versions prior to 2019.4.

### Fixed
- Fixed some errors with paths and wildcards in the sample code.
- Fixed broken internal links and inconsistencies in the user manual.

## [2.4.0-preview.1] - 2020-10-21
### Features
- Added MonoBehaviour recording support in the Animation Recorder.

### Bugfixes
- Fixed a bug where the animation recorder settings game object bindings were not saved properly when saving as a recorder preset.
- Reset the window without having to close it when a version upgrade happens while the Recorder Window is open.
- Fixed a bug where switching from an absolute path to any other path type would create an invalid output path.
- Log a warning when multiple concurrent Movie Recorder instances are concurrent, because this is not supported.
- Fixed a visual glitch with the "Cap FPS" checkbox extending outside its GUI element.
- Forced the "Render Frame Step" values in Recorders to be larger than zero. 
- Fixed a bug causing excessive Timeline updates while changing the output file.
- Removed superfluous "CaptureAudio" option from the AudioRecorder.
- Fixed a bug that caused Copy/Pasted RecorderClips to lose the settings when entering in playmode.
- Fixed a visual glitch with very long paths when inspecting RecorderClips.
- Fixed a bug where in a Scriptable Render Pipeline, the GameView recordings would export transparency for PNG files if the camera background had transparency.
- Fixed a crash issue when starting and stopping a GIF animation recording while the Play Mode is paused.
- Fixed an issue where multiple recorders with various camera targets would produce flipped content.

## [2.3.0-preview.3] - 2020-09-23
### Features
- Added animation curve data compression setting with keyframe reduction options in the Animation Recorder.
- Added an option to set the generated animation key tangents to ClampedAuto in the Animation Recorder.
- Added ProRes encoder support in the MovieRecorder for OSX and Windows.
- Added option to specify the color space of the recorded images as either sRGB or Linear sRGB (unclamped).
- UX improvements

### Bugfixes
- Fixed a bug that in the texture sampler recorder for animated camera fov and animated physical properties 
- Fixed a bug that caused the recorder to hang when docked next to the GameView.
- Fixed a bug that caused the recorder to hang if starting recording while already in playmode
- Fixed a bug where the AudioRecorder would not close the handle to the recorded file.
- Fixed a bug where the recorder could output multiple files in a single session, when only one was expected.
- Fixed a bug where one could not set a framerate larger than 120FPS
- Fixed a bug where the AnimationRecorder would not flush the data to disk when done recording.
- Fixed a bug where the RecorderWindow would get corrupted if some custom RecorderSettings code would not compile.
- Fixed a bug where the RecorderWindow would generate errors after a failed QuickRecord session.
- Fixed a bug where file creation could fail because of certain invalid characters in the file name.
- Fixed a bug where scripted Recorder sessions could generate errors if frames were recorded before the session was prepared.

### Known issues
- When the animationRecorder starts at frame zero and the timelineWindow inspects the timeline, multiple clip files are generated (some empty). To Fix, please update to the latest version of the Timeline package.

## [2.2.0-preview.4] - 2020-04-08
### Public API and bugfixes
- Added public api's to allow loading previously saved recorder lists.
- Fixed bug where the first few frames after going in play-mode were not recorded. 
- Fixed a bug where the texture sampling recorder that did not correctly support the physical cameras. 
- Fixed an issue when starting a record session from scripts.
- Fixed movie recoder's default framerate value being not set in the API. Default value is now set at 30 FPS.

## [2.1.0-preview.1] - 2019-12-17
### Public API and HDR recording
- Added public APIs to be able to implement custom recorders.
- Added the ability to enable recording HDR images (available only with HDRP)

## [2.0.3-preview.1] - 2019-08-13
### 2019.3, HDRP and build Fixes
- iOS Cloud build fails with an `exportArchive: Code signing "fccore.bundle" failed` error
- Recorder Clip with a Targeted Camera input gives black images with HDRP
- In 2019.3, HDRP is out of experimental and namespace was renamed
- Recorder settings warnings were not displayed in the console at recording start
- Massive documentation update
- Fixed "NullReferenceException" errors related to changes in GameView in 2019.3.0b1.

## [2.0.2-preview.2] - 2019-05-28
### Fixes and Linux support
- Add Gif and Legacy Recorders core library binaries for Linux
- Fix build errors related to fccore
- Fix a bug with Recorder Clip that would produce one frame movie if never displayed in the inspector
- Warning clean-up in 2018.4
- Remove ActiveCamera target when HDRP is available
- Fixed "NullReferenceException" errors related to changes in GameView in 2019.3

## [2.0.1-preview.1] - 2019-05-20
### Audio Recorder and some bug fixes
- Integrate AudioRecorder in the package
- Fixed Recorder labels that were not editable anymore
- Fixed Recorder List presets that were reloaded with an empty name
- Clean UIElements warning in 2019.2

## [2.0.0-preview.6] - 2019-03-26
### First package release of the *Unity Recorder*.
This mainly address moving from Asset Store to Package Manager. It also includes :
- Timeline dependency fix : since 2019.1 Timeline is a package. Code changes are compatible with
both 2018.3 (2018.3.4f1 and up) and 2019.1 (2019.1.0b2 and up).
- Updates to use official UIElements module as experimental API is deprecated in 2019.1.
- Warnings clean-up
- Samples fixes : documentation updates and proper asmdef to avoid issues during build
- Improved texture readback. Most speed improvements will be effective with previous versions,
 BUT at their top in 2019.1.
- Ability to capture a Light Weight Render Pipeline camera (requires Scriptable Render Pipeline > 5.3.0)

## [1.0.2] - 2018-09-07
### Custom resolutions. Multi-Scene editing. Various bug fixes.
- Ability to use custom resolution and custom aspect ratio
- Fixed GameObject reference sometimes resetting to None when in multi-scenes editing
- Fixed the Recorder Clip duplication issue
- Little improvement for errors reporting in the Recorder Window
- Fixed 360 View being too dark when in linear color space
- Fixed Flip Vertical being too dark in linear color space
- Fixed frame skipped issue when two or more recorders end at the same frame.
- Ability to change/reset the Take value for all recorders
- Added option to exit Playmode automatically when recording's stopped

## [1.0.1] - 2018-08-17
### 2018.1 support. Various UX fixes.
- Added support for 2018.1 (2018.1.9f1 and up)
- UI Fixes when reducing RecorderWindow size in 2018.2 and up
- Added icons for messages in the Recorder Status Bar
- Ability to use Arrow keys to switch between recorders
- Added visual indication for when the Recorder List has focus

## [1.0.0] - 2018-08-02
### First release of the *Unity Recorder*.
This is mainly a UX revamp of the Asset Store's Media Recorder. Main improvement is the ability to have multiple recorders in parallel.
