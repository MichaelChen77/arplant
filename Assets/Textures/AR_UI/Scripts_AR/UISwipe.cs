using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class UISwipe : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public ToggleGroup tgPages;
    private RectTransform rt;
    private float startPos;
    private float dragPos;
    private float rtX;
    private float rtOriX;

    private int totalPages;
    private int curPage;
    private Toggle[] listToggles;
    private float oriXPos;

    public float pageWidth = 900f;

    private void Awake()
    {
        rt = GetComponent<RectTransform>();
        rtX = rtOriX = rt.anchoredPosition.x;
        listToggles = tgPages.GetComponentsInChildren<Toggle>();
        totalPages = listToggles.Length; 
        curPage = 1;
        oriXPos = rt.anchoredPosition.x;
        Debug.Log(rt.anchoredPosition);
    }

    public void OnBeginDrag(PointerEventData data)
    {
        startPos = data.position.x;
    }

    public void OnDrag(PointerEventData data)
    {
        dragPos = data.position.x;
        rt.anchoredPosition = new Vector2(rtX + (dragPos - startPos), rt.anchoredPosition.y);
    }

    public void OnEndDrag(PointerEventData data)
    {
        rtX = rt.anchoredPosition.x;
        if (rtX > rtOriX)
        {
            Debug.Log("Swipe Left, Prev Page!");
            if (curPage > 1)
            {
                curPage--;
            }
        }
        else
        {
            Debug.Log("Swipe Right, Next Page!");
            if (curPage < totalPages)
            {
                curPage++;
            }
        }

        //Tween to page accordingly and update togglegroup.
        LeanTween.moveX(rt, oriXPos + (-pageWidth * (curPage - 1)), 0.3f).setOnComplete(UpdatePos);
    }

    private void UpdatePos()
    {
        for(int i = 0; i < listToggles.Length; i++)
        {
            if(listToggles[i].name.Contains("" + curPage))
            {
                listToggles[i].isOn = true;
            }
        }
        rtX = rtOriX = rt.anchoredPosition.x;
    }

    public void ResetPages()
    {
        curPage = 1;
        for (int i = 0; i < listToggles.Length; i++)
        {
            if (listToggles[i].name.Contains("" + curPage))
            {
                listToggles[i].isOn = true;
            }
        }
        rt.anchoredPosition = new Vector2(oriXPos, 0f);
        rtX = rtOriX = rt.anchoredPosition.x;
    }
}