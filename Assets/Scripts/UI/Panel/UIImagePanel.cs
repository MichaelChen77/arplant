using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;

namespace IMAV.UI
{
    public class UIImagePanel : UIControl
    {
        public UISwipe swipe;
        public RectTransform bottomRect;
        public float moveTime = 0.2f;

        public override void Open()
        {
            Open(0);
        }

        public void Open(int index)
        {
            gameObject.SetActive(true);
            swipe.Open();
            swipe.PageCount = ImageManager.Singleton.Images.Count;
            swipe.OnSwipeCompleted = updateImages;
            swipe.OnSwipeClicked = clickImage;
            GotoPage(index);
        }

        void updateImages()
        {
            if (swipe.CurrentMoveType == UIMoveToType.None || swipe.CurrentPage == 0 || swipe.CurrentPage == (swipe.PageCount - 1))
            {
                return;
            }
            bool flag = (swipe.CurrentMoveType == UIMoveToType.Next);
            if ((flag && swipe.CurrentPage == 1) || (!flag && swipe.CurrentPage == swipe.PageCount - 2))
                return;
            Transform tran = swipe.Switch(flag);
            if (tran != null)
            {
                Image m = tran.GetComponent<Image>();
                Destroy(m.sprite);
                if (flag)
                    m.sprite = Load(ImageManager.Singleton.GetImagePath(swipe.CurrentPage + 1));
                else
                    m.sprite = Load(ImageManager.Singleton.GetImagePath(swipe.CurrentPage - 1));
            }
        }

        void clickImage()
        {
            float targetPos = 0;
            if (bottomRect.anchoredPosition.y == 0)
            {
                targetPos = -bottomRect.sizeDelta.y;
            }
            LeanTween.moveY(bottomRect, targetPos, moveTime);
        }

        public Sprite GetCurrentImage()
        {
            Transform curObj = null;
            if (swipe.CurrentPage == 0)
                curObj = swipe.transform.GetChild(0);
            else if (swipe.CurrentPage == swipe.PageCount - 1)
                curObj = swipe.transform.GetChild(swipe.transform.childCount - 1);
            else
                curObj = swipe.transform.GetChild(1);
            if(curObj != null)
            {
                return curObj.GetComponent<Image>().sprite;
            }
            return null;
        }

        public void GotoPage(int index)
        {
            int changedInt;
            if (index == 0)
                changedInt = 0;
            else if (index == swipe.PageCount - 1)
                changedInt = -2;
            else
                changedInt = -1;
            swipe.MovePage(changedInt, index);
            for (int i = 0; i < swipe.transform.childCount; i++)
            {
                Image img = swipe.transform.GetChild(i).GetComponent<Image>();
                Destroy(img.sprite);
                img.sprite = Load(ImageManager.Singleton.GetImagePath(index + i + changedInt));
            }
        }

        public void SaveImage()
        {
            Sprite sp = GetCurrentImage();
            if (sp != null)
            {
                ImageManager.Singleton.SaveScreenShot(sp.texture);
            }
        }

        public void DeleteImage()
        {
            ImageManager.Singleton.msgDialog.Show("Delete the selected item?","Cancel", "Delete", null, DeleteImageCallback);
        }

        void DeleteImageCallback(int res, System.Object refobj)
        {
            if (res == 1)
            {
                ImageManager.Singleton.DeleteImage(swipe.CurrentPage);
                swipe.PageCount = ImageManager.Singleton.Images.Count;
                if (swipe.PageCount > 0)
                {
                    if (swipe.CurrentPage == swipe.PageCount)
                        GotoPage(swipe.CurrentPage - 1);
                    else
                        GotoPage(swipe.CurrentPage);
                }
            }
        }

        public override void Close()
        {
            gameObject.SetActive(false);
            if (ImageManager.Singleton.imageGallery.isActiveAndEnabled)
                ImageManager.Singleton.imageGallery.Refresh();
            //ResourceManager.Singleton.Resume();
        }

        public Sprite Load(string str)
        {
            if (str != string.Empty)
            {
                try
                {
                    FileInfo fi = new FileInfo(str);
                    if (fi.Exists)
                    {
                        byte[] content = File.ReadAllBytes(str);
                        return DataUtility.CreateSprite(content);
                    }
                }
                catch { }
            }
            return null;
        }
    }
}
