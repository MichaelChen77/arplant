using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace IMAV.UI
{
    public class UIMediaCapture : UIControl
    {
        public GameObject thumbnailImage;
        public GameObject imageCaptureButton;
        public GameObject videoRecordButton;
        public GameObject closeButton;
        public GameObject pauseButton;
        public GameObject stopButton;
        public GameObject recordTimePanel;
        public Text recordTimeText;


        public Image previewImage;
        float recordTime = 0;
        int lastRecordSec = 0;

        [SerializeField]
        bool isRecording = false;

        public override void Open()
        {
            base.Open();
            if (previewImage.sprite == null)
                previewImage.sprite = ImageManager.Singleton.GetLastImage(true);
        }

        public void CaptureImage()
        {
            thumbnailImage.transform.localScale = Vector3.zero;
            ImageManager.Singleton.CaptureScreen(isRecording, OnPostScreenCaptured);
        }

        void OnPostScreenCaptured(string path)
        {
            Sprite sp = ImageManager.Singleton.Thumbnails[path];
            if (sp == null)
            {
                byte[] bytes = File.ReadAllBytes(DataUtility.GetScreenThumbnailPath() + path);
                sp = DataUtility.CreateSprite(bytes);
            }
            previewImage.sprite = sp;
            LeanTween.scale(thumbnailImage, Vector3.one, 0.2f);
        }

        public void StartVideoRecord()
        {
            SetIsRecording(true);
            recordTime = 0;
            lastRecordSec = 0;
            Everyplay.StartRecording();
        }

        void WalkSessions()
        {
            Debug.Log("Walking sessions");
            string basePath = Application.temporaryCachePath;
#if UNITY_ANDROID
            string[] sParts = basePath.Split('/');
            basePath = "/sdcard/Android/data/" + sParts[sParts.Length - 2] + "/cache/sessions/";
#elif UNITY_IOS
  basePath = basePath.Substring (0,basePath.Length-15);
  basePath += "/tmp/Everyplay/session/";
#endif
            string[] dirs = System.IO.Directory.GetDirectories(basePath);
            foreach (string s in dirs)
            {
                Debug.Log("Found folder: " + s);
                string[] files = System.IO.Directory.GetFiles(s);
                Debug.Log("It contains the following files:");
                foreach (string s2 in files)
                    Debug.Log(s2);
            }
        }

        void SetIsRecording(bool flag)
        {
            isRecording = flag;
            thumbnailImage.SetActive(!isRecording);
            videoRecordButton.SetActive(!isRecording);
            closeButton.SetActive(!isRecording);
            pauseButton.SetActive(isRecording);
            stopButton.SetActive(isRecording);
            recordTimePanel.SetActive(isRecording);
        }

        public void StopVideoRecord()
        {
            Everyplay.StopRecording();
            SetIsRecording(false);
        }

        public void PauseVideoRecord()
        {
            isRecording = !isRecording;
            if (isRecording)
                Everyplay.ResumeRecording();
            else
                Everyplay.PauseRecording();
        }

        public override void Close()
        {

        }

        void Update()
        {
            if (isRecording)
            {
                recordTime += Time.deltaTime;
                SetRecordTimeText();
            }
        }

        void SetRecordTimeText()
        {
            if (lastRecordSec != (int)recordTime)
            {
                lastRecordSec = (int)recordTime;
                recordTimeText.text = DataUtility.CovertToTimeString(lastRecordSec);
            }
        }
    }
}
