using System;
using UnityEngine;

namespace UnityEditor.Recorder
{
    class TextureFlipper : IDisposable
    {
        internal RenderTexture workTexture { private set; get; }
        internal bool inPlace { set; get; }

        internal TextureFlipper(bool flipInPlace = true)
        {
            inPlace = flipInPlace;
        }

        internal void Init(RenderTexture template)
        {
            UnityHelpers.Destroy(workTexture);
            workTexture = new RenderTexture(template);
            workTexture.name = "TextureFlipper_worktexture";
        }

        internal RenderTexture Flip(RenderTexture target)
        {
            RenderTexture outputRT = target;
            if (workTexture == null || workTexture.width != target.width || workTexture.height != target.height)
                Init(target);

            var sRGBWrite = GL.sRGBWrite;
            GL.sRGBWrite = PlayerSettings.colorSpace == ColorSpace.Linear;
            RenderTexture keepActive = RenderTexture.active;
            Graphics.Blit(target, workTexture, new Vector2(1.0f, -1.0f), new Vector2(0.0f, 1.0f));
            if (inPlace)
            {
                Graphics.Blit(workTexture, target);
            }
            else
            {
                // If we flip the image not in place return the work texture as an output texture.
                outputRT = workTexture;
            }
            // Case REC-62 Multiple Recorder produce flipped content
            // Important to restore the previously active RenderTexture target after using Graphics.Blit
            RenderTexture.active = keepActive;

            GL.sRGBWrite = sRGBWrite;
            return outputRT;
        }

        public void Dispose()
        {
            UnityHelpers.Destroy(workTexture);
            workTexture = null;
        }
    }
}
