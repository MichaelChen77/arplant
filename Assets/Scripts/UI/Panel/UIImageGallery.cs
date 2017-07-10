using UnityEngine;
using UnityEngine.EventSystems;
using System.IO;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

namespace IMAV.UI
{
    public class UIImageGallery : MonoBehaviour
    {
        public float spaceY = 10f;
        public float moveTime = 0.5f;
        public GameObject gridPrefab;
        public RectTransform contentRect;
        public GameObject swipeObject;

        RectTransform galleryRect;
        bool isOpened = false;
        float startPos = 0;
        float topPos = 0;

        void Awake()
        {
            galleryRect = GetComponent<RectTransform>();    
        }

        void Start()
        {
            topPos = -spaceY;
            //DirectoryInfo dir = new DirectoryInfo(DataUtility.GetScreenThumbnailPath());
            DirectoryInfo dir = new DirectoryInfo(@"C:\WorkSpace\AR\TestImages\");
            StartCoroutine(LoadDirectory(dir));
        }

        IEnumerator LoadDirectory(DirectoryInfo dir)
        {
            if (dir.Exists)
            {
                DirectoryInfo[] dirs = dir.GetDirectories().OrderByDescending(d=>d.LastWriteTime).ToArray();
                for (int i = 0; i < dirs.Length; i++)
                {
                    UIImageGrid grid = AddItem(dirs[i]);
                    if (grid != null)
                    {
                        yield return new WaitUntil(grid.IsLoaded);
                        topPos -= grid.GetHeight() + spaceY;
                    }
                }
            }
            yield return null;
            Refresh();
        }

        public UIImageGrid AddItem(DirectoryInfo dir)
        {
            GameObject obj = Instantiate(gridPrefab, contentRect);
            RectTransform rect = obj.GetComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, topPos);
            UIImageGrid grid = obj.GetComponent<UIImageGrid>();
            grid.Open(dir);
            return grid;
        }

        public void Refresh()
        {
            contentRect.sizeDelta = new Vector2(contentRect.sizeDelta.x, -topPos);
        }

        public void OnBeginDrag(BaseEventData data)
        {
            startPos = galleryRect.anchoredPosition.y;
        }

        public void OnDrag(BaseEventData data)
        {
            PointerEventData p = data as PointerEventData;
            galleryRect.anchoredPosition = new Vector2(galleryRect.anchoredPosition.x, galleryRect.anchoredPosition.y + p.delta.y);
        }

        public void OnEndDrag(BaseEventData data)
        {
            float targetPos = 0;
            isOpened = true;
            if (galleryRect.anchoredPosition.y < startPos)
            {
                targetPos = startPos;
                isOpened = false;
            }
            LeanTween.moveY(galleryRect, targetPos, moveTime).setOnComplete(OnOpenCompleted).setEase(LeanTweenType.linear);
        }

        void OnOpenCompleted()
        {
            if(isOpened)
            {
                swipeObject.SetActive(false);
            }
        }

    }
}
