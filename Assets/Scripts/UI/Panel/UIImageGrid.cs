﻿using System.Collections;
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

        public void Open(DirectoryInfo dir)
        {
            isloaded = false;
            StartCoroutine(LoadDirectory(dir));
        }

        IEnumerator LoadDirectory(DirectoryInfo dir)
        {
            if (dir.Exists)
            {
                gridDir = dir;
                gridName.text = dir.Name;
                FileInfo[] files = dir.GetFiles();
                for (int i = 0; i < files.Length; i++)
                    AddItem(files[i]);
            }
            yield return new WaitForEndOfFrame();
            Refresh();
        }

        public bool EqualDirectory(string path)
        {
            return gridDir.FullName == path;
        }

        public bool IsLoaded()
        {
            return isloaded;
        }

        public float GetHeight()
        {
            return gridRect.sizeDelta.y;
        }

        public void AddItem(FileInfo f)
        {
            GameObject obj = Instantiate(imagePrefab, gridGroup.transform);
            UIImage im = obj.GetComponent<UIImage>();
            im.LoadImage(gridDir.Name, f);
            im.OnClickHandler = ClickOnImage;
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
                foreach(UIControl c in items)
                {
                    c.Refresh();
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
    }
}
