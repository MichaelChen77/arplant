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
        bool isPaused = false;

        Texture2D thumbnailTex;

        void Start()
        {
            Everyplay.RecordingStarted += RecordingStarted;
            Everyplay.RecordingStopped += RecordingStopped;
        }

        void Destroy()
        {
            Everyplay.RecordingStarted -= RecordingStarted;
            Everyplay.RecordingStopped -= RecordingStopped;
        }

        private void RecordingStarted()
        {
            SetIsRecording(true);
            isPaused = false;
            recordTime = 0;
            lastRecordSec = 0;
            ImageManager.Singleton.StartRecordVideo();
        }

        private void RecordingStopped()
        {
            SetIsRecording(false);
            recordTime = 0;
            SetRecordTimeText();
            ImageManager.Singleton.StopRecordVideo(OnPostScreenCaptured);
        }

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
            ResourceManager.Singleton.DebugString("Start video");
            Everyplay.StartRecording();
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
            ResourceManager.Singleton.DebugString("Stop video");
            Everyplay.StopRecording();
        }

        public void PauseVideoRecord()
        {
            isPaused = !isPaused;
            if (isPaused)
                Everyplay.PauseRecording();
            else
                Everyplay.ResumeRecording();
        }

        public override void Close()
        {

        }

        void Update()
        {
            if (isRecording && !isPaused)
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
