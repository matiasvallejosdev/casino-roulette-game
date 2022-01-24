using System.IO;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEditor.Recorder.Input;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Recorder;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityObject = UnityEngine.Object;

namespace UnityEditor.Recorder
{
    /// <summary>
    /// An ad-hoc collection of helpers for the Recorders.
    /// </summary>
    public static class UnityHelpers
    {
        /// <summary>
        /// Allows destroying Unity.Objects.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="allowDestroyingAssets"></param>
        public static void Destroy(UnityObject obj, bool allowDestroyingAssets = false)
        {
            if (obj == null)
                return;

            if (EditorApplication.isPlaying)
                UnityObject.Destroy(obj);
            else
                UnityObject.DestroyImmediate(obj, allowDestroyingAssets);
        }

        internal static bool IsPlaying()
        {
            return EditorApplication.isPlaying;
        }

        internal static GameObject CreateRecorderGameObject(string name)
        {
            var gameObject = new GameObject(name) { tag = "EditorOnly" };
            SetGameObjectVisibility(gameObject, RecorderOptions.ShowRecorderGameObject);
            return gameObject;
        }

        internal static void SetGameObjectsVisibility(bool value)
        {
            var rcb = BindingManager.FindRecorderBindings();
            foreach (var rc in rcb)
            {
                SetGameObjectVisibility(rc.gameObject, value);
            }

            var rcs = Object.FindObjectsOfType<RecorderComponent>();
            foreach (var rc in rcs)
            {
                SetGameObjectVisibility(rc.gameObject, value);
            }
        }

        static void SetGameObjectVisibility(GameObject obj, bool visible)
        {
            if (obj != null)
            {
                obj.hideFlags = visible ? HideFlags.None : HideFlags.HideInHierarchy;

                if (!Application.isPlaying)
                {
                    try
                    {
                        EditorSceneManager.MarkSceneDirty(obj.scene);
                        EditorApplication.RepaintHierarchyWindow();
                        EditorApplication.DirtyHierarchyWindowSorting();
                    }
                    catch
                    {
                        // ignored
                    }
                }
            }
        }

        internal static bool AreAllSceneDataLoaded()
        {
            for (int i = 0; i < SceneManager.sceneCount; ++i)
            {
                Scene s = SceneManager.GetSceneAt(i);
                if (s.isLoaded == false)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// A label including the version of the package, for use in encoder metadata tags for instance.
        /// </summary>
        internal static string PackageDescription
        {
            get
            {
                return "Unity Recorder " + PackageVersion;
            }
        }

        private static ListRequest LsPackages = Client.List();
        private static string PackageVersion
        {
            get
            {
                if (m_PackageVersion.Length == 0)
                {
                    // Read the package version
                    var packageInfo = PackageManager.PackageInfo.FindForAssetPath("Packages/com.unity.recorder");
                    m_PackageVersion = packageInfo.version;
                }
                return m_PackageVersion;
            }
        }
        private static string m_PackageVersion = "";

        /// <summary>
        /// Perform a vertical flip of the supplied texture.
        /// </summary>
        /// <param name="tex">The texture to flip</param>
        /// <param name="captureAlpha">Whether or not to include transparency</param>
        /// <returns></returns>
        internal static Texture2D FlipTextureVertically(Texture2D tex, bool captureAlpha)
        {
            var FlippedTexture = new Texture2D(tex.width, tex.height, tex.format, false);
            // Texture2D -> RenderTexture for flip
            var active = RenderTexture.active; // remember for later
            var rt = RenderTexture.GetTemporary(tex.width, tex.height);
            RenderTexture.active = rt;
            // Use flip material
            var copyMaterial = new Material(Shader.Find("Hidden/Recorder/Inputs/CameraInput/Copy"));
            if (captureAlpha)
                copyMaterial.EnableKeyword("TRANSPARENCY_ON");
            copyMaterial.EnableKeyword("VERTICAL_FLIP");
            Graphics.Blit(tex, rt, copyMaterial); // Copy Texture2D to RenderTexture using the shader above
            // Flipped RenderTexture -> Texture2D
            RenderTexture.active = rt;
            FlippedTexture.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
            FlippedTexture.Apply();

            // Restore initial active RT
            RenderTexture.active = active; // restore
            RenderTexture.ReleaseTemporary(rt);

            return FlippedTexture;
        }

        /// <summary>
        /// Convert an RGBA32 texture to an RGB24 one.
        /// </summary>
        /// <param name="tex"></param>
        /// <returns></returns>
        internal static Texture2D RGBA32_to_RGB24(Texture2D tex)
        {
            if (tex.format != TextureFormat.RGBA32)
                throw new System.Exception($"Expecting RGBA32 format, received {tex.format}");

            Texture2D newTex = new Texture2D(tex.width, tex.height, TextureFormat.RGB24, false);
            newTex.SetPixels(tex.GetPixels());
            newTex.Apply();

            return newTex;
        }

        /// <summary>
        /// Load an asset from the current package's Editor/Assets folder.
        /// </summary>
        /// <param name="relativeFilePathWithExtension">The relative filename inside the Editor/Assets folder, without
        /// leading slash.</param>
        /// <param name="logError">Set this flag to true if you need to log errors when the Recorder cannot find the asset.</param>
        /// <typeparam name="T">The type of asset to load</typeparam>
        /// <returns></returns>
        internal static T LoadLocalPackageAsset<T>(string relativeFilePathWithExtension, bool logError) where T : Object
        {
            T result = default(T);
            var fullPathInProject = $"Packages/com.unity.recorder/Editor/Assets/{relativeFilePathWithExtension}";

            if (File.Exists(fullPathInProject))
                result = AssetDatabase.LoadAssetAtPath(fullPathInProject, typeof(T)) as T;
            else if (logError)
                Debug.LogError($"Local asset file {fullPathInProject} not found.");
            return result;
        }
    }
}
