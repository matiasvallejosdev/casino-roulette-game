using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Media;
using UnityEngine;
using static UnityEditor.Recorder.MovieRecorderSettings;

namespace UnityEditor.Recorder
{
    [CustomEditor(typeof(MovieRecorderSettings))]
    class MovieRecorderEditor : RecorderEditor
    {
        SerializedProperty m_OutputFormat;
        SerializedProperty m_OutputFormatSuffix;
        SerializedProperty m_EncodingBitRateMode;
        SerializedProperty m_CaptureAlpha;
        SerializedProperty m_ContainerFormatSelected;
        SerializedProperty m_EncoderSelected;
        SerializedProperty m_EncoderPresetSelected;
        SerializedProperty m_EncoderPresetSelectedOptions;
        SerializedProperty m_EncoderPresetSelectedName;
        SerializedProperty m_EncoderPresetSelectedSuffixes;
        SerializedProperty m_EncoderColorDefinitionSelected;
        SerializedProperty m_EncoderCustomOptions;
        SerializedProperty m_EncoderOverrideBitRate;
        SerializedProperty m_EncoderOverrideBitRateValue;

        private List<string> EncoderNames
        {
            get
            {
                if (_mEncoderNames != null)
                    return _mEncoderNames;

                _mEncoderNames = new List<string>();
                foreach (var e in RegisteredEncoders)
                {
                    _mEncoderNames.Add(e.GetName());
                }

                return _mEncoderNames;
            }
        }
        private List<string> _mEncoderNames = null;

        private MediaEncoderRegister[] RegisteredEncoders
        {
            get
            {
                if (_mRegisteredEncoders != null)
                    return _mRegisteredEncoders;

                _mRegisteredEncoders = (target as MovieRecorderSettings).encodersRegistered.ToArray();
                return _mRegisteredEncoders;
            }
        }

        private MediaEncoderRegister[] _mRegisteredEncoders = null;


        /// <summary>
        /// Gets the list of supported formats (as strings) for the registered Encoders.
        /// </summary>
        /// <returns></returns>
        private List<string> GetFormatsSupportedByRegisteredEncoders()
        {
            if (_mFormatsSupportedByRegisteredEncoders != null)
                return _mFormatsSupportedByRegisteredEncoders;
            // Look at the formats that are supported by the registered encoders
            _mFormatsSupportedByRegisteredEncoders = new List<string>();
            foreach (var encoder in RegisteredEncoders)
            {
                var currFormats = encoder.GetSupportedFormats();
                // Add to the list of formats for the GUI
                foreach (var format in currFormats)
                    _mFormatsSupportedByRegisteredEncoders.Add(format.FormatToName());
            }

            return _mFormatsSupportedByRegisteredEncoders;
        }

        private List<string> _mFormatsSupportedByRegisteredEncoders = null;

        /// Whether or not we need to show the choices of encoders. This is only enabled if there is at least one
        /// format that is supported by multiple encoders.
        bool needToDisplayEncoderDropDown
        {
            get
            {
                if (_mNeedToDisplayEncoderDropDown != null)
                    return _mNeedToDisplayEncoderDropDown.Value;

                // Determine whether or not any format is supported by more than 1 encoder.
                _mNeedToDisplayEncoderDropDown = false;
                foreach (var v in Enum.GetValues(typeof(VideoRecorderOutputFormat)))
                {
                    var value = (VideoRecorderOutputFormat)v;
                    var supportedCount = 0;
                    foreach (var encoder in RegisteredEncoders)
                    {
                        if (encoder.GetSupportedFormats().Contains(value))
                            supportedCount++;
                    }

                    if (supportedCount > 1)
                    {
                        _mNeedToDisplayEncoderDropDown = true;
                        break;
                    }
                }

                return _mNeedToDisplayEncoderDropDown.Value;
            }
        }

        private bool? _mNeedToDisplayEncoderDropDown = null;

        static class Styles
        {
            internal static readonly GUIContent VideoBitRateLabel = new GUIContent("Quality", "The quality of the output movie.");
            internal static readonly GUIContent FormatLabel = new GUIContent("Media File Format", "The file encoding format of the recorded output.");
            internal static readonly GUIContent CaptureAlphaLabel = new GUIContent("Include Alpha", "To Include the alpha channel in the recording.");
            internal static readonly GUIContent EncoderLabel = new GUIContent("Encoder", "The encoder to choose to generate the output recording");
            internal static readonly GUIContent EncoderCustomOptionsLabel = new GUIContent("Encoder options", "The options available in this encoder");
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            if (target == null)
                return;

