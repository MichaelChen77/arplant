using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace IMAV.UI
{
    public class UITextInform : MonoBehaviour
    {
        public Text informText;
        public float moveTime = 0.2f;
        public float targetHeight;

        RectTransform informRect;

        bool mAutoHide = false;
        float mShowTime = 0f;
        bool showed = false;

        void Awake()
        {
            informRect = GetComponent<RectTransform>();
        }

        public void ShowInform(string str, bool autoHide, float showTime)
        {
            gameObject.SetActive(true);
            informText.text = str;
            mAutoHide = autoHide;
            mShowTime = showTime;
            if (informRect == null)
                informRect = GetComponent<RectTransform>();
            if (!showed)
            {
                LeanTween.moveY(informRect, targetHeight, moveTime).setOnComplete(HideInform);
                showed = true;
            }
            else if (mAutoHide)
                HideInform();
        }

        public void ShowInform(string str, float delay)
        {
            StartCoroutine(delayShow(str, delay, false, 0));
        }

        public void ShowInform(string str, float delay, bool autoHide, float showTime)
        {
            StartCoroutine(delayShow(str, delay, autoHide, showTime));
        }

        public void ShowInform(string str)
        {
            ShowInform(str, false, 0);
        }

        void HideInform()
        {
            if(mAutoHide)
            {
                Invoke("Hide", mShowTime);
            }
        }

		IEnumerator delayShow(string str, float delay, bool autoHide, float showTime)
		{
			yield return new WaitForSeconds(delay);
			ShowInform(str, autoHide, showTime);
		}

        public void Hide()
        {
            Hide(moveTime);
        }

        public void Hide(float t)
        {
            if (gameObject.activeSelf)
            {
                LeanTween.moveY(informRect, informRect.sizeDelta.y + 5, t).setOnComplete(() =>
                {
                    showed = false;
                    gameObject.SetActive(false);
                });
            }
        }
    }
}
