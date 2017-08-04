using UnityEngine;
using System.Collections;

public class CaptureAndSaveScreen : MonoBehaviour {

	CaptureAndSave snapShot ;

	string log="Log";
	void Start()
	{
		snapShot = GameObject.FindObjectOfType<CaptureAndSave>();
	}

	void OnEnable()
	{
		CaptureAndSaveEventListener.onError += OnError;
		CaptureAndSaveEventListener.onSuccess += OnSuccess;
	}

	void OnDisable()
	{
		CaptureAndSaveEventListener.onError += OnError;
		CaptureAndSaveEventListener.onSuccess += OnSuccess;
	}

	void OnError(string error)
	{
		log += "\n"+error;
		Debug.Log ("Error : "+error);
	}

	void OnSuccess(string msg)
	{
		log += "\n"+msg;
		Debug.Log ("Success : "+msg);
	}

	void OnGUI()
	{
		GUILayout.Label (log);
		if(GUI.Button(new Rect(20,300,150,50),"Save Full Screen"))
		{
			snapShot.CaptureAndSaveToAlbum(ImageType.PNG);
			//snapShot.CaptureAndSaveAtPath(System.IO.Path.Combine(Application.persistentDataPath,"Image.png"),ImageType.PNG);
		}
	}
}