            m_OutputFormat = serializedObject.FindProperty("outputFormat");
            m_OutputFormatSuffix = serializedObject.FindProperty("outputFormatSuffix");
            m_CaptureAlpha = serializedObject.FindProperty("captureAlpha");
            m_EncodingBitRateMode = serializedObject.FindProperty("videoBitRateMode");
            m_ContainerFormatSelected = serializedObject.FindProperty("containerFormatSelected");
            m_EncoderSelected = serializedObject.FindProperty("encoderSelected");
            m_EncoderPresetSelected = serializedObject.FindProperty("encoderPresetSelected");
            m_EncoderPresetSelectedName = serializedObject.FindProperty("encoderPresetSelectedName");
            m_EncoderPresetSelectedOptions = serializedObject.FindProperty("encoderPresetSelectedOptions");
            m_EncoderPresetSelectedSuffixes = serializedObject.FindProperty("encoderPresetSelectedSuffixes");
            m_EncoderColorDefinitionSelected = serializedObject.FindProperty("encoderColorDefinitionSelected");
            m_EncoderCustomOptions = serializedObject.FindProperty("encoderCustomOptions");
            m_EncoderOverrideBitRate = serializedObject.FindProperty("encoderOverrideBitRate");
            m_EncoderOverrideBitRateValue = serializedObject.FindProperty("encoderOverrideBitRateValue");
        }

        protected override void OnEncodingGui()
        {
        }

