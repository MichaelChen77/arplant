using UnityEngine;
using UnityEngine.EventSystems;
using System;
using UnityEngine.UI;

namespace IMAV.UI
{
    public enum UIDirectionType
    {
        Horizontal, Vertical
    }

    public enum UIMoveToType
    {
        Previous, Next, None
    }

    public class UISwipe : UIControl, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public UIDirectionType dirType = UIDirectionType.Horizontal;
        public float moveTime = 0.3f;

        public Action OnSwipeCompleted;
        public Action OnSwipeStart;

        protected RectTransform rt;
        protected float lastTargetPos;
        protected float originRtPos;
        protected float pageSize = 900f;
        protected int curPage = 0;
        public int CurrentPage
        {
            get { return curPage; }
            set
            {
                if (curPage != value)
                {
                    curPage = value;
                    int index = curPage % transform.childCount;
                    MovePage(-index);
                }
            }
        }

        protected int pageCount = 0;
        public int PageCount
        {
            get { return pageCount; }
            set
            {
                pageCount = value;
            }
        }

        protected UIMoveToType curMove = UIMoveToType.None;
        public UIMoveToType CurrentMoveType
        {
            get { return curMove; }
        }

        private void Awake()
        {
            rt = GetComponent<RectTransform>();
            pageCount = transform.childCount;
        }

        public override void Open()
        {
            base.Open();
            rt.offsetMax = rt.offsetMin = Vector2.zero;
            if (dirType == UIDirectionType.Horizontal)
            {
                pageSize = rt.rect.width;
                rt.offsetMax = new Vector2(pageSize * (transform.childCount - 1), 0);
                SetPosX(pageSize);
                originRtPos = rt.anchoredPosition.x;
            }
            else
            {
                pageSize = rt.rect.height;
                rt.offsetMax = new Vector2(0, pageSize * (transform.childCount - 1));
                SetPosY(pageSize);
                originRtPos = rt.anchoredPosition.y;
            }
        }

        public Transform Switch(bool moveNext)
        {
            float _size = pageSize;
            Transform tran = null;
            if (moveNext)
            {
                tran = transform.GetChild(0);
                tran.SetAsLastSibling();
            }
            else
            {
                _size = -pageSize;
                tran = transform.GetChild(transform.childCount - 1);
                tran.SetAsFirstSibling();
            }
            if (dirType == UIDirectionType.Horizontal)
            {
                rt.offsetMin = new Vector2(rt.offsetMin.x + _size, 0);
                rt.offsetMax = new Vector2(rt.offsetMax.x + _size, 0);
                SetPosX(pageSize);
            }
            else
            {
                rt.offsetMin = new Vector2(0, rt.offsetMin.y + _size);
                rt.offsetMax = new Vector2(0, rt.offsetMax.x + _size);
                SetPosY(pageSize);
            }
            return tran;
        }

        void SetPosX(float _size)
        {
            foreach (Transform tran in transform)
            {
                RectTransform rect = tran.GetComponent<RectTransform>();
                rect.sizeDelta = new Vector2(_size, rect.sizeDelta.y);
                rect.anchoredPosition = new Vector2(_size * tran.GetSiblingIndex(), rect.anchoredPosition.y);
            }
            lastTargetPos = rt.anchoredPosition.x;
        }

        void SetPosY(float _size)
        {
            foreach (Transform tran in transform)
            {
                RectTransform rect = tran.GetComponent<RectTransform>();
                rect.sizeDelta = new Vector2(rect.sizeDelta.y, _size);
                rect.anchoredPosition = new Vector2(rect.anchoredPosition.y, _size * tran.GetSiblingIndex());
            }
            lastTargetPos = rt.anchoredPosition.y;
        }

        public void MovePage(int num)
        {
            float _size = num * pageSize;
            if (dirType == UIDirectionType.Horizontal)
                rt.anchoredPosition = new Vector2(originRtPos + _size, rt.anchoredPosition.y);
            else
                rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, originRtPos + _size);
        }

        public void OnBeginDrag(PointerEventData data)
        {
            if (OnSwipeStart != null)
                OnSwipeStart();
        }

        public void OnDrag(PointerEventData data)
        {
            if (dirType == UIDirectionType.Horizontal)
                rt.anchoredPosition = new Vector2(rt.anchoredPosition.x + data.delta.x, rt.anchoredPosition.y);
            else
                rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, rt.anchoredPosition.y + data.delta.y);
        }

        public void OnEndDrag(PointerEventData data)
        {
            curMove = UIMoveToType.None;
            if (dirType == UIDirectionType.Horizontal)
            {
                if (rt.anchoredPosition.x > lastTargetPos && curPage > 0)
                {
                    curPage--;
                    lastTargetPos += pageSize;
                    curMove = UIMoveToType.Previous;
                }
                else if (rt.anchoredPosition.x < lastTargetPos && curPage < (pageCount - 1))
                {
                    curPage++;
                    lastTargetPos -= pageSize;
                    curMove = UIMoveToType.Next;
                }
                LeanTween.moveX(rt, lastTargetPos, moveTime).setOnComplete(OnSwipeCompleted).setEase(LeanTweenType.linear);
            }
            else
            {
                if (rt.anchoredPosition.y > lastTargetPos && curPage > 0)
                {
                    curPage--;
                    lastTargetPos += pageSize;
                    curMove = UIMoveToType.Previous;
                }
                else if (rt.anchoredPosition.y > lastTargetPos && curPage < (PageCount - 1))
                {
                    curPage++;
                    lastTargetPos -= pageSize;
                    curMove = UIMoveToType.Next;
                }
                LeanTween.moveY(rt, lastTargetPos, moveTime).setOnComplete(OnSwipeCompleted).setEase(LeanTweenType.linear);
            }
        }

        public override void Close()
        {
            base.Close();
        }
    }
}