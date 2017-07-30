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
        public Image tempImage;

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
                ClearImage(m);
                if (flag)
                {
                    m.sprite = DataUtility.CreateSprite(ImageManager.Singleton.GetImagePath(swipe.CurrentPage + 1));
                    DetectVideo(m, swipe.CurrentPage + 1);
                }
                else
                {
                    m.sprite = DataUtility.CreateSprite(ImageManager.Singleton.GetImagePath(swipe.CurrentPage - 1));
                    DetectVideo(m, swipe.CurrentPage - 1);
                }
                AutosetImageColor(m);
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
                ClearImage(img);
                img.sprite = DataUtility.CreateSprite(ImageManager.Singleton.GetImagePath(index + i + changedInt));
                DetectVideo(img, index + i + changedInt);
                AutosetImageColor(img);
            }
        }

        public void DetectVideo(Image m, string str)
        {
            if (ImageManager.Singleton.IsVideoImage(str))
                m.transform.GetChild(0).gameObject.SetActive(true);
            else
                m.transform.GetChild(0).gameObject.SetActive(false);
        }

        public void DetectVideo(Image m, int index)
        {
            string str = ImageManager.Singleton.GetPath(index);
            DetectVideo(m, str);
        }

        public void PlayVideo()
        {
            string str = ImageManager.Singleton.GetPath(swipe.CurrentPage);
            ImageManager.Singleton.PlayVideo(str);
            //string fileName = ImageManager.Singleton.GetVideoFileName(str);
            //string path = File.ReadAllText(DataUtility.GetScreenVideoPath() + fileName + ".json");
            //ResourceManager.Singleton.DebugString("play: " + path);
            //List<System.Object> videos = EveryplayMiniJSON.Json.Deserialize(path) as List<System.Object>;
            //if (videos.Count == 1)
            //{
            //    foreach (Dictionary<string, object> video in videos)
            //    {
            //        Everyplay.PlayVideoWithDictionary(video);
            //    }
            //}
        }

        void ClearImage(Image img)
        {
            if (img.sprite != null)
                Destroy(img.sprite.texture);
            Destroy(img.sprite);
        }

        void AutosetImageColor(Image img)
        {
            if (img.sprite == null)
                img.color = Color.clear;
            else
                img.color = Color.white;
        }

        public void SaveFile()
        {
            if (ImageManager.Singleton.IsVideoImage(swipe.CurrentPage))
            {
                ImageManager.Singleton.SaveVideo(swipe.CurrentPage);
            }
            else
            {
                string path = ImageManager.Singleton.GetPath(swipe.CurrentPage);
                ResourceManager.Singleton.DebugString("save file: " + path);
                if (path != string.Empty)
                {
                    ImageManager.Singleton.SaveScreenShot(path);
                }
            }
        }

        public void DeleteFile()
        {
            ImageManager.Singleton.msgDialog.Show("Delete the selected item?","Cancel", "Delete", null, DeleteImageCallback);
        }

        void DeleteImageCallback(int res, System.Object refobj)
        {
            if (res == 1)
            {
                ImageManager.Singleton.DeleteImage(swipe.CurrentPage);
                tempImage.sprite = Instantiate(GetCurrentImage());
                tempImage.transform.localScale = Vector3.one;
                tempImage.gameObject.SetActive(true);
                LeanTween.scaleY(tempImage.gameObject, 0, 0.5f).setOnComplete(FileDeleteCompleted);
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

        void FileDeleteCompleted()
        {
            tempImage.gameObject.SetActive(false);
            ClearImage(tempImage);
        }

        public override void Close()
        {
            if (ImageManager.Singleton.imageGallery.isActiveAndEnabled)
            {
                ImageManager.Singleton.imageGallery.DelayRefresh();
            }
            gameObject.SetActive(false);
            //ResourceManager.Singleton.Resume();
        }
    }
}
