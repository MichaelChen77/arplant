using UnityEngine;
using UnityEngine.UI;
using IMAV.Controller;
using System;
#if PLATFORM_IOS
using UnityEngine.iOS;
using UnityEngine.Apple.ReplayKit;
#endif
namespace IMAV.UI
{
	public class UIMediaCapture : UIControl
	{
		private bool alreadySavedInAppGallery;
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
			//            if (Everyplay.IsSupported() && Everyplay.IsRecordingSupported())
			#if PLATFORM_IOS
			print("ReplayKit.cameraEnabled = " + ReplayKit.cameraEnabled);
			if (ReplayKit.APIAvailable)
			{
				videoRecordButton.gameObject.SetActive(true);
				//                InitEvents();
			}
			else
			{
				videoRecordButton.gameObject.SetActive(false);
				//                DestroyEvents();
			}
			#endif
		}

		void InitEvents()
		{
			//            Everyplay.RecordingStarted += RecordingStarted;
			//            Everyplay.RecordingStopped += RecordingStopped;
			//            Everyplay.ReadyForRecording += Everyplay_ReadyForRecording;
		}

		void DestroyEvents()
		{
			//            Everyplay.RecordingStarted -= RecordingStarted;
			//            Everyplay.RecordingStopped -= RecordingStopped;
			//            Everyplay.ReadyForRecording -= Everyplay_ReadyForRecording;
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

		void StartVideoRecord()
		{
			string lastError = "";
			//			if (Everyplay.IsReadyForRecording()){
			//				print ("UIMediaCapture StartVideoRecord is ready");
			//                Everyplay.StartRecording();
			//			}
			//The following code is especially for ios platform
			//			if (!ReplayKit.APIAvailable) {
			//				MediaController.Singleton.msgDialog.Show ("Video recording is not ready yet, please try again later");
			//				return;
			//			} else {
			//				ReplayKit.StartRecording();
			//			}
			#if PLATFORM_IOS
			try{
				print("StartVideoRecord");
				RecordingStarted_ReplayKit();
				ReplayKit.StartRecording();
			} 
			catch (Exception e)
			{
				lastError = e.ToString();
			}

			#endif          
		}

		//        void Everyplay_ReadyForRecording(bool isEnabled)
		//        {
		//            videoRecordButton.interactable = isEnabled;
		//        }

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

		void StopVideoRecord()
		{
			//            Everyplay.StopRecording();
			string lastError = "";
			#if PLATFORM_IOS
			try {
				print("StopVideoRecord");
				RecordingStopped_ReplayKit();
				ReplayKit.StopRecording();
			}
			catch (Exception e)
			{
				lastError = e.ToString();
			}
			print("ReplayKit.recordingAvailable = " + ReplayKit.recordingAvailable);
			//			RecordingStopped_ReplayKit();

			#endif
		}

		public void PauseVideoRecord()
		{
			print("Pause ReplayKit.recordingAvailable = " + ReplayKit.recordingAvailable);
			isPaused = !isPaused;
			//            if (isPaused)
			//                Everyplay.PauseRecording();
			//            else
			//                Everyplay.ResumeRecording();
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
			//            if (isRecording && !isPaused)
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

		//The following functions are end with _ReplayKit 
		void RecordingStarted_ReplayKit(){
			alreadySavedInAppGallery = false;
			SetIsRecording(true);
			recordTime = 0;
			lastRecordSec = 0;
			MediaController.Singleton.StartRecordVideo();
		}

		void RecordingStopped_ReplayKit(){
			SetIsRecording(false);
			recordTime = 0;
			SetRecordTimeText();
			//			MediaController.Singleton.StopRecordVideo(OnPostScreenCaptured);
		}

		void OnGUI(){
			print("OnGUI ReplayKit.recordingAvailable = " + ReplayKit.recordingAvailable);
			if (ReplayKit.recordingAvailable) {
				if (alreadySavedInAppGallery == false) {
					alreadySavedInAppGallery = true;
					MediaController.Singleton.StopRecordVideo (OnPostScreenCaptured);
				}
				if (GUI.Button (new Rect (10, 350, 500, 200), "Preview")) {
					ReplayKit.Preview ();
				}
				if (GUI.Button (new Rect (10, 560, 500, 200), "Discard")) {
					ReplayKit.Discard ();
				}
			}
		}
				
	}
}
