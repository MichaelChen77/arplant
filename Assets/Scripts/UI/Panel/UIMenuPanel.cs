using UnityEngine;

namespace IMAV.UI
{
    public class UIMenuPanel : UIControl
    {
        public GameObject controlButton;
        public RectTransform productRect;
        RectTransform panelRect;
        public RectTransform PanelRect
        {
            get { return panelRect; }
        }

        bool isOpened = false;
        public bool IsOpened
        {
            get { return isOpened; }
        }

        void Awake()
        {
            panelRect = GetComponent<RectTransform>();
        }

        public override void Open()
        {
            isOpened = true;
            LeanTween.moveX(panelRect, -panelRect.rect.width, 0.25f).setEaseOutQuad();
            if (productRect.gameObject.activeSelf)
                productRect.offsetMax = new Vector2(-panelRect.rect.width, productRect.offsetMax.y);
            controlButton.SetActive(false);
        }

        public override void Close()
        {
            isOpened = false;
            LeanTween.moveX(panelRect, 0, 0.25f).setEaseInQuad();
            if (productRect.gameObject.activeSelf)
                productRect.offsetMax = new Vector2(0, productRect.offsetMax.y);
            controlButton.SetActive(true);
        }
    }
}
