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
            StartCoroutine(CreateThumbnail(tex, path));
        }

        IEnumerator CreateThumbnail(Texture2D tex, string path)
        {
            int w = 200;
            int h = w * tex.height / tex.width;
            int _t = h % 4;
            h = h - _t;
            Texture2D newTex = Instantiate(tex);
            yield return new WaitForEndOfFrame();
            TextureScale.Point(newTex, w, h);
            byte[] bytes = newTex.EncodeToJPG();
            File.WriteAllBytes(path, bytes);
        }

        public void CreateThumbnailFrom()
        {
            Sprite sp = imagePanel.GetCurrentImage();
            string path = @"C:\WorkSpace\AR\TestImages\Thumbnails\thumbnail.jpg";
            StartCoroutine(CreateThumbnail(sp.texture, path));
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
