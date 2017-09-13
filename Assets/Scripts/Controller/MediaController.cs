using System;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using IMAV.UI;
using IMAV.Model;

namespace IMAV.Controller
{
    public class MediaController : MonoBehaviour
    {
        public UIImagePanel imagePanel;
        public UIImageGallery imageGallery;
        public UIVideoPlayer videoPlayer;
        public UIDialog msgDialog;
        public UITextInform textInform;
        public GameObject screenshotFlash;
        public AudioController audioController;

        private static MediaController mSingleton;
        public static MediaController Singleton
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

            //---Stripped down version---0818
            StartCoroutine(loadFiles());
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

        #region Screenshot and Video Record methods
        /// <summary>
        /// Takes a screenshot of the camera feed and any projected objects, without any UI.
        /// </summary>
        public void CaptureScreen(bool duringRecord, bool createThumbnail, Action<string> run)
        {
            System.DateTime dt = System.DateTime.Now.ToLocalTime();
            string date = dt.ToString("yyyyMMdd");
            string filePath = DataUtility.GetScreenShotPath() + date + "/";
            string thumbPath = DataUtility.GetScreenThumbnailPath() + date + "/";
            DataUtility.SetDirectory(filePath);
            DataUtility.SetDirectory(thumbPath);

            string filename = "WhizHome_" + dt.ToString("yyMMdd_HHmmss") + ".png";
            StartCoroutine(Screenshot(date + "/" + filename, createThumbnail, true, run));
        }

        IEnumerator Screenshot(string filepath, bool createThumbnail, bool showScreenshotHint, Action<string> run)
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
            byte[] bytes = screen.EncodeToPNG();

            System.IO.File.WriteAllBytes(DataUtility.GetScreenShotPath() + filepath, bytes);

            if (showScreenshotHint)
            {
                screenshotFlash.SetActive(true);
                PlayAudio(AudioEnum.Screenshot);
            }
            if (createThumbnail)
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

        public void PlayAudio(AudioEnum au)
        {
            if (audioController != null)
                audioController.PlayOneshot(au);
        }

        void SaveThumbnail(Texture2D screen, string filepath)
        {
            int h = DataUtility.GetRelatedHeight(Tags.ThumbnailWidth);
            Texture2D newTex = Instantiate(screen);
            TextureScale.Point(newTex, Tags.ThumbnailWidth, h);
            byte[] thumbbytes = newTex.EncodeToPNG();
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

        void WalkSessions(string videofile)
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
                        if (file.Extension == ".mp4")
                        {
                            File.Move(file.FullName, videofile);
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

            string fileName = "WhizHome_" + dt.ToString("yyMMdd_HHmmss");
            curRecordName = date + "/" + fileName;

            PlayAudio(AudioEnum.StartRecord);
            StartCoroutine(Screenshot(curRecordName + "_v$d.png", true, false, null));
        }

        public void StopRecordVideo(Action<string> run)
        {
            string videoName = DataUtility.GetScreenVideoPath() + curRecordName + ".mp4";
            WalkSessions(videoName);
            PlayAudio(AudioEnum.Save);
            if (run != null)
                run(curRecordName + "_v$d.png");
        }

        public void PlayVideo(string str)
        {
            string fileName = GetVideoFileName(str);
            string videoFile = DataUtility.GetScreenVideoPath() + fileName + ".mp4";
            videoPlayer.Open(videoFile);
        }

        public void SaveScreenShot(string str)
        {
            try
            {
                int id = str.IndexOf('/');
                if (id != -1)
                {
                    string source = DataUtility.GetScreenShotPath() + str;
                    string file = str.Substring(id + 1);
                    GallerySaver.CopyToGallery(source, file);
                }
            }
            catch(System.Exception ex)
            {
                TestCenter.Singleton.Log("error: " + ex.Message);
            }
        }

        public void SaveVideo(int index)
        {
            string str = GetPath(index);
            SaveVideo(str);
        }

        public void ScreenShotAndSave()
        {
            CaptureScreen(false, false, SaveScreenShot);
        }

        public void SaveVideo(string str)
        {
            string fullname = GetVideoFileName(str);
            if (!fullname.Equals(string.Empty))
            {
                string source = DataUtility.GetScreenVideoPath() + fullname + ".mp4";
                string file = fullname.Substring(fullname.IndexOf('/') + 1) + ".mp4";
                GallerySaver.CopyToGallery(source, file);
                textInform.ShowInform("Video Saved", true, 1f);
            }
            else
                textInform.ShowInform("Video donot Exist", true, 1f);
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


        public void ShareMedia(bool absolutePath, string str)
        {
            string path = str;
            if (!absolutePath)
            {
                if (IsVideoImage(str))
                    path = DataUtility.GetScreenVideoPath() + GetVideoFileName(str) + ".mp4";
                else
                    path = DataUtility.GetScreenShotPath() + str;
            }
            if (str.EndsWith(".mp4"))
                NativeShare.Share("share video", path, "", FileType.Video);
            else
                NativeShare.Share("share image", path, "", FileType.Image);
        }

        public void ShareString(string str)
        {
            NativeShare.Share("Whiz Home product: "+str, string.Empty, string.Empty, FileType.Text);
        }

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
                TestCenter.Singleton.Log("show screen shot error: " + ex.Message);
            }
        }

        public void RenameFile(string source, string filename)
        {
            string path = DataUtility.GetPathName(source);
            int id = files.IndexOf(source);
            if (id != -1)
            {
                files[id] = path + "/" + filename+".png";
                if (IsVideoImage(source))
                    files[id] += path + "/" + filename + "_v$d.png";
                
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

        public string GetAbsolutePath(int index)
        {
            if (index > -1 && index < files.Count)
            {
                if (IsVideoImage(files[index]))
                    return DataUtility.GetScreenVideoPath() + GetVideoFileName(files[index]) + ".mp4";
                else
                    return DataUtility.GetScreenShotPath() + files[index];
            }
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
            if (end.Equals("v$d.png"))
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
            return str.EndsWith("_v$d.png");
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
