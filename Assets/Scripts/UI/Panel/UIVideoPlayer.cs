using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace IMAV.UI
{
    [RequireComponent(typeof(VideoPlayer))]
    [RequireComponent(typeof(AudioSource))]
    public class UIVideoPlayer : UIControl, IPointerClickHandler
    {
        public RawImage image;
        public RectTransform uiRect;
        public float showTime = 0.5f;
        public float openTime = 0.15f;
        public Text playTimeText;
        public Text videoTimeText;
        public Text nameText;
        public Slider videoSlider;
        public GToggleButton playButton;
        public DisableSelf buttonEffect;

        bool menuShowedTicking = false;
        float menuShowedTime = 0;
        protected VideoPlayer player;
        protected AudioSource audioSource;
        RectTransform playerRect;
        string currentURL;

        public VideoPlayer Player
        {
            get { return player; }
        }

        public string VideoURL
        {
            get { return currentURL; }
            set { currentURL = value; }
        }

        private void Awake()
        {
            player = GetComponent<VideoPlayer>();
            audioSource = GetComponent<AudioSource>();
            playerRect = GetComponent<RectTransform>();
        }

        void Start()
        {
            player.playOnAwake = false;
            audioSource.playOnAwake = false;
            audioSource.Pause();

            player.prepareCompleted += OnVideoPrepared;
            player.seekCompleted += Player_seekCompleted;
            player.loopPointReached += Player_loopPointReached;
            player.audioOutputMode = VideoAudioOutputMode.AudioSource;
            player.controlledAudioTrackCount = 1;
            player.EnableAudioTrack(0, true);
            player.SetTargetAudioSource(0, audioSource);
            player.source = VideoSource.Url;
            player.aspectRatio = VideoAspectRatio.FitInside;
            player.skipOnDrop = true;
        }

        private void Player_loopPointReached(VideoPlayer source)
        {
            playButton.setTriggerWithoutAnimation(false);
            ShowMenu(true);
        }

        public void Open(string str)
        {
            Open();
            Load(str);
        }

        public void Load(string str)
        {
            player.url = str;
            nameText.text = System.IO.Path.GetFileName(str);
            player.source = VideoSource.Url;
            player.Prepare();
        }

        void OnVideoPrepared(VideoPlayer vp)
        {
            image.texture = player.texture;
            SetAspectMode(player.aspectRatio);
            videoSlider.value = 0;
            int timeCount = (int)(player.frameCount / player.frameRate);
            videoSlider.maxValue = player.frameCount;
            videoTimeText.text = DataUtility.CovertToTimeString(timeCount);
            player.Play();
            audioSource.Play();
        }

        public void LoopSetVideoAspectMode()
        {
            switch(player.aspectRatio)
            {
                case VideoAspectRatio.FitInside: SetAspectMode(VideoAspectRatio.Stretch); break;
                case VideoAspectRatio.Stretch: SetAspectMode(VideoAspectRatio.NoScaling); break;
                case VideoAspectRatio.NoScaling: SetAspectMode(VideoAspectRatio.FitInside); break;
            }
        }

        public void SetAspectMode(VideoAspectRatio mode)
        {
            player.aspectRatio = mode;
            switch (player.aspectRatio)
            {
                case VideoAspectRatio.Stretch: image.rectTransform.sizeDelta = playerRect.rect.size; break;
                case VideoAspectRatio.NoScaling: image.rectTransform.sizeDelta = new Vector2(player.texture.width, player.texture.height); break;
                case VideoAspectRatio.FitHorizontally: image.rectTransform.sizeDelta = DataUtility.GetFitVector(true, new Vector2(player.texture.width, player.texture.height), playerRect.rect.size); break;
                case VideoAspectRatio.FitVertically: image.rectTransform.sizeDelta = DataUtility.GetFitVector(false, new Vector2(player.texture.width, player.texture.height), playerRect.rect.size); break;
                case VideoAspectRatio.FitInside:
                    float rateX = player.texture.width / playerRect.rect.size.x;
                    float rateY = player.texture.height / playerRect.rect.size.y;
                    image.rectTransform.sizeDelta = DataUtility.GetFitVector(rateX > rateY, new Vector2(player.texture.width, player.texture.height), playerRect.rect.size);
                    break;
                case VideoAspectRatio.FitOutside:
                    float rateX1 = player.texture.width / playerRect.rect.size.x;
                    float rateY1 = player.texture.height / playerRect.rect.size.y;
                    image.rectTransform.sizeDelta = DataUtility.GetFitVector(rateX1 < rateY1, new Vector2(player.texture.width, player.texture.height), playerRect.rect.size);
                    break;
            }
        }

        public void OnPointerClick(PointerEventData data)
        { 
            ShowMenu(!uiRect.gameObject.activeSelf);
        }

        public void ShowMenu(bool flag)
        {
            menuShowedTicking = flag;
            menuShowedTime = 0;
            uiRect.gameObject.SetActive(menuShowedTicking);
            LeanTween.alpha(uiRect, flag ? 1 : 0, openTime);
        }

        public void PlayVideo()
        {
            if (player.isPlaying)
            {
                player.Pause();
                audioSource.Pause();
                playButton.setTrigger(false);
            }
            else
            {
                player.Play();
                audioSource.Play();
                playButton.setTrigger(true);
                StartMenuShowedTicking(true);
            }
        }

        public void videoFrameHandlerDown(BaseEventData data)
        {
            player.Pause();
            playButton.gameObject.SetActive(false);
        }

        public void videoFrameHandlerUp(BaseEventData data)
        {
            player.frame = (long)videoSlider.value;
            videoSlideOnDrag(data);
        }

        private void Player_seekCompleted(VideoPlayer source)
        {
            player.Play();
            playButton.gameObject.SetActive(true);
        }

        public void videoSlideOnDrag(BaseEventData data)
        {
            int _time = (int)(videoSlider.value / player.frameRate);
            playTimeText.text = DataUtility.CovertToTimeString(_time);
        }

        public void StartMenuShowedTicking(bool flag)
        {
            menuShowedTicking = flag;
            menuShowedTime = 0;
        }

        public void ButtonDown(RectTransform rect)
        {
            StartMenuShowedTicking(false);
            buttonEffect.OpenBelow(rect);
        }

        void Update()
        {
            if (menuShowedTicking && player.isPlaying)
            {
                menuShowedTime += Time.deltaTime;
                if (menuShowedTime > showTime)
                    ShowMenu(false);
            }
            if (player.isPrepared && player.isPlaying)
            {
                playTimeText.text = DataUtility.CovertToTimeString((int)player.time);
                videoSlider.value = player.frame;
            }
        }

        public void StopVideo()
        {
            player.Stop();
        }

        public override void Close()
        {
            player.Stop();
            ShowMenu(false);
            base.Close();
        }
    }
}
