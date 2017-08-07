using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.IO;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

namespace IMAV.UI
{
    public class UIImageGallery : UIContainer
    {
        public float spaceY = 10f;
        public float moveTime = 0.5f;
        public Text selectBtnText;
        public GameObject gridPrefab;
        public RectTransform contentRect;
        public RectTransform header;
        public RectTransform bodyRect;
        public RectTransform bottomRect;

        RectTransform galleryRect;
        List<UIImage> selectedImages = new List<UIImage>();
        bool isSelectState = false;
        float topPos = 0;

        void Awake()
        {
            galleryRect = GetComponent<RectTransform>();
        }

        public override void Open()
        {
            ResourceManager.Singleton.Pause();
            base.Open();
            topPos = -spaceY;
            DirectoryInfo dir = new DirectoryInfo(DataUtility.GetScreenThumbnailPath());
            StartCoroutine(LoadDirectory(dir));
        }

        public override void Close()
        {
            base.Close();
            ResourceManager.Singleton.Resume();
        }

        IEnumerator LoadDirectory(DirectoryInfo dir)
        {
            if (dir.Exists)
            {
                DirectoryInfo[] dirs = dir.GetDirectories().OrderByDescending(d=>d.LastWriteTime).ToArray();
                for (int i = 0; i < items.Count; i++)
                {
                    UIImageGrid ug = items[i] as UIImageGrid;
                    if (!ug.EqualDirectory(dirs[i]))
                    {
                        ug.Load(dirs[i]);
                    }
                }
                if (items.Count < dirs.Length)
                {
                    for(int i = items.Count; i<dirs.Length; i++)
                    {
                        UIImageGrid grid = AddItem(dirs[i]);
                        if (grid != null)
                        {
                            yield return new WaitUntil(grid.IsLoaded);
                            topPos -= grid.GetHeight() + spaceY;
                        }
                    }
                }
                topPos -= bottomRect.sizeDelta.y;
            }
            yield return new WaitForEndOfFrame();
            Refresh();
        }

        public UIImageGrid GetItem(string path)
        {
            UIImageGrid grid = null;
            foreach (UIControl c in items)
            {
                grid = c as UIImageGrid;
                if (grid.EqualDirectory(path))
                    return grid;
            }
            return null;
        }

        public void AddImage(string str)
        {
            FileInfo file = new FileInfo(str);
            if (file.Exists)
            {
                UIImageGrid grid = GetItem(file.DirectoryName);
                if (grid == null)
                {
                    grid = AddItem(file.Directory, 0);
                }
                else
                    grid.AddItem(file, 0.4f, 0);
            }
            DelayRefresh();
        }

        public UIImageGrid AddItem(DirectoryInfo dir, int index = -1)
        {
            GameObject obj = Instantiate(gridPrefab, contentRect);
            UIImageGrid grid = obj.GetComponent<UIImageGrid>();
            grid.SetPosY(topPos);
            grid.Load(dir);
            grid.ItemClickedHandler = OnClickImage;
            if (index != -1)
            {
                Items.Insert(index, grid);
                obj.transform.SetSiblingIndex(index);
            }
            grid.Parent = this;
            return grid;
        }

        public void DelayRefresh(float f = 0)
        {
            StartCoroutine(RefreshItor(f));
        }

        IEnumerator RefreshItor(float delayTime)
        {
            if (delayTime == 0)
                yield return new WaitForEndOfFrame();
            else
                yield return new WaitForSeconds(delayTime);
            Refresh();
        }

        public override void Refresh()
        {
            topPos = -spaceY;
            for (int i = items.Count - 1; i > -1; i--)
                ((UIImageGrid)items[i]).Refresh();
            for(int i=0; i<items.Count; i++)
            {
                UIImageGrid _grid = items[i] as UIImageGrid;
                _grid.SetPosY(topPos);
                topPos -= _grid.GetHeight() + spaceY;
            }
            topPos -= bottomRect.sizeDelta.y;
            UpdateSize();
        }

        public void UpdateSize()
        {
            if (topPos + Screen.height > 0)
                topPos = -Screen.height;
            contentRect.sizeDelta = new Vector2(contentRect.sizeDelta.x, -topPos);
        }

        public void OnSelectClick()
        {
            isSelectState = !isSelectState;
            if(isSelectState)
            {
                selectBtnText.text = "Cancel";
            }
            else
            {
                selectBtnText.text = "Select";
                UnSelect();
            }
            bottomRect.gameObject.SetActive(isSelectState);
        }

        void UnSelect()
        {
            foreach(UIImage im in selectedImages)
            {
                im.Selected = false;
            }
            selectedImages.Clear();
        }

        public void RemoveSelectedImages()
        {
            for(int i=selectedImages.Count-1; i>-1; i--)
            {
                selectedImages[i].Delete(true);
            }
            selectedImages.Clear();
            OnSelectClick();
            DelayRefresh();
        }

        public void OnClickImage(UIImageGrid grid, UIImage im)
        {
            if(isSelectState)
            {
                im.Selected = !im.Selected;
                if (im.Selected)
                    selectedImages.Add(im);
                else
                    selectedImages.Remove(im);
            }
            else
            {
                string path = DataUtility.GetScreenShotPath() + im.ImageTag;
                MediaCenter.Singleton.ShowScreenShot(im.ImageTag);
            }
        }


        public UIImage GetImage(string str)
        {
            UIImageGrid ug = GetImageGrid(DataUtility.GetPathName(str));
            if(ug != null)
            {
                return ug.GetImage(str);
            }
            return null;
        }

        public UIImageGrid GetImageGrid(string str)
        {
            foreach (UIControl ic in items)
            {
                UIImageGrid ug = ic as UIImageGrid;
                if (ug != null && ug.gridName.text == str)
                    return ug;
            }
            return null;
        }

    }
}
