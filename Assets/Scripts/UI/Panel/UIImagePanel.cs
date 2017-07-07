using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;

namespace IMAV.UI
{
    public class UIImagePanel : MonoBehaviour
    {
        public UISwipe swipe;

        public void Open(int index)
        {
            gameObject.SetActive(true);
            swipe.Open();
            swipe.PageCount = ImageManager.Singleton.Images.Count;
            swipe.OnSwipeCompleted = updateImages;
            

            for(int i=0; i<swipe.transform.childCount; i++)
            {
                Image img = swipe.transform.GetChild(i).GetComponent<Image>();
                img.sprite = Load(ImageManager.Singleton.GetImagePath(index + i-1));
                Debug.Log("name: " + img.name + " ; " + (index + i - 1)+img.GetComponent<RectTransform>().anchoredPosition);
            }
            swipe.CurrentPage = index;
        }

        void updateImages()
        {
            if (swipe.CurrentMoveType != UIMoveToType.None)
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
                            m.sprite = Load(ImageManager.Singleton.GetImagePath(swipe.CurrentPage + 1));
                        else
                            m.sprite = Load(ImageManager.Singleton.GetImagePath(swipe.CurrentPage - 1));
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
    }
}
