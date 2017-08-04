using UnityEngine;
using UnityEngine.UI;

public class StatusButton : MonoBehaviour {
	public Sprite[] statusSprites;
	int buttonID = 0;
	bool defaultType = true;
	public int Status {
		get{ return buttonID; }
	}

	public int StatusCount {
		get{ return statusSprites.Length; }
	}

	Image btnImage;

	// Use this for initialization
	void Awake () {
		btnImage = GetComponent <Image> ();
		SetStatus (buttonID);
	}

	public void Switch(bool flag)
	{
		defaultType = flag;
	}

	public void SetStatus(int _id)
	{
		buttonID = _id;
		if (_id < StatusCount) {
			int rid = _id;
			if (!defaultType) {
				switch (_id) {
				case 1:
					rid = 3;
					break;
				case 2:
					rid = 1;
					break;
				case 3:
					rid = 2;
					break;
				}
			}
			btnImage.sprite = statusSprites [rid];
		}
	}

	public void SetNextStatus()
	{
		buttonID++;
		if (buttonID >= StatusCount)
			buttonID = 0;
		SetStatus (buttonID);
	}
}
