﻿using UnityEngine;
using System.Collections;

public class SpinUI : MonoBehaviour {

    bool start = false;
    int _count = 0;
    public bool SpinOnAwake = false;
    RectTransform rect;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        if (SpinOnAwake)
            Show();
    }

    public void Show()
    {
        gameObject.SetActive(true);
        transform.rotation = Quaternion.identity;
        start = true;
    }

    //public void Show(RectTransform tran)
    //{
    //    transform.SetParent(tran);
    //    transform.SetAsLastSibling();
    //    rect.anchoredPosition = tran.rect.center;
    //    Show();
    //}

    public void Hide()
    {
        start = false;
        gameObject.SetActive(false);
    }

    void FixedUpdate()
    {
        if (start)
        {
            _count++;
            if (_count == 36)
            {
                transform.rotation = Quaternion.identity;
                _count = 0;
            }
            else if(_count % 3 == 0)
            {
                transform.Rotate(Vector3.forward, -30f);
            }
        }
    }
}
