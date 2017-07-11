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

        Action<int, System.Object> msgHandler;
        System.Object referObject;

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
            contentText.text = content;
            firstBtnText.text = button1Text;
            secondBtnText.text = button2Text;
        }

        public void OnBtnClicked(int i)
        {
            if (msgHandler != null)
                msgHandler(i, referObject);
            gameObject.SetActive(false);
        }
    }
}
