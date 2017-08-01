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
        public Slider videoSlider;

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
            player.audioOutputMode = VideoAudioOutputMode.AudioSource;
            player.controlledAudioTrackCount = 1;
            player.EnableAudioTrack(0, true);
            player.SetTargetAudioSource(0, audioSource);
            player.source = VideoSource.Url;
            player.aspectRatio = VideoAspectRatio.FitInside;

            //Load("file://D:/WorkSpace/AR/Test/big_buck_bunny.mp4");
            //Load("file://C:/WorkSpace/AR/TestImages/Videos/20170728/WhizHome_20170728_105225.mp4");
            Load("file://C:/WorkSpace/AR/TestImages/Videos/20170728/darker.mp4");
        }

        public void Open(string str)
        {
            Open();
            Load(str);
        }

        public void Load(string str)
        {
            player.url = str;
            player.source = VideoSource.Url;
            player.Prepare();
        }

        void OnVideoPrepared(VideoPlayer vp)
        {
            image.texture = player.texture;
            SetAspectMode(player.aspectRatio);
            videoSlider.value = 0;
            videoSlider.maxValue = player.frameCount;
            videoTimeText.text = DataUtility.CovertToTimeString((int)(player.frameCount / player.frameRate));
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
            ShowMenu(!menuShowedTicking);
        }

        public void ShowMenu(bool flag)
        {
            menuShowedTicking = flag;
            menuShowedTime = 0;
            if (menuShowedTicking)
            {
                uiRect.gameObject.SetActive(true);
                LeanTween.alpha(uiRect, 1, openTime);
            }
            else
            {
                LeanTween.alpha(uiRect, 0, openTime).setOnComplete(() => uiRect.gameObject.SetActive(false));
            }
        }

        public void PlayVideo()
        {
            Debug.Log("Click");
            if (player.isPlaying)
            {
                player.Pause();
                audioSource.Pause();
            }
            else
            {
                player.Play();
                audioSource.Play();
            }
        }

        public void videoFrameHandlerDown(BaseEventData data)
        {
            player.Pause();
        }

        public void videoFrameHandlerUp(BaseEventData data)
        {
            videoSlideOnDrag(data);
            StartCoroutine(DelayPlay());
        }

        IEnumerator DelayPlay()
        {
            yield return new WaitUntil(() => !player.isPlaying);
            player.Play();
        }

        public void videoSlideOnDrag(BaseEventData data)
        {
            player.frame = (long)videoSlider.value;
            playTimeText.text = DataUtility.CovertToTimeString((int)player.time);
        }

        public void StopMenuShowedTicking()
        {
            menuShowedTicking = false;
            menuShowedTime = 0;
        }

        public void SelectOn()
        {
            Debug.Log("onslect");
        }

        void Update()
        {
            if (menuShowedTicking)
            {
                menuShowedTime += Time.deltaTime;
                if (menuShowedTime > showTime)
                    ShowMenu(false);
            }
            if(player.isPrepared && player.isPlaying)
            {
                playTimeText.text = DataUtility.CovertToTimeString((int)player.time);
                videoSlider.value = player.frame;
            }
        }

        public override void Close()
        {
            base.Close();
            ShowMenu(false);
        }
    }
}
