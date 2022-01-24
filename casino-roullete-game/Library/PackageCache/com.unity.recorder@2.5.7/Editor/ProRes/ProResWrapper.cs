using System;
using System.Runtime.InteropServices;
using System.Text;

struct ProResWrapperInfo
{
#if UNITY_EDITOR_WIN
    public const string LibraryPath = "ProResWrapper";
#elif UNITY_EDITOR_OSX
    public const string LibraryPath = "Packages/com.unity.recorder/Plugins/macOS/AVFoundationWrapper.bundle";
#else
    public const string LibraryPath = "N/A";
#endif
}

// This is public so that the tests can use the call to test for OS support of a codec format
struct ProResWrapperHelpers
{
#if UNITY_EDITOR_OSX
    [DllImport(ProResWrapperInfo.LibraryPath, CallingConvention = CallingConvention.Cdecl)]
    public static extern bool SupportsCodecFormat(int codecType, StringBuilder errorMsg, int errorMsgLen);
#endif
}

internal struct ProResWrapper
{
    [DllImport(ProResWrapperInfo.LibraryPath)]
    public static extern IntPtr Create(string sMetadata, string sFileName, int width, int height, float fps, bool hasAudio, float fAudioSamplingRate, int codecType, bool hasTransparency, int colorDesc);
    [DllImport(ProResWrapperInfo.LibraryPath)]
    public static extern bool AddVideoFrame(IntPtr pEncoder, byte[] pixels);
    [DllImport(ProResWrapperInfo.LibraryPath)]
    public static extern bool AddAudioSamples(IntPtr pEncoder, float[] samples, int numSamples);
    [DllImport(ProResWrapperInfo.LibraryPath)]
    public static extern bool Close(IntPtr pEncoder);
}
