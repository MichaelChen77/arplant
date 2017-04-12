using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class ToggleButton : MonoBehaviour, IPointerClickHandler {
	public Sprite onSprite;
	public Sprite offSprite;
	public Color ignoreColor;
	Color originColor;
    [SerializeField]
	bool isOn = true;
	public bool IsOn
	{
		get{ return isOn; }
	}
	Image btnImage;
	bool ignore = false;
	public bool Ignore {
		get{ return ignore; }
		set {
			ignore = value;
			if (ignore)
				btnImage.color = ignoreColor;
			else
				btnImage.color = originColor;
		}
	}

	public delegate void RecBoolCallback(bool flag);
	public RecBoolCallback onToggleClick;

	// Use this for initialization
	void Awake () {
		btnImage = GetComponent <Image> ();
		originColor = btnImage.color;
	}

	public void SetToggle(bool flag)
	{
		isOn = flag;
		if (isOn)
			btnImage.sprite = onSprite;
		else
			btnImage.sprite = offSprite;
	}
		
	public void OnPointerClick(PointerEventData data)
	{
		if (ignore)
			Ignore = false;
		else {
			SetToggle (!isOn);
			if (onToggleClick != null) {
				onToggleClick (isOn);
			}
		}
	}
}
