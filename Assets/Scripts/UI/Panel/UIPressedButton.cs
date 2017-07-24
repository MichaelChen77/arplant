using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace IMAV.UI
{
    public class UIPressedButton : MonoBehaviour
    {
        Image backgroundImage;
        public Color pressedColor;
        public Color normalColor;

        private void Awake()
        {
            backgroundImage = GetComponent<Image>();
        }

        void OnEnable()
        {
            backgroundImage.color = normalColor;
        }

        public void OnButtonDown()
        {
            backgroundImage.color = pressedColor;

        }

        public void OnButtonUp()
        {
            backgroundImage.color = normalColor;
        }
    }
}
