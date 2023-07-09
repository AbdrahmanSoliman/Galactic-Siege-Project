#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace ShadowBox.IconStudio
{
    [AddComponentMenu("Shadowed/Icon Studio")]
    public class IconStudio : MonoBehaviour
    {
        private Vector2 iconSize = new Vector2(2048, 2048);
        public string iconFolder = "/Shadowed/Icon Studio/Generated/";
        public Camera iconViewport;
        public Canvas backgroundCanvas;
        public Texture2D foregroundImage;

        private void OnValidate()
        {
            if (backgroundCanvas == null)
                backgroundCanvas = FindObjectOfType<Canvas>();
        }


        void OnDrawGizmos()
        {
            Vector3 top_right = Vector3.zero;
            Vector3 bottom_right = Vector3.zero;
            Vector3 bottom_left = Vector3.zero;
            Vector3 top_left = Vector3.zero;

            Vector3 cPos = new Vector3(Screen.currentResolution.width / 2, Screen.currentResolution.height / 2) / (4 * 3);
            cPos += iconViewport.transform.position;
            cPos.x -= iconSize.x / 2 / (4*3);
            cPos.y = cPos.x;
            top_right = cPos;
            bottom_left = -cPos;

            bottom_right = top_right;
            bottom_right.y = bottom_left.y;

            top_left = top_right;
            top_left.x = bottom_left.x;

            bottom_left.z = top_left.z;

            Gizmos.color = Color.yellow;

            Gizmos.DrawLine(top_right, bottom_right);
            Gizmos.DrawLine(bottom_right, bottom_left);
            Gizmos.DrawLine(bottom_left, top_left);
            Gizmos.DrawLine(top_left, top_right);
        }

        public void SetIconSize(int size)
        {
            iconSize = new Vector2(size, size);
        }

        public void SaveScreenshot(bool chromaKeyed)
        {
            if (iconFolder[0].ToString() != "/")
            {
                var y = "/" + iconFolder;
                iconFolder = y;
            }

            var x = iconFolder.Length - 1;
            if (iconFolder[x].ToString() != "/")
                iconFolder += "/";

            if (iconViewport == null)
                iconViewport = Camera.main;

            RenderTexture tTex = new RenderTexture((int)iconSize.x, (int)iconSize.y, -1, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);
            tTex.antiAliasing = 8;

            var tex = new Texture2D(tTex.width, tTex.height, TextureFormat.ARGB32, false);
            iconViewport.targetTexture = tTex;
            iconViewport.Render();
            RenderTexture.active = tTex;

            tex.ReadPixels(new Rect(0, 0, tTex.width, tTex.height), 0, 0);
            tex.Apply();

            if (chromaKeyed)
            {
                if (GraphicsSettings.currentRenderPipeline)
                {
                    if (GraphicsSettings.currentRenderPipeline.GetType().ToString().Contains("HighDefinition"))
                    {
                        iconViewport.clearFlags = CameraClearFlags.Color;
                    }
                }
                else
                    iconViewport.clearFlags = CameraClearFlags.Color;

                iconViewport.backgroundColor = Color.clear;
                backgroundCanvas.gameObject.SetActive(false);

                tex = ChromaKeyTexture(tex);
                backgroundCanvas.gameObject.SetActive(true);
            }
            else if (foregroundImage != null)
            {
                iconViewport.clearFlags = CameraClearFlags.Nothing;
                iconViewport.backgroundColor = Color.white;
                backgroundCanvas.gameObject.SetActive(true);

                if (!foregroundImage.isReadable)
                    foregroundImage = GetReadableTexture2D(foregroundImage);
                var img = Resize(foregroundImage, (int)iconSize.x, (int)iconSize.y);
                tex = StackTextures(tex, img);
            }

            if (!File.Exists(Application.dataPath + iconFolder))
                Directory.CreateDirectory(Application.dataPath + iconFolder);

            string fileName = Application.dataPath + iconFolder + DateTime.Now.ToFileTime() + ".png";

            File.WriteAllBytes(fileName, tex.EncodeToPNG());

            if (File.Exists(fileName))
                Debug.Log($"Icon saved in {fileName}");

            DestroyImmediate(tex);
            RenderTexture.active = null;
            iconViewport.targetTexture = null;
            iconViewport.Render();
            DestroyImmediate(tTex);
        }

        public void SaveSolidColorIcon()
        {
            if (iconFolder[0].ToString() != "/")
            {
                var y = "/" + iconFolder;
                iconFolder = y;
            }

            var x = iconFolder.Length - 1;
            if (iconFolder[x].ToString() != "/")
                iconFolder += "/";

            if (iconViewport == null)
                iconViewport = Camera.main;

            RenderTexture tTex = new RenderTexture((int)iconSize.x, (int)iconSize.y, -1, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);
            tTex.antiAliasing = 8;

            var tex = new Texture2D(tTex.width, tTex.height, TextureFormat.ARGB32, false);
            iconViewport.targetTexture = tTex;
            iconViewport.Render();
            RenderTexture.active = tTex;

            tex.ReadPixels(new Rect(0, 0, tTex.width, tTex.height), 0, 0);
            tex.Apply();

            iconViewport.clearFlags = CameraClearFlags.Color;

            iconViewport.backgroundColor = Color.clear;
            backgroundCanvas.gameObject.SetActive(false);

            tex = SolidColorTransparentIcon(tex);
            backgroundCanvas.gameObject.SetActive(true);

            if (!File.Exists(Application.dataPath + iconFolder))
                Directory.CreateDirectory(Application.dataPath + iconFolder);

            string fileName = Application.dataPath + iconFolder + DateTime.Now.ToFileTime() + ".png";

            File.WriteAllBytes(fileName, tex.EncodeToPNG());

            if (File.Exists(fileName))
                Debug.Log($"Icon saved in {fileName}");

            DestroyImmediate(tex);
            RenderTexture.active = null;
            iconViewport.targetTexture = null;
            iconViewport.Render();
            DestroyImmediate(tTex);
        }

        private Texture2D StackTextures(Texture2D background, Texture2D foreground)
        {
            iconViewport.clearFlags = CameraClearFlags.Nothing;
            iconViewport.backgroundColor = Color.white;
            backgroundCanvas.gameObject.SetActive(true);
            for (int x = 0; x < background.width; x++)
            {
                for (int y = 0; y < background.height; y++)
                {
                    Color bgColor = background.GetPixel(x, y);
                    Color wmColor = foreground.GetPixel(x, y);

                    Color final_color = Color.Lerp(bgColor, wmColor, wmColor.a / 1);
                    background.SetPixel(x, y, final_color);
                }
            }

            background.Apply();
            return background;
        }

        private Texture2D SolidColorTransparentIcon(Texture2D input)
        {
            backgroundCanvas.gameObject.SetActive(false);

            var output = new Texture2D(input.width, input.height, TextureFormat.ARGB32, false);
            var blackt = new Texture2D(input.width, input.height, TextureFormat.ARGB32, false);
            var greent = new Texture2D(input.width, input.height, TextureFormat.ARGB32, false);

            var screen = new Rect(0, 0, input.width, input.height);

            RenderTexture rT;
            if (GraphicsSettings.currentRenderPipeline && GraphicsSettings.currentRenderPipeline.GetType().ToString().Contains("HighDefinition"))
            {
                Debug.LogWarning("<b><color=orange>Icon Studio:</color></b> HDRP detected. This icon will not be transparent unless the Color Buffer Format is set to R16G16B16A16 under <b>Edit > Project Settings > Quality > HDRP > Rendering</b>.");
                rT = RenderTexture.GetTemporary(input.width, input.height, -24, UnityEngine.Experimental.Rendering.GraphicsFormat.R16G16B16A16_SFloat, 8, RenderTextureMemoryless.Depth);
                RenderTexture.active = rT;
                iconViewport.targetTexture = rT;
                iconViewport.clearFlags = CameraClearFlags.Color;
                iconViewport.depth = -1;
                iconViewport.depthTextureMode = DepthTextureMode.DepthNormals;
                iconViewport.backgroundColor = Color.clear;
                iconViewport.forceIntoRenderTexture = true;
                iconViewport.Render();
                output.ReadPixels(screen, 0, 0);
                iconViewport.forceIntoRenderTexture = false;
                RenderTexture.active = null;
                RenderTexture.ReleaseTemporary(rT);
            }
            else
            {
                rT = RenderTexture.GetTemporary(input.width, input.height, -24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);
                RenderTexture.active = rT;
                iconViewport.targetTexture = rT;
                iconViewport.clearFlags = CameraClearFlags.Color;

                // Capture with black background
                iconViewport.backgroundColor = Color.black;
                iconViewport.Render();
                blackt.ReadPixels(screen, 0, 0);
                blackt.Apply();

                iconViewport.backgroundColor = Color.green;
                iconViewport.Render();
                greent.ReadPixels(screen, 0, 0);
                greent.Apply();

                RenderTexture.active = null;
                RenderTexture.ReleaseTemporary(rT);

                for (int y = 0; y < input.height; ++y)
                {
                    for (int x = 0; x < input.width; ++x)
                    {
                        float a = greent.GetPixel(x, y).g - blackt.GetPixel(x, y).g;
                        a = 1.0f - a;
                        Color c;
                        if (a == 0) c = Color.clear;
                        else c = Color.white;

                        c.a = a;
                        output.SetPixel(x, y, c);
                    }
                }
            }

            output.Apply();
            iconViewport.targetTexture = null;
            iconViewport.clearFlags = CameraClearFlags.Nothing;
            return output;
        }

        private Texture2D ChromaKeyTexture(Texture2D input)
        {
            backgroundCanvas.gameObject.SetActive(false);

            var output = new Texture2D(input.width, input.height, TextureFormat.ARGB32, false);
            var blackt = new Texture2D(input.width, input.height, TextureFormat.ARGB32, false);
            var greent = new Texture2D(input.width, input.height, TextureFormat.ARGB32, false);

            var screen = new Rect(0, 0, input.width, input.height);

            RenderTexture rT;
            if (GraphicsSettings.currentRenderPipeline &&GraphicsSettings.currentRenderPipeline.GetType().ToString().Contains("HighDefinition"))
            {
                    Debug.LogWarning("<b><color=orange>Icon Studio:</color></b> HDRP detected. This icon will not be transparent unless the Color Buffer Format is set to R16G16B16A16 under <b>Edit > Project Settings > Quality > HDRP > Rendering</b>.");
                    rT = RenderTexture.GetTemporary(input.width, input.height, -24, UnityEngine.Experimental.Rendering.GraphicsFormat.R16G16B16A16_SFloat, 8, RenderTextureMemoryless.Depth);
                    RenderTexture.active = rT;
                    iconViewport.targetTexture = rT;
                    iconViewport.clearFlags = CameraClearFlags.Color;
                    iconViewport.depth = -1;
                    iconViewport.depthTextureMode = DepthTextureMode.DepthNormals;
                    iconViewport.backgroundColor = Color.clear;
                    iconViewport.forceIntoRenderTexture = true;
                    iconViewport.Render();
                    output.ReadPixels(screen, 0, 0);
                    iconViewport.forceIntoRenderTexture = false;
                    RenderTexture.active = null;
                    RenderTexture.ReleaseTemporary(rT);
            }
            else
            {
                rT = RenderTexture.GetTemporary(input.width, input.height, -24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);
                RenderTexture.active = rT;
                iconViewport.targetTexture = rT;
                iconViewport.clearFlags = CameraClearFlags.Color;

                // Capture with black background
                iconViewport.backgroundColor = Color.black;
                iconViewport.Render();
                blackt.ReadPixels(screen, 0, 0);
                blackt.Apply();

                iconViewport.backgroundColor = Color.green;
                iconViewport.Render();
                greent.ReadPixels(screen, 0, 0);
                greent.Apply();

                RenderTexture.active = null;
                RenderTexture.ReleaseTemporary(rT);

                for (int y = 0; y < input.height; ++y)
                {
                    for (int x = 0; x < input.width; ++x)
                    {
                        float a = greent.GetPixel(x, y).g - blackt.GetPixel(x, y).g;
                        a = 1.0f - a;
                        Color c;
                        if (a == 0) c = Color.clear;
                        else c = blackt.GetPixel(x, y) / a;

                        c.a = a;
                        output.SetPixel(x, y, c);
                    }
                }
            }

            output.Apply();
            iconViewport.targetTexture = null;
            iconViewport.clearFlags = CameraClearFlags.Nothing;
            return output;
        }
        private Texture2D Resize(Texture2D source, int newWidth, int newHeight)
        {
            source.filterMode = FilterMode.Point;
            RenderTexture rt = RenderTexture.GetTemporary(newWidth, newHeight);
            rt.filterMode = FilterMode.Point;
            RenderTexture.active = rt;
            Graphics.Blit(source, rt);
            Texture2D nTex = new Texture2D(newWidth, newHeight);
            nTex.ReadPixels(new Rect(0, 0, newWidth, newHeight), 0, 0);
            nTex.Apply();
            RenderTexture.active = null;
            RenderTexture.ReleaseTemporary(rt);
            return nTex;
        }

        // Credit to "Programmer"
        // https://stackoverflow.com/questions/44733841/how-to-make-texture2d-readable-via-script
        private Texture2D GetReadableTexture2D(Texture2D source)
        {
            RenderTexture renderTex = RenderTexture.GetTemporary(
                        source.width,
                        source.height,
                        0,
                        RenderTextureFormat.Default,
                        RenderTextureReadWrite.Linear);

            Graphics.Blit(source, renderTex);
            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = renderTex;
            Texture2D readableText = new Texture2D(source.width, source.height);
            readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
            readableText.Apply();
            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary(renderTex);
            return readableText;
        }
    }
}
#endif