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

        bool menuShowed = false;
        float _time = 0;
        protected VideoPlayer player;
        protected AudioSource audioSource;
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
        }

        void Start()
        {
            player.playOnAwake = false;
            audioSource.playOnAwake = false;
            audioSource.Pause();

            player.audioOutputMode = VideoAudioOutputMode.AudioSource;
            player.EnableAudioTrack(0, true);
            player.SetTargetAudioSource(0, audioSource);
            player.source = VideoSource.Url;

            RenderTexture rt = new RenderTexture(Screen.width, Screen.height, 24);
            rt.Create();
            player.targetTexture = rt;

            //Load("file://D:/WorkSpace/AR/Test/big_buck_bunny.mp4");
            //Load("file://C:/WorkSpace/AR/TestImages/Videos/20170728/WhizHome_20170728_105225.mp4");
            Load("file://C:/WorkSpace/AR/TestImages/Videos/20170728/darker.mp4");
        }

        public override void Open()
        {
            base.Open();
            player.Prepare();
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
            if (player.isPrepared)
                Debug.Log("prepared");
        }

        void OnVideoPrepared(VideoPlayer vp)
        {
            playTimeText.text = vp.time.ToString();
            videoTimeText.text = (vp.frame / vp.frameRate).ToString();
        }

        public override void Close()
        {
            base.Close();
        }

        public void OnPointerClick(PointerEventData data)
        { 
            ShowMenu(!menuShowed);
        }

        public void ShowMenu(bool flag)
        {
            menuShowed = flag;
            if (menuShowed)
            {
                uiRect.gameObject.SetActive(true);
                LeanTween.alpha(uiRect, 1, openTime);
            }
            else
            {
                LeanTween.alpha(uiRect, 0, openTime).setOnComplete(() => uiRect.gameObject.SetActive(false));
            }
        }

        public void Play(string str)
        {

        }

        public void PlayVideo()
        {
            if (player.isPlaying)
            {
                player.Pause();
                audioSource.Pause();
            }
            else
            {
                image.texture = player.texture;
                image.rectTransform.sizeDelta = new Vector2(player.texture.width, player.texture.height);
                videoSlider.maxValue = player.frameCount;
                videoTimeText.text = DataUtility.CovertToTimeString((int)(player.frameCount / player.frameRate));
                player.Play();
                audioSource.Play();
            }
            _time = 0;
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

        void Update()
        {
            if (menuShowed)
            {
                _time += Time.deltaTime;
                if (_time > showTime)
                {
                    _time = 0;
                    ShowMenu(false);
                }
            }
            if(player.isPrepared && player.isPlaying)
            {
                playTimeText.text = DataUtility.CovertToTimeString((int)player.time);
                videoSlider.value = player.frame;
            }
        }
    }
}
