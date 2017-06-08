using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IMAV.UI;

public class FrameTest : MonoBehaviour {
    public BoundFrame frame;
    public GameObject target;

    public void AddFrame()
    {
        if (frame.gameObject.activeSelf)
            frame.SetObject(null);
        else
            frame.SetObject(target);
    }
}
