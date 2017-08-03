using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace IMAV.UI
{
    [System.Serializable]
    public struct UIState
    {
        public Sprite sprite;
        public Color color;
        public Color bgcolor;
        public float animationTime;
        public UIAnimationType animType;
    }

    public enum UIAnimationType
    {
        None, Rotate, Scale, Alpha, Color
    }

    public class GStateButton : MonoBehaviour
    {
        public Image targetImage;
        public Image bgImage;
        public UIState[] buttonStates;

        int buttonID = 0;
        public int Status
        {
            get { return buttonID; }
        }

        public int StatusCount
        {
            get { return buttonStates.Length; }
        }

        void Awake()
        {
            SetStatus(buttonID);
        }

        Vector3 originVector;
        Color originColor;
        bool startRotate = false;

        public void SetStatusWithCheck(int _id)
        {
            if (_id != buttonID)
                SetStatus(_id);
        }

        public void SetStatus(int _id)
        {
            UIAnimationType _type = buttonStates[buttonID].animType;
            if (_id == buttonID)
                _type = UIAnimationType.None;
            if (_id > -1 && _id < buttonStates.Length)
            {
                buttonID = _id;
                RestoreState(_type);
                targetImage.sprite = buttonStates[_id].sprite;
                if (buttonStates[_id].animType != UIAnimationType.Color)
                    targetImage.color = buttonStates[_id].color;
                bgImage.color = buttonStates[_id].bgcolor;
                switch (buttonStates[_id].animType)
                {
                    case UIAnimationType.Rotate:
                        originVector = targetImage.transform.eulerAngles;
                        _count = (int)((buttonStates[_id].animationTime/Time.fixedDeltaTime)/36);
                        _passFrame = 0;
                        startRotate = true;
                        //LeanTween.rotateZ(targetImage.gameObject, 180, buttonStates[_id].animationTime).setLoopType(LeanTweenType.punch);
                        break;
                    case UIAnimationType.Alpha:
                        originColor = targetImage.color;
                        LeanTween.alpha(targetImage.GetComponent<RectTransform>(), 0, buttonStates[_id].animationTime).setLoopPingPong();
                        break;
                    case UIAnimationType.Color:
                        originColor = targetImage.color;
                        LeanTween.color(targetImage.GetComponent<RectTransform>(), buttonStates[_id].color, buttonStates[_id].animationTime).setLoopPingPong();
                        break;
                    case UIAnimationType.Scale:
                        originVector = targetImage.transform.localScale;
                        LeanTween.scale(targetImage.gameObject, new Vector3(0.7f, 0.7f, 1), buttonStates[_id].animationTime).setLoopPingPong();
                        break;
                    case UIAnimationType.None: LeanTween.cancel(targetImage.gameObject); break;
                }
            }
        }

        void RestoreState(UIAnimationType _type)
        {
            switch(_type)
            {
                case UIAnimationType.Rotate: startRotate = false; targetImage.transform.eulerAngles = originVector; break;
                case UIAnimationType.Scale: targetImage.transform.localScale = originVector; break;
                case UIAnimationType.Color: targetImage.color = originColor; break;
                case UIAnimationType.Alpha: targetImage.color = originColor; break;
            }
        }

        int _count = 0;
        int _passFrame = 0;
        void FixedUpdate()
        {
            if (startRotate)
            {
                _passFrame++;
                if (_passFrame == _count)
                {
                    targetImage.transform.Rotate(Vector3.back, 10f);
                    _passFrame = 0;
                }
            }
        }

        public void SetNextStatus()
        {
            int id = buttonID + 1;
            if (id >= StatusCount)
                id = 0;
            SetStatus(id);
        }
    }
}
