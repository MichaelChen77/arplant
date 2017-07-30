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
        public DisableSelf saveFileHint;

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

        public bool ExistFile
        {
            get { return files.Count > 0; }
        }

        string curRecordName;

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
            DataUtility.SetDirectory(DataUtility.GetScreenVideoPath());
            StartCoroutine(loadFiles());
            //Everyplay.ThumbnailTextureReady += OnThumbnailReady;
            //Everyplay.WasClosed += EveryPlayClosed;
        }

        //void OnDestroy()
        //{
        //    Everyplay.ThumbnailTextureReady -= OnThumbnailReady;
        //}

        //void EveryPlayClosed()
        //{
        //    ResourceManager.Singleton.DebugString("close everyplay");
        //}

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

        #region Screenshot and Video Record methods
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

            string filename = "WhizHome " + dt.ToString("yyyyMMdd_HHmmss") + ".jpg";
            //if (duringRecord)
            //    filename = "WhizHome " + dt.ToString("yyyyMMdd_HHmmss") + "_video.jpg";
            //    //ScreenshotDuringRecord(Screen.width, Screen.height, date + "/" + filename);
            //    StartCoroutine(Screenshot(date + "/" + filename + "_video.jpg", run));
            //else
            StartCoroutine(Screenshot(date + "/" + filename, run));
        }

        //void ScreenshotDuringRecord(int width, int height, string filename)
        //{
        //    Texture2D screen = new Texture2D(width, height, TextureFormat.RGB24, false);
        //    screen.name = filename;
        //    screen.wrapMode = TextureWrapMode.Clamp;
        //    Everyplay.SetThumbnailTargetTexture(screen);
        //    Everyplay.TakeThumbnail();
        //}

        //private void OnThumbnailReady(Texture2D tex, bool portrait)
        //{
        //    byte[] thumbbytes = tex.EncodeToJPG();
        //    File.WriteAllBytes(DataUtility.GetScreenShotPath() + tex.name, thumbbytes);
        //    SaveThumbnail(tex, tex.name);
        //}

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
            System.IO.File.WriteAllBytes(DataUtility.GetScreenShotPath() + filepath, bytes);

            SaveThumbnail(screen, filepath);

            for (int i = 0; i < uiObjects.Count; i++)
            {
                uiObjects[i].SetActive(true);
            }
            Camera.main.targetTexture = null;
            Destroy(RT);

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
            Destroy(screen);
            AddThumbnail(filepath, newTex);
        }

        void AddThumbnail(string str, Texture2D tex)
        {
            thumbnails[str] = DataUtility.CreateSprite(tex);
            if (!files.Contains(str))
                files.Add(str);
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

        string ChangeJsonContent(string path, string fileName)
        {
            string str = File.ReadAllText(path);
            return str.Substring(0, str.Length - 16) + fileName + ".mp4\"]}}";
        }

        public void StartRecordVideo()
        {
            System.DateTime dt = System.DateTime.Now.ToLocalTime();
            string date = dt.ToString("yyyyMMdd");
            string videoPath = DataUtility.GetScreenVideoPath() + date + "/";
            string thumbPath = DataUtility.GetScreenThumbnailPath() + date + "/";
            string shotPath = DataUtility.GetScreenShotPath() + date + "/";
            DataUtility.SetDirectory(videoPath);
            DataUtility.SetDirectory(thumbPath);
            DataUtility.SetDirectory(shotPath);

            string fileName = "WhizHome " + dt.ToString("yyyyMMdd_HHmmss");
            curRecordName = date + "/" + fileName;

            StartCoroutine(Screenshot(curRecordName + "_video.jpg", null));
            //ScreenshotDuringRecord(Screen.width, Screen.height, curRecordName + ".jpg");
        }

        public void StopRecordVideo(Action<string> run)
        {
            string jsonName = DataUtility.GetScreenVideoPath() + curRecordName + ".json";
            string videoName = DataUtility.GetScreenVideoPath() + curRecordName + ".mp4";
            //string filename = curRecordName.Substring(curRecordName.IndexOf('/') + 1);
            //ResourceManager.Singleton.DebugString("fullname: " + filename);
            WalkSessions(jsonName, videoName);

            if (run != null)
                run(curRecordName + "_video.jpg");
        }

        public void PlayVideo(string str)
        {
            StartCoroutine(CopyVideoToCacheAndPlay(str));
        }

        IEnumerator CopyVideoToCacheAndPlay(string str)
        {
            string fileName = GetVideoFileName(str);
            string jsonFile = DataUtility.GetScreenVideoPath() + fileName + ".json";
            string videoFile = DataUtility.GetScreenVideoPath() + fileName + ".mp4";
            string[] dirs = Directory.GetDirectories(DataUtility.GetSessionPath());
            if (dirs != null && dirs.Length > 0)
            {
                string targetJson = dirs[0] + "/data.json";
                string targetVideo = dirs[0] + "/screen-0.mp4";
                ResourceManager.Singleton.DebugString("target: " + targetJson);
                File.Delete(targetJson);
                File.Delete(targetVideo);
                File.Copy(videoFile, targetVideo);
                File.Copy(jsonFile, targetJson);
            }

            yield return new WaitForEndOfFrame();
            Everyplay.PlayLastRecording();
        }

        public void SaveScreenShot(string str)
        {
            int id = str.IndexOf('/');
            if(id != -1)
            {
                string source = DataUtility.GetScreenShotPath() + str;
                string file = str.Substring(id + 1);
                ResourceManager.Singleton.DebugString("source: " + source + " ; target: " + file);
                GallerySaver.CopyToGallery(source, file);
                saveFileHint.Open("Image Saved");
            }
            else
                saveFileHint.Open("Image donot Exist");
        }

        public void SaveVideo(int index)
        {
            string str = GetPath(index);
            SaveVideo(str);
        }

        public void SaveVideo(string str)
        {
            string fullname = GetVideoFileName(str);
            if (!fullname.Equals(string.Empty))
            {
                string source = DataUtility.GetScreenVideoPath() + fullname + ".mp4";
                string file = fullname.Substring(fullname.IndexOf('/') + 1) + ".mp4";
                ResourceManager.Singleton.DebugString("source: " + source + " ; target: " + file);
                GallerySaver.CopyToGallery(source, file);
                saveFileHint.Open("Video Saved");
            }
            else
                saveFileHint.Open("Video donot Exist");
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

        public void ShowScreenShot(string str)
        {
            try
            {
                int index = files.IndexOf(str);
                if (index == -1)
                    imagePanel.Open(files.Count - 1);
                else
                    imagePanel.Open(index);
            }
            catch (System.Exception ex)
            {
                Debug.Log("show screen shot error: " + ex.Message);
            }
        }

        public void RenameFile(string source, string filename)
        {
            string path = DataUtility.GetPathName(source);
            int id = files.IndexOf(source);
            if (id != -1)
            {
                files[id] = path + "/" + filename+".jpg";
                if (IsVideoImage(source))
                    files[id] += path + "/" + filename + "_video.jpg";
                
                if(thumbnails.ContainsKey(source))
                {
                    thumbnails[files[id]] = thumbnails[source];
                    thumbnails.Remove(source);
                }
                File.Move(DataUtility.GetScreenShotPath() + source, DataUtility.GetScreenShotPath() + files[id]);
                File.Move(DataUtility.GetScreenThumbnailPath() + source, DataUtility.GetScreenThumbnailPath() + files[id]);
                if (IsVideoImage(source))
                {
                    string vfile = GetVideoFileName(source);

                    File.Move(DataUtility.GetScreenVideoPath() + vfile + ".json", DataUtility.GetScreenVideoPath() + filename + ".json");
                    File.Move(DataUtility.GetScreenVideoPath() + vfile + ".mp4", DataUtility.GetScreenVideoPath() + filename + ".mp4");
                }

                UIImage im = imageGallery.GetImage(source);
                if (im != null)
                    im.RenameTag(files[id]);
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

        public string GetPath(int index)
        {
            if (index > -1 && index < files.Count)
                return files[index];
            else
                return string.Empty;
        }

        public void DeleteFile(string str)
        {
            files.Remove(str);
            string path1 = DataUtility.GetScreenShotPath() + str;
            string path2 = DataUtility.GetScreenThumbnailPath() + str;
            File.Delete(path1);
            File.Delete(path2);

            DeleteVideo(str);

            UIImage im = imageGallery.GetImage(str);
            if (im != null)
                im.Delete(false);
        }

        public void DeleteVideo(string str)
        {
            string vfile = GetVideoFileName(str);
            if (vfile != string.Empty)
            {
                string path1 = DataUtility.GetScreenVideoPath() + vfile + ".json";
                string path2 = DataUtility.GetScreenVideoPath() + vfile + ".mp4";
                File.Delete(path1);
                File.Delete(path2);
            }
        }

        public string GetVideoFileName(string str)
        {
            int index = str.LastIndexOf('_');
            string end = str.Substring(index + 1);
            if (end.Equals("video.jpg"))
                return str.Substring(0, index);
            else
                return string.Empty;
        }

        public bool IsVideoImage(int index)
        {
            string str = GetPath(index);
            return IsVideoImage(str);
        }

        public bool IsVideoImage(string str)
        {
            return str.EndsWith("_video.jpg");
        }

        public void DeleteImage(int index)
        {
            if (index > -1 && index < files.Count)
            {
                DeleteFile(files[index]);
            }
        }

        public void DeleteImageFolder(string str)
        {
            DirectoryInfo thumbdir = new DirectoryInfo(DataUtility.GetScreenThumbnailPath()+str);
            if (thumbdir.Exists)
                thumbdir.Delete(true);
            DirectoryInfo dir = new DirectoryInfo(DataUtility.GetScreenShotPath() + str);
            if (dir.Exists)
                dir.Delete(true);
            DirectoryInfo videoDir = new DirectoryInfo(DataUtility.GetScreenVideoPath() + str);
            if (videoDir.Exists)
                videoDir.Delete(true);
        }

        public Sprite GetLatestThumbnail()
        {
            if (ExistFile)
            {
                string str = files[files.Count - 1];
                return GetImage(str, true);
            }
            else
                return null;
        }

        public Sprite GetImage(string str, bool isThumbnail)
        {
            if (isThumbnail)
            {
                if (!thumbnails.ContainsKey(str) || thumbnails[str] == null)
                    thumbnails[str] = DataUtility.CreateSprite(DataUtility.GetScreenThumbnailPath() + str);
                if (!files.Contains(str))
                    files.Add(str);
                return thumbnails[str];
            }
            else
                return DataUtility.CreateSprite(DataUtility.GetScreenShotPath() + str);
        }

        public void ShowLatestImage()
        {
            if (ExistFile)
                imagePanel.Open(files.Count - 1);
        }
    }
}
