using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugView : MonoBehaviour {

    public Text debugText;
    public RectTransform content;
    public Scrollbar bar;
    public bool alwayShowBottom = false;

    public void Open()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }

    public void AppendText(string str)
    {
        debugText.text += str;
        UpdateContentSize();
    }

    public void Log(string str)
    {
        debugText.text += "\n" + str;
        UpdateContentSize();
    }

    public void SetTextLog(string str)
    {
        debugText.text = str;
        UpdateContentSize();
    }

    void UpdateContentSize()
    {
        content.sizeDelta = new Vector2(content.sizeDelta.x, debugText.preferredHeight);
        if (alwayShowBottom)
            bar.value = 0;
    }

    public void Clear()
    {
        debugText.text = "";
        UpdateContentSize();
    }
}
