using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabButton : MonoBehaviour {

    public GameObject tabPage;
    public Image buttonImage;
    Color selectedColor;
    Color normalColor;
    TabGroup parent;
    [SerializeField]
    bool isSelected = false;
    public bool IsSelected
    {
        get { return isSelected; }
        set
        {
            isSelected = value;
            if (isSelected)
                buttonImage.color = selectedColor;
            else
                buttonImage.color = normalColor;
            tabPage.SetActive(isSelected);
        }
    }

	// Use this for initialization
	public void Init (TabGroup group) {
        parent = group;
        selectedColor = group.selectedColor;
        normalColor = group.normalColor;
	}
	

    public void OnClick()
    {
        parent.SelectTab(this);
    }
}
