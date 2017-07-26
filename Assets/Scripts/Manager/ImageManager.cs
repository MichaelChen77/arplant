using System;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
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

        Dictionary<string, Sprite> thumbnails = new Dictionary<string, Sprite>();
        public Dictionary<string, Sprite> Thumbnails
        {
            get { return thumbnails; }
        }

        public CaptureAndSave snapShot;

        string curRecordName;
        bool isVideoThumb = false;

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
            DataUtility.SetDirectory(DataUtility.GetScreenShotPath());
            DataUtility.SetDirectory(DataUtility.GetScreenThumbnailPath());
            StartCoroutine(loadFiles());
            Everyplay.ThumbnailTextureReady += OnThumbnailReady;
        }

        void OnDestroy()
        {
            Everyplay.ThumbnailTextureReady -= OnThumbnailReady;
        }

        IEnumerator loadFiles()
        {
            files.Clear();
            yield return null;
            DirectoryInfo dir = new DirectoryInfo(DataUtility.GetScreenThumbnailPath());
            DirectoryInfo[] dirs = dir.GetDirectories().OrderBy(d => d.LastWriteTime).ToArray();
            for (int i = 0; i < dirs.Length; i++)
            {
                FileInfo[] fs = dirs[i].GetFiles().OrderBy(d=>d.LastWriteTime).ToArray();
                for(int j =0; j<fs.Length; j++)
                {
                    files.Add(dirs[i].Name + "/" + fs[j].Name);
                }
            }
        }

        #region Screenshot methods
        private void OnThumbnailReady(Texture2D tex, bool portrait)
        {
            byte[] thumbbytes = tex.EncodeToJPG();
            if (isVideoThumb)
            {
                File.WriteAllBytes(DataUtility.GetScreenThumbnailPath() + tex.name, thumbbytes);
                thumbnails[tex.name] = DataUtility.CreateSprite(tex);
                isVideoThumb = false;
            }
            else
            { 
                File.WriteAllBytes(DataUtility.GetScreenShotPath() + tex.name, thumbbytes);
                SaveThumbnail(tex, tex.name);
                Destroy(tex);
            }
        }

        /// <summary>
        /// Takes a screenshot of the camera feed and any projected objects, without any UI.
        /// </summary>
        public void CaptureScreen(bool duringRecord, Action<string> run)
        {
            System.DateTime dt = System.DateTime.Now.ToLocalTime();
            string date = dt.ToString("yyyyMMdd");
            string filePath = DataUtility.GetScreenShotPath() + date + "/";
            string thumbPath = DataUtility.GetScreenThumbnailPath() + date + "/";
            DataUtility.SetDirectory(filePath);
            DataUtility.SetDirectory(thumbPath);

            string filename = "Screen " + dt.ToString("yyyyMMdd_HHmmss") + ".jpg";
            if (duringRecord)
                ScreenshotDuringRecord(Screen.width, Screen.height, date + "/" + filename);
            else
                StartCoroutine(Screenshot(date + "/" + filename, run));
        }

        void ScreenshotDuringRecord(int width, int height, string filename)
        {
            Texture2D screen = new Texture2D(width, height, TextureFormat.RGB24, false);
            screen.name = filename;
            screen.wrapMode = TextureWrapMode.Clamp;
            Everyplay.SetThumbnailTargetTexture(screen);
            Everyplay.TakeThumbnail();
        }

        IEnumerator Screenshot(string filepath, Action<string> run)
        {
            List<GameObject> uiObjects = FindGameObjectsInUILayer();

            for (int i = 0; i < uiObjects.Count; i++)
            {
                uiObjects[i].SetActive(false);
            }

            yield return new WaitForEndOfFrame();

            RenderTexture RT = new RenderTexture(Screen.width, Screen.height, 24);
            Camera.main.targetTexture = RT;
            Camera.main.Render();
            Texture2D screen = new Texture2D(RT.width, RT.height, TextureFormat.RGB24, false);
            screen.ReadPixels(new Rect(0, 0, RT.width, RT.height), 0, 0);
            byte[] bytes = screen.EncodeToJPG();
            System.IO.File.WriteAllBytes(DataUtility.GetScreenShotPath()+filepath, bytes);

            SaveThumbnail(screen, filepath);

            for (int i = 0; i < uiObjects.Count; i++)
            {
                uiObjects[i].SetActive(true);
            }
            Camera.main.targetTexture = null;
            Destroy(RT);
            Destroy(screen);

            if (run != null)
                run(filepath);
        }

        void SaveThumbnail(Texture2D screen, string filepath)
        {
            int h = DataUtility.GetRelatedHeight(Tags.ThumbnailWidth);
            Texture2D newTex = Instantiate(screen);
            TextureScale.Point(newTex, Tags.ThumbnailWidth, h);
            byte[] thumbbytes = newTex.EncodeToJPG();
            File.WriteAllBytes(DataUtility.GetScreenThumbnailPath() + filepath, thumbbytes);
            thumbnails[filepath] = DataUtility.CreateSprite(newTex);
        }

        void WalkSessions(string jsonfile, string videofile)
        {
            string[] dirs = System.IO.Directory.GetDirectories(DataUtility.GetSessionPath());
            if (dirs != null)
            {
                foreach (string s in dirs)
                {
                    string[] files = System.IO.Directory.GetFiles(s);
                    foreach (string f in files)
                    {
                        FileInfo file = new FileInfo(f);
                        if (file.Extension == ".json")
                        {
                            File.Copy(file.FullName, jsonfile);
                        }
                        else if (file.Extension == ".mp4")
                        {
                            File.Copy(file.FullName, videofile);
                        }
                    }
                }
            }
        }

        public void StopRecordVideo(Action<string> run)
        {
            string jsonName = DataUtility.GetScreenVideoPath() + curRecordName + ".json";
            string videoName = DataUtility.GetScreenVideoPath() + curRecordName + ".mp4";

            WalkSessions(jsonName, videoName);

            if (run != null)
                run(curRecordName + ".jpg");
        }

        public void StartRecordVideo()
        {
            System.DateTime dt = System.DateTime.Now.ToLocalTime();
            string date = dt.ToString("yyyyMMdd");
            string videoPath = DataUtility.GetScreenVideoPath() + date + "/";
            string thumbPath = DataUtility.GetScreenThumbnailPath() + date + "/";
            DataUtility.SetDirectory(videoPath);
            DataUtility.SetDirectory(thumbPath);

            string fileName = "Video " + dt.ToString("yyyyMMdd_HHmmss");
            curRecordName = date + "/" + fileName;

            int h = DataUtility.GetRelatedHeight(Tags.ThumbnailWidth);
            isVideoThumb = true;
            ScreenshotDuringRecord(Tags.ThumbnailWidth, h, curRecordName + ".jpg");
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
        #endregion

        public void SaveScreenShot(Texture2D tex)
        {
            snapShot.SaveTextureToGallery(tex, ImageType.JPG);
        }

        public void SaveVideo(string source, string filename)
        {
            GallerySaver.CopyToGallery(source, filename);
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

        public Sprite GetLatestThumbnail()
        {
            string str = files[files.Count - 1];
            return GetImage(str, true);
        }

        public Sprite GetImage(string str, bool isThumbnail)
        {
            if (isThumbnail)
            {
                if (!thumbnails.ContainsKey(str) || thumbnails[str] == null)
                    thumbnails[str] = DataUtility.CreateSprite(DataUtility.GetScreenThumbnailPath() + str);
                return thumbnails[str];
            }
            else
                return DataUtility.CreateSprite(DataUtility.GetScreenShotPath() + str);
        }

        public void ShowLatestImage()
        {
            Debug.Log("image: " + files[files.Count - 1]);
            imagePanel.Open(files.Count-1);
        }
    }
}
