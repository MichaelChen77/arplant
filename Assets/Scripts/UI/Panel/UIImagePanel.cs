using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;

namespace IMAV.UI
{
    public class UIImagePanel : MonoBehaviour
    {
        public UISwipe swipe;
        public Image imageOne;
        public Image imageTwo;
        public Image imageThree;
        public float prevThold;
        public float nextThold;

        List<FileInfo> files = new List<FileInfo>();
        int currentIndex;
        float dragDistance = 0;
        bool startDrag = false;
        string prevfile = "";

        public void Open(int index)
        {
            gameObject.SetActive(true);
            swipe.Open();
            swipe.PageCount = ImageManager.Singleton.Images.Count;
            swipe.OnSwipeCompleted = updateImages;
            swipe.CurrentPage = index;
        }

        void updateImages()
        {
            if(swipe.CurrentMoveType != UIMoveToType.None)
            {
                if (swipe.CurrentPage != 0 && swipe.CurrentPage != (swipe.PageCount - 1))
                {
                    bool flag = (swipe.CurrentMoveType == UIMoveToType.Next);
                    Transform tran = swipe.Switch(flag);
                    if (tran != null)
                    {
                        Image m = tran.GetComponent<Image>();
                        Destroy(m.sprite);
                        if (flag)
                            m.sprite = Load(ImageManager.Singleton.Images[swipe.CurrentPage + 1]);
                        else
                            m.sprite = Load(ImageManager.Singleton.Images[swipe.CurrentPage - 1]);
                    }
                }
            }
        }

        public void Close()
        {
            gameObject.SetActive(false);
            ResourceManager.Singleton.Resume();
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
                        return DataUtility.CreateSprit(content);
                    }
                }
                catch { }
            }
            return null;
        }

        int getFileIndex(string path)
        {
            for (int i = 0; i < files.Count; i++)
            {
                if (files[i] == path)
                    return i;
            }
            return 0;
        }

        public void LoadNext()
        {
            currentIndex++;
            if (currentIndex >= files.Count)
                currentIndex = 0;
            Load(files[currentIndex]);
        }

        public void LoadPrev()
        {
            currentIndex--;
            if (currentIndex < 0)
                currentIndex = files.Count - 1;
            Load(files[currentIndex]);
        }

        public void OnPointerClick(PointerEventData data)
        {
            header.SetActive(!header.activeSelf);
        }

        public void OnBeginDrag(PointerEventData data)
        {
            dragDistance = 0;
            startDrag = true;
        }

        public void OnDrag(PointerEventData data)
        {
            if (startDrag)
            {
                dragDistance += data.delta.x;
                if (dragDistance > nextThold)
                {
                    LoadNext();
                    startDrag = false;
                }
                else if (dragDistance < prevThold)
                {
                    LoadPrev();
                    startDrag = false;
                }
            }
        }

        public void OnEndDrag(PointerEventData data)
        {
            startDrag = false;
        }
    }
}
