using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;

namespace IMAV.UI
{
    public class UIImagePanel : UIControl
    {
        public UISwipe swipe;
        public RectTransform bottomRect;
        public RectTransform topRect;
        public float moveTime = 0.2f;
        public Image tempImage;
        public UIFileInforDialog infoDlg;

        public override void Open()
        {
            Open(0);
        }

        public void Open(int index)
        {
            gameObject.SetActive(true);
            swipe.Open();
            swipe.PageCount = MediaCenter.Singleton.Images.Count;
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
                ClearImage(m);
                if (flag)
                {
                    //m.sprite = DataUtility.CreateSprite(MediaCenter.Singleton.GetImagePath(swipe.CurrentPage + 1));
                    StartCoroutine(startLoadSprite(m, swipe.CurrentPage + 1));
                    DetectVideo(m.transform, swipe.CurrentPage + 1);
                }
                else
                {
                    //m.sprite = DataUtility.CreateSprite(MediaCenter.Singleton.GetImagePath(swipe.CurrentPage - 1));
                    StartCoroutine(startLoadSprite(m, swipe.CurrentPage - 1));
                    DetectVideo(m.transform, swipe.CurrentPage - 1);
                }
                AutoSetImageColor(m);
            }
        }

        IEnumerator startLoadSprite(Image m, int id)
        {
            string str = MediaCenter.Singleton.GetImagePath(id);
            WWW www = new WWW(str);
            yield return www;
            m.sprite = DataUtility.CreateSprite(www.texture);
        }

        void clickImage()
        {
            float targetPos = 0;
            if (bottomRect.anchoredPosition.y == 0)
            {
                targetPos = -bottomRect.sizeDelta.y;
            }
            LeanTween.moveY(bottomRect, targetPos, moveTime);
            targetPos = 0;
            if(topRect.anchoredPosition.y == 0)
            {
                targetPos = topRect.sizeDelta.y;
            }
            LeanTween.moveY(topRect, targetPos, moveTime);
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
                ClearImage(img);
                img.sprite = DataUtility.CreateSprite(MediaCenter.Singleton.GetImagePath(index + i + changedInt));
                DetectVideo(img.transform, index + i + changedInt);
                AutoSetImageColor(img);
            }
        }

        public void DetectVideo(Transform m, string str)
        {
            if (MediaCenter.Singleton.IsVideoImage(str))
                m.GetChild(0).gameObject.SetActive(true);
            else
                m.GetChild(0).gameObject.SetActive(false);
        }

        public void DetectVideo(Transform m, int index)
        {
            string str = MediaCenter.Singleton.GetPath(index);
            DetectVideo(m, str);
        }

        public void PlayVideo()
        {
            string str = MediaCenter.Singleton.GetPath(swipe.CurrentPage);
            MediaCenter.Singleton.PlayVideo(str);
        }

        public void ShareFile()
        {
            string str = MediaCenter.Singleton.GetPath(swipe.CurrentPage);
            MediaCenter.Singleton.ShareMedia(false, str);
        }

        void ClearImage(Image img)
        {
            if (img.sprite != null)
                Destroy(img.sprite.texture);
            Destroy(img.sprite);
        }

        void AutoSetImageColor(Image img)
        {
            if (img.sprite == null)
                img.color = Color.clear;
            else
                img.color = Color.white;
        }

        public void SaveFile()
        {
            if (MediaCenter.Singleton.IsVideoImage(swipe.CurrentPage))
            {
                MediaCenter.Singleton.SaveVideo(swipe.CurrentPage);
            }
            else
            {
                string path = MediaCenter.Singleton.GetPath(swipe.CurrentPage);
                if (path != string.Empty)
                {
                    MediaCenter.Singleton.SaveScreenShot(path);
                }
            }
        }

        public void DeleteFile()
        {
            MediaCenter.Singleton.msgDialog.Show("Delete the selected item?","Cancel", "Delete", null, DeleteImageCallback);
        }

        void DeleteImageCallback(int res, System.Object refobj)
        {
            if (res == 1)
            {
                MediaCenter.Singleton.DeleteImage(swipe.CurrentPage);
                tempImage.sprite = Instantiate(GetCurrentImage());
                tempImage.transform.localScale = Vector3.one;
                tempImage.gameObject.SetActive(true);
                LeanTween.scaleY(tempImage.gameObject, 0, 0.5f).setOnComplete(FileDeleteCompleted);
                swipe.PageCount = MediaCenter.Singleton.Images.Count;
                if (swipe.PageCount > 0)
                {
                    if (swipe.CurrentPage == swipe.PageCount)
                        GotoPage(swipe.CurrentPage - 1);
                    else
                        GotoPage(swipe.CurrentPage);
                }
            }
        }

        void FileDeleteCompleted()
        {
            tempImage.gameObject.SetActive(false);
            ClearImage(tempImage);
        }

        public override void Close()
        {
            if (MediaCenter.Singleton.imageGallery.isActiveAndEnabled)
            {
                MediaCenter.Singleton.imageGallery.DelayRefresh();
            }
            gameObject.SetActive(false);
        }

        public void ShowDetails()
        {
            string path = MediaCenter.Singleton.GetAbsolutePath(swipe.CurrentPage);
            if(path != string.Empty)
            {
                infoDlg.Open(path);
            }
        }
    }
}
