using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class distance_calculation : MonoBehaviour {

	public InputField input;
	public InputField result;
	int i = 0;

	// Use this for initialization
	void Start () {
		if(!Input.gyro.enabled)
		{
			Input.gyro.enabled = true;
		}
		input.text = "0";
		result.text = "0";
	}
	
	// Update is called once per frame
	void Update () {
		if (i % 10 == 0) {
			int height = System.Convert.ToInt32 (input.text);
			if (height != 0) {
				float angle = 360 - Input.gyro.attitude.eulerAngles.y;
				float distance = height * Mathf.Tan (angle * 2 * Mathf.PI / 360);
				result.text = distance.ToString();
				Debug.Log ("Rotation: " + angle);
				Debug.Log ("Tan: " + Mathf.Tan (angle * 2 * Mathf.PI / 360));
			}
		}
		i = (i + 1) % 10;
	}
}
