using System;

namespace UnityEditor.Recorder.Input
{
    class RenderTextureInput : BaseRenderTextureInput
    {
        TextureFlipper m_VFlipper;

        RenderTextureInputSettings cbSettings
        {
            get { return (RenderTextureInputSettings)settings; }
        }

        protected internal override void BeginRecording(RecordingSession session)
        {
            if (cbSettings.renderTexture == null)
                throw new Exception("No Render Texture object provided as source");

            OutputHeight = cbSettings.OutputHeight;
            OutputWidth = cbSettings.OutputWidth;

            OutputRenderTexture = cbSettings.renderTexture;

            bool needToFlip = cbSettings.FlipFinalOutput; // whether or not the recorder settings have the flip box checked
            var movieRecorderSettings = session.settings as MovieRecorderSettings;
            if (movieRecorderSettings != null)
            {
                bool encoderAlreadyFlips = movieRecorderSettings.encodersRegistered[movieRecorderSettings.encoderSelected].PerformsVerticalFlip;
                needToFlip = needToFlip ? encoderAlreadyFlips : !encoderAlreadyFlips;
            }
            if (needToFlip)
                m_VFlipper = new TextureFlipper();
        }

        protected internal override void NewFrameReady(RecordingSession session)
        {
            bool needToFlip = cbSettings.FlipFinalOutput; // whether or not the recorder settings have the flip box checked
            var movieRecorderSettings = session.settings as MovieRecorderSettings;
            if (movieRecorderSettings != null)
            {
                bool encoderAlreadyFlips = movieRecorderSettings.encodersRegistered[movieRecorderSettings.encoderSelected].PerformsVerticalFlip;
                needToFlip = needToFlip ? encoderAlreadyFlips : !encoderAlreadyFlips;
            }
            if (needToFlip)
                OutputRenderTexture =  m_VFlipper.Flip(OutputRenderTexture);

            base.NewFrameReady(session);
        }
    }
}
