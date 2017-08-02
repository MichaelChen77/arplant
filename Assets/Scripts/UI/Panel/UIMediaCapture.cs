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
        //public GameObject HUDCam;

        public UIImagePanel imagePanel;


        public Image previewImage;
        float recordTime = 0;
        int lastRecordSec = 0;

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
            ResourceManager.Singleton.DebugString("recording start");
            MediaCenter.Singleton.StartRecordVideo();
        }

        private void RecordingStopped()
        {
            SetIsRecording(false);
            recordTime = 0;
            SetRecordTimeText();
            ResourceManager.Singleton.DebugString("recording stop");
            //HUDCam.SetActive(false);
            MediaCenter.Singleton.StopRecordVideo(OnPostScreenCaptured);
        }

        public override void Open()
        {
            base.Open();
            previewImage.sprite = MediaCenter.Singleton.GetLatestThumbnail();
        }

        public void CaptureImage()
        {
            thumbnailImage.transform.localScale = Vector3.zero;
            MediaCenter.Singleton.CaptureScreen(isRecording, OnPostScreenCaptured);
        }

        void OnPostScreenCaptured(string path)
        {
            Sprite sp = MediaCenter.Singleton.GetImage(path, true);
            previewImage.sprite = sp;
            LeanTween.scale(thumbnailImage, Vector3.one, 0.3f);
        }

        public void StartVideoRecord()
        {
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
            base.Close();
        }

        public void ShowImage()
        {
            if (MediaCenter.Singleton.ExistFile)
            {
                MediaCenter.Singleton.ShowLatestImage();
                Close();
            }
            else
            {
                MediaCenter.Singleton.msgDialog.Show("Not any image\video exist in the gallery!");
            }
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
