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
            string[] strs = Directory.GetFiles(DataUtility.GetScreenShotPath());
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
            string filePath = DataUtility.GetScreenShotPath() + "FurAR " + System.DateTime.Now.ToLocalTime().ToString("yyyy-M-d H:mm:ss") + ".jpg";
            ResourceManager.Singleton._kudanTracker.takeScreenshot(filePath, PostScreenShot);
            files.Add(filePath);
        }

        void PostScreenShot(Texture2D tex)
        {
            snapShot.SaveTextureToGallery(tex, ImageType.PNG);
        }

        public void ShowScreenShot(string str)
        {
            try
            {
                string res = str;
                if (str == string.Empty)
                    res = files[files.Count - 1];
                imagePanel.Open(str);
            }
            catch(System.Exception ex)
            {
                Debug.Log("show screen shot error: " + ex.Message);
            }
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
