using UnityEngine;
using System.Collections;

public class PrintTestData : MonoBehaviour {
	ScreenOrientation originSOrientation;
	// Use this for initialization
	void Start () {
		originSOrientation = Screen.orientation;
		InvokeRepeating("PrintData", 0.5f, 3f);
	}
	
	// Update is called once per frame
	void Update () {
		PrintDataOnce();
	}

	void PrintDataOnce()
	{
		if (originSOrientation != Screen.orientation) {
			Debug.Log ("# prev: " + Screen.orientation + " ; now: " + Screen.orientation);
			originSOrientation = Screen.orientation;
		}
	}

	void PrintData () {
		float[] result1 = new float[7];
		Kudan.AR.NativeInterface.ArbiTrackGetPose (result1);
		Quaternion ARBorientation = new Quaternion (result1 [3], result1 [4], result1 [5], result1 [6]); // The current orientation of the floor in 3D space, relative to the devic
		Vector3 rotation = ARBorientation.eulerAngles;
		Debug.Log("# current orientation: "+Screen.orientation + " ; rot1: "+rotation+" ; rot: "+transform.eulerAngles);
	}
}