        protected override void FileTypeAndFormatGUI()
        {
            // Display and save the selected format
            m_ContainerFormatSelected.intValue = EditorGUILayout.Popup(Styles.FormatLabel, m_ContainerFormatSelected.intValue, GetFormatsSupportedByRegisteredEncoders().ToArray());

            // Get the encoders that support the current format
            var lsEncoderNamesSupportingSelectedFormat = new List<string>();
            int indexLastEncoderForSelectedFormat = 0; // the last encoder supporting the selected format
            // TODO: support for multiple encoders for a given format
            int i = 0;
            foreach (var encoder in RegisteredEncoders)
            {
                var currFormats = encoder.GetSupportedFormats();
                var compare = GetFormatsSupportedByRegisteredEncoders()[m_ContainerFormatSelected.intValue].NameToFormat();

                if (currFormats.Contains(compare))
                {
                    lsEncoderNamesSupportingSelectedFormat.Add(encoder.GetName());
                    indexLastEncoderForSelectedFormat = i;
                }

                i++;
            }

            if (needToDisplayEncoderDropDown)
            {
                // Display and save the encoders that support this format
                m_EncoderSelected.intValue = EditorGUILayout.Popup(Styles.EncoderLabel, m_EncoderSelected.intValue, lsEncoderNamesSupportingSelectedFormat.ToArray());
            }
            else
            {
                // Update the choice without showing a drop down. Pick the last (and only) encoder that supports this format
                m_EncoderSelected.intValue = indexLastEncoderForSelectedFormat;
            }

            // Now show all the attributes of the currently selected encoder
            List<IMediaEncoderAttribute> attr = new List<IMediaEncoderAttribute>();
            RegisteredEncoders[m_EncoderSelected.intValue].GetAttributes(out attr);

            // Display popup of codec formats for this encoder
            var movieSettings = target as MovieRecorderSettings;
            var anAttr = attr.FirstOrDefault(a => a.GetName() == AttributeLabels[MovieRecorderSettingsAttributes.CodecFormat]);
            if (anAttr != null)
            {
                MediaPresetAttribute pAttr = (MediaPresetAttribute)anAttr;

                // Present a popup for the presets (if any) of the selected encoder
                List<string> presetName = new List<string>();
                List<string> presetOptions = new List<string>();
                List<string> presetSuffixes = new List<string>();
                foreach (var p in pAttr.Value)
                {
                    presetName.Add(p.displayName);
                    presetOptions.Add(p.options);
                    presetSuffixes.Add(p.suffix);
                }

                if (presetName.Count > 0)
                {
                    ++EditorGUI.indentLevel;
                    m_EncoderPresetSelected.intValue =
                        EditorGUILayout.Popup(pAttr.GetLabel(), m_EncoderPresetSelected.intValue,
                            presetName.ToArray());
                    --EditorGUI.indentLevel;
                    m_EncoderPresetSelectedOptions.stringValue =
                        presetOptions[m_EncoderPresetSelected.intValue];
                    m_EncoderPresetSelectedName.stringValue =
                        presetName[m_EncoderPresetSelected.intValue];
                    m_EncoderPresetSelectedSuffixes.stringValue =
                        presetSuffixes[m_EncoderPresetSelected.intValue];

                    // Save the selected preset value
                    movieSettings.encoderPresetSelected = m_EncoderPresetSelected.intValue;
                    // Display Preset options in the custom field
                    var customEnabled = m_EncoderPresetSelectedName.stringValue == "Custom";
                    if (customEnabled)
                    {
                        m_EncoderCustomOptions.stringValue = EditorGUILayout.TextField(Styles.EncoderCustomOptionsLabel, m_EncoderCustomOptions.stringValue);
                        movieSettings.encoderCustomOptions = m_EncoderCustomOptions.stringValue;
                    }
                    else
                    {
                        if (presetOptions[movieSettings.encoderPresetSelected].Length != 0)
                        {
                            EditorGUI.indentLevel += 2;
                            EditorGUILayout.SelectableLabel(string.Format("Preset options: {0}", presetOptions[movieSettings.encoderPresetSelected]));
                            EditorGUI.indentLevel -= 2;
                        }
                    }
                }
            }

            // Support for color definition in encoder
            anAttr = attr.FirstOrDefault(a => a.GetName() == AttributeLabels[MovieRecorderSettingsAttributes.ColorDefinition]);
            if (anAttr != null)
            {
                MediaPresetAttribute pAttr = (MediaPresetAttribute)anAttr;

                // Present a popup for the color definitions (if any) of the selected encoder
                var presetName = new List<string>();
                foreach (var p in pAttr.Value)
                {
                    presetName.Add(p.displayName);
                }

                if (presetName.Count > 0)
                {
                    ++EditorGUI.indentLevel;
                    m_EncoderColorDefinitionSelected.intValue =
                        EditorGUILayout.Popup(pAttr.GetLabel(), m_EncoderColorDefinitionSelected.intValue,
                            presetName.ToArray());
                    --EditorGUI.indentLevel;

                    // Save the selected preset value
                    movieSettings.encoderColorDefinitionSelected = m_EncoderColorDefinitionSelected.intValue;
                }
            }

            var showAlphaCheckbox = false;
            if (RegisteredEncoders[m_EncoderSelected.intValue].GetType() == typeof(CoreMediaEncoderRegister))
            {
                var format = (VideoRecorderOutputFormat)m_ContainerFormatSelected.intValue;
                movieSettings.OutputFormat = format; // update the selected output format
                string errorMsg;
                showAlphaCheckbox = RegisteredEncoders[m_EncoderSelected.intValue].SupportsTransparency(movieSettings, out errorMsg);
            }
            else if (RegisteredEncoders[m_EncoderSelected.intValue].GetType() == typeof(ProResEncoderRegister))
            {
                string errorMsg;
                showAlphaCheckbox = RegisteredEncoders[m_EncoderSelected.intValue].SupportsTransparency(movieSettings, out errorMsg);
            }

            // Special case for the core media encoder show the encoding bit rate popup
            if (RegisteredEncoders[m_EncoderSelected.intValue].GetType() == typeof(CoreMediaEncoderRegister))
            {
                ++EditorGUI.indentLevel;
                EditorGUILayout.PropertyField(m_EncodingBitRateMode, Styles.VideoBitRateLabel);
                --EditorGUI.indentLevel;
            }

            // Show transparency checkbox
            if (showAlphaCheckbox)
            {
                ++EditorGUI.indentLevel;
                EditorGUILayout.PropertyField(m_CaptureAlpha, Styles.CaptureAlphaLabel);
                --EditorGUI.indentLevel;
            }
        }
    }
}
