using UnityEngine;
using UnityEngine.UI;
using IMAV.Controller;

namespace IMAV.UI
{
    public class UIMediaCapture : UIControl
    {
        public GameObject thumbnailImage;
        public GameObject imageCaptureButton;
        public Button videoRecordButton;
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
                videoRecordButton.gameObject.SetActive(true);
                InitEvents();
            }
            else
            {
                videoRecordButton.gameObject.SetActive(false);
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
            MediaController.Singleton.StartRecordVideo();
        }

        private void RecordingStopped()
        {
            SetIsRecording(false);
            recordTime = 0;
            SetRecordTimeText();
            MediaController.Singleton.StopRecordVideo(OnPostScreenCaptured);
        }

        public override void Open()
        {
            base.Open();
            previewImage.sprite = MediaController.Singleton.GetLatestThumbnail();
        }

        public void CaptureImage()
        {
            thumbnailImage.transform.localScale = Vector3.zero;
            MediaController.Singleton.CaptureScreen(isRecording, true, OnPostScreenCaptured);
        }

        void OnPostScreenCaptured(string path)
        {
            Sprite sp = MediaController.Singleton.GetImage(path, true);
            previewImage.sprite = sp;
            LeanTween.scale(thumbnailImage, Vector3.one, 0.4f);
        }

        public void StartVideoRecord()
        {
            if (Everyplay.IsReadyForRecording())
                Everyplay.StartRecording();
            else
                MediaController.Singleton.msgDialog.Show("Video recording is not ready yet, please try again later");
        }

        void Everyplay_ReadyForRecording(bool isEnabled)
        {
            videoRecordButton.interactable = isEnabled;
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
            if (MediaController.Singleton.ExistFile)
            {
                MediaController.Singleton.ShowLatestImage();
                Close();
            }
            else
            {
                MediaController.Singleton.msgDialog.Show("Not any image or video exist in the gallery!");
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
