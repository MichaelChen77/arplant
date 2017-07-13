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
        public UIImageGallery imageGallery;
        public UIDialog msgDialog;

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

        //void Start()
        //{
        //    ResetFiles();
        //}

        //void ResetFiles()
        //{
        //    files.Clear();
        //    string[] strs = Directory.GetFiles(DataUtility.GetScreenShotPath());
        //    foreach (string s in strs)
        //    {
        //        files.Add(s);
        //    }
        //}

        public void CaptureScreen()
        {
            System.DateTime dt = System.DateTime.Now.ToLocalTime();
            string date = dt.ToString("yyyy-MM-dd");
            string filePath = DataUtility.GetScreenShotPath() + date + "/";
            string thumbnailPath = DataUtility.GetScreenThumbnailPath() + date + "/";
            DataUtility.SetDirectory(filePath);
            DataUtility.SetDirectory(thumbnailPath);

            string filename = "FurAR " + dt.ToString("HH-mm-ss") + ".jpg";
            //ResourceManager.Singleton._kudanTracker.takeScreenshot(filePath+filename, thumbnailPath+filename, PostScreenShot);
            StartCoroutine(Screenshot(filePath + filename, thumbnailPath + filename, PostScreenShot));
        }

        List<GameObject> FindGameObjectsInUILayer()
        {
            GameObject[] goArray = FindObjectsOfType<GameObject>();

            List<GameObject> uiList = new List<GameObject>();

            for (var i = 0; i < goArray.Length; i++)
            {
                if (goArray[i].layer == 5)
                {
                    uiList.Add(goArray[i]);
                }
            }

            if (uiList.Count == 0)
            {
                return null;
            }

            return uiList;
        }

        IEnumerator Screenshot(string filePath, string thumbPath, System.Action<Texture2D, string, string> run)
        {
            List<GameObject> uiObjects = FindGameObjectsInUILayer();

            for (int i = 0; i < uiObjects.Count; i++)
            {
                uiObjects[i].SetActive(false);
            }


            yield return new WaitForEndOfFrame();
            RenderTexture RT = new RenderTexture(Screen.width, Screen.height, 24);
            Camera.main.targetTexture = RT;
            Texture2D screen = new Texture2D(RT.width, RT.height, TextureFormat.RGB24, false);
            screen.ReadPixels(new Rect(0, 0, RT.width, RT.height), 0, 0);
            byte[] bytes = screen.EncodeToJPG();
            System.IO.File.WriteAllBytes(filePath, bytes);

            for (int i = 0; i < uiObjects.Count; i++)
            {
                uiObjects[i].SetActive(true);
            }

            Camera.main.targetTexture = null;
            Destroy(RT);
            if (run != null)
                run(screen, filePath, thumbPath);
        }

        void PostScreenShot(Texture2D tex, string filepath, string thumbpath)
        {
            files.Add(filepath);
            StartCoroutine(CreateThumbnail(tex, thumbpath));
        }

        public void SaveScreenShot(Texture2D tex)
        {
            snapShot.SaveTextureToGallery(tex, ImageType.JPG);
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
            imageGallery.AddImage(path);
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
                    Debug.Log("show " + index + " ; " + str);
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
                return DataUtility.GetScreenShotPath() + files[index];
            else
                return string.Empty;
        }

        public void DeleteImage(string str)
        {
            files.Remove(str);
            string path1 = DataUtility.GetScreenShotPath() + str;
            string path2 = DataUtility.GetScreenThumbnailPath() + str;
            File.Delete(path1);
            File.Delete(path2);
            Debug.Log("Delete Image: " + str);
        }

        public void DeleteImage(int index)
        {
            if (index > -1 && index < files.Count)
            {
                DeleteImage(files[index]);
            }
        }

        public void AddImage(string str)
        {
            if (!files.Contains(str))
                files.Add(str);
        }

        public void DeleteImageFolder(string str)
        {
            DirectoryInfo thumbdir = new DirectoryInfo(DataUtility.GetScreenThumbnailPath()+str);
            if (thumbdir.Exists)
                StartCoroutine(DeleteFolder(thumbdir));
            DirectoryInfo dir = new DirectoryInfo(DataUtility.GetScreenShotPath() + str);
            if (dir.Exists)
                StartCoroutine(DeleteFolder(dir));
        }

        IEnumerator DeleteFolder(DirectoryInfo dir)
        {
            int tick = 0;
            while (dir.GetFiles().Length != 0 && tick < 5000)
            {
                yield return new WaitForEndOfFrame();
                tick++;
            }
            dir.Delete();
        }
    }
}
