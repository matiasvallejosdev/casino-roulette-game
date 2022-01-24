using System;
using UnityEngine;
using UnityEngine.Profiling;

namespace UnityEditor.Recorder.Input
{
    class GameViewInput : BaseRenderTextureInput
    {
        bool m_ModifiedResolution;
        TextureFlipper m_VFlipper;
        RenderTexture m_CaptureTexture;
        RenderTexture m_TempCaptureTextureOpaque; // A temp RenderTexture for alpha conversion
        Material m_ToOpaqueMaterial = null;

        private Material ToOpaqueMaterial
        {
            get
            {
                if (m_ToOpaqueMaterial == null)
                    m_ToOpaqueMaterial = new Material(Shader.Find("Hidden/Recorder/Inputs/MakeOpaque"));
                return m_ToOpaqueMaterial;
            }
        }

        GameViewInputSettings scSettings
        {
            get { return (GameViewInputSettings)settings; }
        }

        internal void MakeFullyOpaque(Texture tex)
        {
            var rememberActive = RenderTexture.active;
            if (tex is RenderTexture)
            {
                var rt = tex as RenderTexture;
                Graphics.Blit(rt, m_TempCaptureTextureOpaque); // copy tex to rt
                Graphics.Blit(m_TempCaptureTextureOpaque, rt, ToOpaqueMaterial); // copy rt to tex with full opacity
            }
            else if (tex is Texture2D)
            {
                var tex2D = tex as Texture2D;
                Graphics.Blit(tex2D, m_TempCaptureTextureOpaque, ToOpaqueMaterial); // copy  with full opacity
                // Back to Texture2D
                RenderTexture.active = m_TempCaptureTextureOpaque;
                tex2D.ReadPixels(new Rect(0, 0, m_TempCaptureTextureOpaque.width, m_TempCaptureTextureOpaque.height), 0, 0);
                tex2D.Apply();
            }
            else
            {
                Debug.LogError($"Unexpected Texture type to render opaque.");
            }
            RenderTexture.active = rememberActive; // restore active RT
        }

        protected internal override void NewFrameReady(RecordingSession session)
        {
            Profiler.BeginSample("GameViewInput.NewFrameReady");
            ScreenCapture.CaptureScreenshotIntoRenderTexture(m_CaptureTexture);
            var movieRecorderSettings = session.settings as MovieRecorderSettings;
            bool needToFlip = scSettings.FlipFinalOutput;
            if (movieRecorderSettings != null)
            {
                bool encoderAlreadyFlips = movieRecorderSettings.encodersRegistered[movieRecorderSettings.encoderSelected].PerformsVerticalFlip;
                needToFlip &= encoderAlreadyFlips;
            }

            if (needToFlip)
                OutputRenderTexture = m_VFlipper?.Flip(m_CaptureTexture);

            // Force opaque alpha channel
            MakeFullyOpaque(OutputRenderTexture);
            Profiler.EndSample();
        }

        protected internal override void BeginRecording(RecordingSession session)
        {
            OutputWidth = scSettings.OutputWidth;
            OutputHeight = scSettings.OutputHeight;

            int w, h;
            GameViewSize.GetGameRenderSize(out w, out h);
            if (w != OutputWidth || h != OutputHeight)
            {
                var size = GameViewSize.SetCustomSize(OutputWidth, OutputHeight) ?? GameViewSize.AddSize(OutputWidth, OutputHeight);
                if (GameViewSize.modifiedResolutionCount == 0)
                    GameViewSize.BackupCurrentSize();
                else
                {
                    if (size != GameViewSize.currentSize)
                    {
                        Debug.LogError("Requesting a resolution change while a recorder's input has already requested one! Undefined behaviour.");
                    }
                }
                GameViewSize.modifiedResolutionCount++;
                m_ModifiedResolution = true;
                GameViewSize.SelectSize(size);
            }

            // Initialize the temporary texture for forcing opacity
            m_TempCaptureTextureOpaque = RenderTexture.GetTemporary(OutputWidth, OutputHeight);

#if !UNITY_2019_1_OR_NEWER
            // Before 2019.1, we capture synchronously into a Texture2D, so we don't need to create
            // a RenderTexture that is used for reading asynchronously.
            return;
#else
            m_CaptureTexture = new RenderTexture(OutputWidth, OutputHeight, 0, RenderTextureFormat.ARGB32)
            {
                wrapMode = TextureWrapMode.Repeat
            };
            m_CaptureTexture.Create();
            m_CaptureTexture.name = "GameViewInput_mCaptureTexture";

            var movieRecorderSettings = session.settings as MovieRecorderSettings;
            bool needToFlip = scSettings.FlipFinalOutput;
            if (movieRecorderSettings != null)
            {
                bool encoderAlreadyFlips = movieRecorderSettings.encodersRegistered[movieRecorderSettings.encoderSelected].PerformsVerticalFlip;
                needToFlip &= encoderAlreadyFlips;
            }

            if (needToFlip)
            {
                m_VFlipper = new TextureFlipper(false);
                m_VFlipper.Init(m_CaptureTexture);
                OutputRenderTexture = m_VFlipper.workTexture;
            }
            else
                OutputRenderTexture = m_CaptureTexture;
#endif
        }

        protected internal override void EndRecording(RecordingSession session)
        {
            base.EndRecording(session);
            RenderTexture.ReleaseTemporary(m_TempCaptureTextureOpaque);
        }

        protected internal override void FrameDone(RecordingSession session)
        {
            UnityHelpers.Destroy(ReadbackTexture);
            ReadbackTexture = null;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (m_ModifiedResolution)
                {
                    if (GameViewSize.modifiedResolutionCount > 0)
                        GameViewSize.modifiedResolutionCount--; // don't allow negative if called twice
                    if (GameViewSize.modifiedResolutionCount == 0)
                        GameViewSize.RestoreSize();
                }
            }

            m_VFlipper?.Dispose();
            m_VFlipper = null;

            base.Dispose(disposing);
        }
    }
}
