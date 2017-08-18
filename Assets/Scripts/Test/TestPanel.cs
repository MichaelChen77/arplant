using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Kudan.AR;

public class TestPanel : MonoBehaviour {

    public KudanTracker _kudan;
    public Transform driver;
    public MarkerlessTransformDriver markerless;
    public Text kudanText;
    public Text driverText;
    public Text markerlessText;
    public Text arbiText;
    public Text floorText;
    public Text fullposText;
    public Text childrenText;

    Vector3 pos1, pos2;
    Quaternion rot1, rot2;
    string childStr = "";
    Transform tran;

    public void Open()
    {
        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
        kudanText.text = markerless.DiffString;
        driverText.text = driver.localPosition + " ; " + driver.localRotation;
        markerlessText.text = markerless.transform.localPosition + " ; " + markerless.transform.localRotation;
        fullposText.text = driver.position + " ; " + markerless.transform.position;
        _kudan.ArbiTrackGetPose(out pos1, out rot1);
        _kudan.FloorPlaceGetPose(out pos2, out rot2);
        arbiText.text = pos1 + " ; " + rot1;
        floorText.text = pos2 + " ; " + rot2;

        childStr = "";
        for(int i=1; i<markerless.transform.childCount; i++)
        {
            tran = markerless.transform.GetChild(i);
            if(tran != null)
            {
                childStr += tran.name + " : " + tran.localPosition + " , " + tran.localScale + " ; ";
            }
        }
        childrenText.text = childStr;
	}
}
