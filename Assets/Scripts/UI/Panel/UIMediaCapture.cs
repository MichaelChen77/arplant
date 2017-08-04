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
        public GToggleButton videoRecordButton;
        public GameObject closeButton;
        public GameObject pauseButton;
        public GameObject stopButton;
        public GameObject recordTimePanel;
        public Text recordTimeText;
        public Image previewImage;

        float recordTime = 0;
        int lastRecordSec = 0;

        bool isRecording = false;
        bool isPaused = false;

        Texture2D thumbnailTex;

        private void Awake()
        {
            if (Everyplay.IsSupported() && Everyplay.IsRecordingSupported())
            {
                videoRecordButton.setTrigger(true);
                InitEvents();
            }
            else
            {
                videoRecordButton.setTrigger(false);
                DestroyEvents();
            }
        }

        void InitEvents()
        {
            Everyplay.RecordingStarted += RecordingStarted;
            Everyplay.RecordingStopped += RecordingStopped;
            Everyplay.ReadyForRecording += Everyplay_ReadyForRecording;
        }

        void DestroyEvents()
        {
            Everyplay.RecordingStarted -= RecordingStarted;
            Everyplay.RecordingStopped -= RecordingStopped;
            Everyplay.ReadyForRecording -= Everyplay_ReadyForRecording;
        }

        void OnDestroy()
        {
            DestroyEvents();
        }

        private void RecordingStarted()
        {
            SetIsRecording(true);
            isPaused = false;
            recordTime = 0;
            lastRecordSec = 0;
            MediaCenter.Singleton.StartRecordVideo();
        }

        private void RecordingStopped()
        {
            SetIsRecording(false);
            recordTime = 0;
            SetRecordTimeText();
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
            LeanTween.scale(thumbnailImage, Vector3.one, 0.4f);
        }

        public void StartVideoRecord()
        {
            if (Everyplay.IsSupported() && Everyplay.IsRecordingSupported())
            {
                if (Everyplay.IsReadyForRecording())
                    Everyplay.StartRecording();
                else
                    MediaCenter.Singleton.msgDialog.Show("Video recording is not ready yet, please try again later");
            }
            else
            {
                MediaCenter.Singleton.msgDialog.Show("Video recording is not supported in this device");
            }
        }

        private void Everyplay_ReadyForRecording(bool enabled)
        {
            videoRecordButton.setTrigger(enabled);
        }

        void SetIsRecording(bool flag)
        {
            isRecording = flag;
            thumbnailImage.SetActive(!isRecording);
            videoRecordButton.gameObject.SetActive(!isRecording);
            closeButton.SetActive(!isRecording);
            pauseButton.SetActive(isRecording);
            stopButton.SetActive(isRecording);
            recordTimePanel.SetActive(isRecording);
        }

        public void StopVideoRecord()
        {
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
            if (Everyplay.IsReadyForRecording())
                videoRecordButton.gameObject.SetActive(true);
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
