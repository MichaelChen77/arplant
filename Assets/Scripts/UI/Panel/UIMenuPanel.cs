﻿using UnityEngine;

namespace IMAV.UI
{
    public class UIMenuPanel : UIControl
    {
        public GToggleButton controlButton;
		//test
		public GToggleButton SwitchWifiButton;
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
            if (!isOpened)
            {
                isOpened = true;
                if (controlButton != null)
                    controlButton.setTrigger(false);
                LeanTween.moveX(panelRect, -panelRect.rect.width, 0.25f).setEaseOutQuad();
                if (productRect != null && productRect.gameObject.activeSelf)
                    productRect.offsetMax = new Vector2(-panelRect.rect.width, productRect.offsetMax.y);
            }
        }

        public void OpenTrigger()
        {
            if (isOpened)
                Close();
            else
                Open();
        }

        public override void Close()
        {
            if (isOpened)
            {
                isOpened = false;
                if (controlButton != null)
                    controlButton.setTrigger(true);
                LeanTween.moveX(panelRect, 0, 0.25f).setEaseInQuad();
                if (productRect != null && productRect.gameObject.activeSelf)
                    productRect.offsetMax = new Vector2(0, productRect.offsetMax.y);
            }
            
        }

		bool isWifiOn = false;
		public void SwitchWifiButtonPress(){
			SwitchWifiButton.setTrigger(!isWifiOn);
			isWifiOn = !isWifiOn;
		}
    }
}
