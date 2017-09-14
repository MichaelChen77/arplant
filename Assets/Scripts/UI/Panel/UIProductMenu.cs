using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IMAV.Model;
using IMAV.Controller;

namespace IMAV.UI
{
    public class UIProductMenu : UIControl
    {
        ProductController currentObject;
        public SceneUIController uiController;
        public GToggleButton uiProdcutDlgBtn;
        public UICatalogPanel uiCatalog;
        public UIProductInforDialog productDlg;
        RectTransform rect;
        RectTransform infoRect;

        private void Awake()
        {
            rect = GetComponent<RectTransform>();
            infoRect = productDlg.GetComponent<RectTransform>();
        }

        public override void Open()
        {
            currentObject = SceneController.Singleton.CurrentObject;
            if (currentObject != null)
                base.Open();
        }

        void UpdatePosition()
        {
            if (currentObject != null)
                rect.position = Camera.main.WorldToScreenPoint(currentObject.GetTopPosition());
        }

        public void RemoveObject()
        {
            SceneController.Singleton.DeleteCurrentProduct();
            Close();
        }

        public void ShowInfo(GToggleButton btn)
        {
            btn.setTrigger();
            if (btn.TriggerFlag)
                ShowInfo();
            else
                HideInfo();
        }

        public void ShowInfo()
        {
            //productDlg.Open(currentObject.ProductModel);
            productDlg.Open();
            LeanTween.moveY(infoRect, infoRect.sizeDelta.y, 0.3f);
        }

        public void HideInfo()
        {
            LeanTween.moveY(infoRect, 0, 0.3f).setOnComplete(()=>{
                productDlg.Close();
            });
        }

        public void SwitchObject()
        {
            uiController.CloseUI(false);
            uiController.CurrentPanel = uiCatalog;
            uiCatalog.Open(true);
        }

        public override void Close()
        {
            base.Close();
            uiProdcutDlgBtn.setTrigger(false);
            infoRect.anchoredPosition = new Vector2(infoRect.anchoredPosition.x, 0);
            productDlg.Close();
        }

        private void Update()
        {
            UpdatePosition();
        }
    }
}
