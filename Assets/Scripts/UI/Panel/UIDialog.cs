using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace IMAV.UI
{
    public class UIDialog : MonoBehaviour
    {
        public Text contentText;
        public Text firstBtnText;
        public Text secondBtnText;
        public Button firstButton;
        public Button secondButton;
        public float externHeight = 120f;

        Action<int, System.Object> msgHandler;
        System.Object referObject;
        RectTransform dlgRect;

        private void Awake()
        {
            dlgRect = GetComponent<RectTransform>();
        }

        public void Show(string content)
        {
            Show(content, "Yes", "", null, null);
        }

        public void Show(string content, Action<int, System.Object> callback)
        {
            Show(content, "Yes", "No", null, callback);
        }

        public void Show(string content, System.Object refer, Action<int, System.Object> callback)
        {
            Show(content, "Yes", "No", refer, callback);
        }

        public void Show(string content, string button1Text, string button2Text, System.Object refer, Action<int, System.Object> callback)
        {
            gameObject.SetActive(true);
            msgHandler = callback;
            referObject = refer;
            SetButton(firstButton, firstBtnText, button1Text);
            SetButton(secondButton, secondBtnText, button2Text);
            contentText.text = content;
            StartCoroutine(DelayRefresh());
        }

        IEnumerator DelayRefresh()
        {
            yield return new WaitForEndOfFrame();
            float h = contentText.preferredHeight + externHeight;
            Debug.Log("height: " + contentText.preferredHeight);
            dlgRect.sizeDelta = new Vector2(dlgRect.sizeDelta.x, h);
        }

        public void OnBtnClicked(int i)
        {
            if (msgHandler != null)
                msgHandler(i, referObject);
            gameObject.SetActive(false);
        }

        public void SetButton(Button btn, Text btnText, string _text)
        {
            if (_text.Equals(string.Empty))
                btn.gameObject.SetActive(false);
            else
            {
                btn.gameObject.SetActive(true);
                btnText.text = _text;
            }
        }
    }
}
