using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IMAV.UI
{
    public class UIMenuPanel : UIControl
    {
        public GameObject controlButton;
        RectTransform panelRect;

        void Awake()
        {
            panelRect = GetComponent<RectTransform>();
        }

        public override void Open()
        {
            LeanTween.move(panelRect, Vector2.zero, 0.25f).setEaseOutQuad();
            controlButton.SetActive(false);
        }

        public override void Close()
        {
            LeanTween.move(panelRect, new Vector2(panelRect.rect.width, 0f), 0.25f).setEaseInQuad();
            controlButton.SetActive(true);
        }
    }
}
