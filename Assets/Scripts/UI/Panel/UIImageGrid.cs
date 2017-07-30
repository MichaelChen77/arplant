using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace IMAV.UI
{
    public class UIImageGrid : UIContainer
    {
        public Text gridName;
        public GridLayoutGroup gridGroup;
        public GameObject imagePrefab;
        public float nameHeight = 35f;

        bool isloaded = false;
        RectTransform gridRect;
        DirectoryInfo gridDir;
        public System.Action<UIImageGrid, UIImage> ItemClickedHandler;

        private void Awake()
        {
            gridRect = GetComponent<RectTransform>();
        }

        public void Load(DirectoryInfo dir)
        {
            Clear();
            UpdateCellSize();
            StartCoroutine(LoadDirectory(dir));
        }

        void UpdateCellSize()
        {
            float _x = gridGroup.cellSize.x + gridGroup.spacing.x;
            float f = gridRect.rect.width % _x;
            int num = (int)(gridRect.rect.width / _x);
            if (f < _x * 0.8f)
                num++;
            float csize = gridRect.rect.width / num - gridGroup.spacing.x;
            gridGroup.cellSize = new Vector2(csize, csize);
        }

        IEnumerator LoadDirectory(DirectoryInfo dir)
        {
            if (dir.Exists)
            {
                gridDir = dir;
                gridName.text = dir.Name;
                FileInfo[] files = dir.GetFiles();
                for (int i = 0; i < files.Length; i++)
                    AddItem(files[i], 0);
            }
            yield return new WaitForEndOfFrame();
            Refresh();
        }

        public bool EqualDirectory(string path)
        {
            return gridDir.FullName == path;
        }

        public bool EqualDirectory(DirectoryInfo d)
        {
            return gridDir == d;
        }

        public bool IsLoaded()
        {
            return isloaded;
        }

        public float GetHeight()
        {
            return gridRect.sizeDelta.y;
        }

        public void AddItem(FileInfo f, float animated, int index = -1)
        {
            GameObject obj = Instantiate(imagePrefab, gridGroup.transform);
            UIImage im = obj.GetComponent<UIImage>();
            im.LoadImage(gridDir.Name, f, animated);
            im.OnClickHandler = ClickOnImage;
            if (index != -1)
            {
                Items.Insert(index, im);
                obj.transform.SetSiblingIndex(index);
            }
            im.Parent = this;
        }

        void ClickOnImage(UIImage im)
        {
            if (ItemClickedHandler != null)
                ItemClickedHandler(this, im);
        }

        public override void Refresh()
        {
            if (ChildCount > 0)
            {
                for(int i=items.Count-1; i>-1; i--)
                {
                    items[i].Refresh();
                }
                gridRect.sizeDelta = new Vector2(gridRect.sizeDelta.x, gridGroup.preferredHeight + nameHeight);
                isloaded = true;
            }
            else
                Delete();
        }

        public void SetPosY(float _y)
        {
            gridRect.anchoredPosition = new Vector2(gridRect.anchoredPosition.x, _y);
        }

        public override void Delete()
        {
            ImageManager.Singleton.DeleteImageFolder(gridDir.Name);
            base.Delete();
           
        }

        public void Clear()
        {
            foreach(Transform tran in gridGroup.transform)
            {
                Destroy(tran.gameObject);
            }
            gridGroup.transform.DetachChildren();
        }

        public UIImage GetImage(string str)
        {
            foreach(UIControl ic in items)
            {
                UIImage im = ic as UIImage;
                if (im != null && im.ImageTag == str)
                    return im;
            }
            return null;
        }

        public UIImage GetImageByName(string str)
        {
            foreach(UIControl ic in items)
            {
                if (ic.name == str)
                    return (UIImage)ic;
            }
            return null;
        }
    }
}
