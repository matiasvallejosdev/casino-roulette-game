using System.Runtime.CompilerServices;
using UnityEngine;

[assembly: InternalsVisibleTo("Unity.Recorder.TestsCodebase")]
namespace ProResOut
{
    /// <summary>
    /// The list of supported ProRes codec formats.
    /// </summary>
    enum ProResCodecFormat
    {
        ProRes4444XQ,
        ProRes4444,
        ProRes422HQ,
        ProRes422,
        ProRes422LT,
        ProRes422Proxy
    }

    /// <summary>
    /// The list of available color definitions.
    /// </summary>
    enum ProResColorDefinition
    {
        // If you expose these values, you must also modify GetProResColorDefinitionFromExposedIndex().
        // They are not exposed because there is no way of disabling them in the GUI if they are available.
        //SD_Rec601_525_60Hz = 0,
        //SD_Rec601_625_50Hz = 1,
        HD_Rec709 = 2,
        //Rec2020 = 3,
        //HDR_SMPTE_ST_2084_Rec2020 = 4,
        //HDR_HLG_Rec2020 = 5
    }

    static internal class ProResPresetExtensions
    {
        /// <summary>
        /// Gets a human-readable name for a given ProRes codec format.
        /// </summary>
        /// <param name="format">The requested ProRes codec format.</param>
        /// <returns>The name of the codec format, with its four-character code.</returns>
        public static string GetDisplayName(this ProResCodecFormat format)
        {
            switch (format)
            {
                case ProResCodecFormat.ProRes4444XQ: return "Apple ProRes 4444 XQ (ap4x)";
                case ProResCodecFormat.ProRes4444: return "Apple ProRes 4444 (ap4h)";
                case ProResCodecFormat.ProRes422HQ: return "Apple ProRes 422 HQ (apch)";
                case ProResCodecFormat.ProRes422: return "Apple ProRes 422 (apcn)";
                case ProResCodecFormat.ProRes422LT: return "Apple ProRes 422 LT (apcs)";
                case ProResCodecFormat.ProRes422Proxy: return "Apple ProRes 422 Proxy (apco)";
            }
            return "N/A";
        }

        /// <summary>
        /// Gets a human-readable name for a given ProRes color definition.
        /// </summary>
        /// <param name="colorDefinition">The requested ProRes color definition value.</param>
        /// <returns>The name of the color definition.</returns>
        public static string GetDisplayName(this ProResColorDefinition colorDefinition)
        {
            switch (colorDefinition)
            {
                case ProResColorDefinition.HD_Rec709: return "HD Rec. 709";
            }
            return "N/A";
        }

        internal static string GetSuffix(this ProResCodecFormat format)
        {
            return ".mov";
        }

        internal static bool CodecFormatSupportsTransparency(ProResCodecFormat format)
        {
            return format == ProResCodecFormat.ProRes4444XQ || format == ProResCodecFormat.ProRes4444;
        }
    }
}
