using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using IMAV.UI;

namespace IMAV
{
    public class ImageManager : MonoBehaviour
    {
        public UIImagePanel imagePanel;

        private static ImageManager mSingleton;
        public static ImageManager Singleton
        {
            get
            {
                return mSingleton;
            }
        }

        List<string> files = new List<string>();
        public List<string> Images
        {
            get { return files; }
        }

        public CaptureAndSave snapShot;

        void Awake()
        {
            if (mSingleton)
            {
                Destroy(gameObject);
            }
            else
            {
                mSingleton = this;
            }
        }

        void Start()
        {
            ResetFiles();
        }

        void ResetFiles()
        {
            files.Clear();
            //string[] strs = Directory.GetFiles(DataUtility.GetScreenShotPath());
            string[] strs = Directory.GetFiles(@"C:\WorkSpace\AR\TestImages\");
            foreach (string s in strs)
            {
                files.Add(s);
            }
        }

        public void CaptureScreen()
        {
            if (!Directory.Exists(DataUtility.GetScreenShotPath()))
            {
                Directory.CreateDirectory(DataUtility.GetScreenShotPath());
            }

            System.DateTime dt = System.DateTime.Now.ToLocalTime();
            string filename = "FurAR " + System.DateTime.Now.ToLocalTime().ToString("yyyy-M-d H:mm:ss") + ".jpg";
            string filePath = DataUtility.GetScreenShotPath() + filename;
            string thumbnailPath = DataUtility.GetScreenThumbnailPath() + filename;
            ResourceManager.Singleton._kudanTracker.takeScreenshot(filePath, thumbnailPath, PostScreenShot);
            files.Add(filePath);
        }

        void PostScreenShot(Texture2D tex, string path)
        {
            snapShot.SaveTextureToGallery(tex, ImageType.JPG);
            CreateThumbnail(tex, path, path);
        }

        IEnumerator CreateThumbnail(Texture2D tex, string path1, string path2)
        {
            int w = 180;
            int h = w * Screen.height / Screen.width;
            yield return new WaitForEndOfFrame();
            RenderTexture RT = new RenderTexture(Screen.width, Screen.height, 24);
            Camera.main.targetTexture = RT;
            Texture2D screen = new Texture2D(RT.width, RT.height, TextureFormat.RGB24, false);
            Texture2D thumbnail = new Texture2D(w, h, TextureFormat.RGB24, false);

            screen.ReadPixels(new Rect(0, 0, RT.width, RT.height), 0, 0);
            thumbnail.ReadPixels(new Rect(0, 0, w, h), 0, 0);
            byte[] bytes = screen.EncodeToJPG();
            byte[] bytes2 = thumbnail.EncodeToJPG();

            System.IO.File.WriteAllBytes(path1, bytes);
            System.IO.File.WriteAllBytes(path2, bytes);
            Camera.main.targetTexture = null;
            Destroy(RT);
            //Texture2D t = new Texture2D(180, 180, TextureFormat.ARGB32, false);
            //tex.Resize(180, 180, TextureFormat.RGB24, false);

            //byte[] bytes = tex.EncodeToJPG();
            //File.WriteAllBytes(path, bytes);
        }

        public void CreateThumbnailFrom()
        {
            Sprite sp = imagePanel.GetCurrentImage();
            if (sp != null)
                StartCoroutine(CreateThumbnail(sp.texture, @"C:\WorkSpace\AR\TestImages\ScreenShots\" + imagePanel.swipe.CurrentPage + ".jpg", @"C:\WorkSpace\AR\TestImages\Thumbnails\" + imagePanel.swipe.CurrentPage + ".jpg"));
            else
                Debug.Log("null sprite");
        }

        public void ShowScreenShot(string str)
        {
            try
            {
                string res = str;
                if (str == string.Empty)
                    imagePanel.Open(files.Count - 1);
                else
                {
                    int index = files.IndexOf(str);
                    imagePanel.Open(index);
                }
            }
            catch (System.Exception ex)
            {
                Debug.Log("show screen shot error: " + ex.Message);
            }
        }

        public void ShowScreenShot(int index)
        {
            if (index > -1 && index < files.Count)
                imagePanel.Open(index);
        }

        public string GetImagePath(int index)
        {
            if (index > -1 && index < files.Count)
                return files[index];
            else
                return string.Empty;
        }
    }
}
