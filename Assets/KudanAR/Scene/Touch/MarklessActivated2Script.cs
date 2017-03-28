using UnityEngine;
using System.Collections;

/// <summary>
/// Used in the touch sample. Tapping the screen in that sample activates this script.
/// </summary>
public class MarklessActivated2Script : MonoBehaviour 
{
	void Update()
	{
		GameObject mobj1 = GameObject.Find ("MLobject1");
		GameObject mobj2 = GameObject.Find ("MLobject2");
		GameObject mobj3 = GameObject.Find ("MLobject3");
		// All this script does for this sample is slowly spin the object it is attached to
		//this.transform.Rotate (0, 1, 0);
		if (mobj1.GetComponent<MarklessActivated2Script> ().enabled == true) {
			mobj1.transform.Rotate (0, -1, 0);
		} else if (mobj2.GetComponent<MarklessActivated2Script> ().enabled == true) {
			mobj2.transform.Rotate (0, 0, -1);
		} else if (mobj3.GetComponent<MarklessActivated2Script> ().enabled == true){
			mobj3.transform.Rotate (0, 0, -1);
		}
	}
}