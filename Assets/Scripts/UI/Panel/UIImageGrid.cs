using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace IMAV.UI
{
    public class UIImageGrid : MonoBehaviour
    {
        public Text gridName;
        public GridLayoutGroup gridGroup;
        public GameObject imagePrefab;
        public float nameHeight = 35f;

        bool isloaded = false;

        RectTransform gridRect;

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
                gridName.text = dir.Name;
                FileInfo[] files = dir.GetFiles();
                for (int i = 0; i < files.Length; i++)
                    AddItem(files[i]);
            }
            yield return new WaitForEndOfFrame();
            Refresh();
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
            im.LoadImage(f);
            im.OnClickHandler = ClickOnImage;
        }

        public void ClickOnImage(UIImage im)
        {
            Debug.Log("Open image: " + im.ImageTag);
        }

        public void AddItemRefresh(FileInfo f)
        {
            AddItem(f);
            Refresh();
        }

        public void Refresh()
        {
            gridRect.sizeDelta = new Vector2(gridRect.sizeDelta.x, gridGroup.preferredHeight + nameHeight);
            isloaded = true;
        }


    }
}
