using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabGroup : MonoBehaviour {

    public Color selectedColor;
    public Color normalColor;

    List<TabButton> items = new List<TabButton>();
    TabButton currentTab;
	// Use this for initialization
	void Start () {
        foreach(Transform tran in transform)
        {
            TabButton tb = tran.GetComponent<TabButton>();
            if (tb != null)
            {
                tb.Init(this);
                items.Add(tb);
            }
        }
        SelectTab(items[0]);
	}

    public void SelectTab(TabButton btn)
    {
        if(currentTab != btn)
        {
            if (currentTab != null)
                currentTab.IsSelected = false;
            currentTab = btn;
            currentTab.IsSelected = true;
        }
    }
}
