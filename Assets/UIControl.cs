using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIControl : MonoBehaviour {

	// Use this for initialization
	void Start () {
		GameObject dontDestroy = GameObject.Find ("DontDestroyModels");
	}
	
	// Update is called once per frame
	void Update () {
	}

	public void back()
	{
		UIManager.Singleton.UnSelect ();
		SceneManager.LoadScene("Kudan");
	}
}
