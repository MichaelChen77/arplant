using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialSeq_Move : MonoBehaviour {

    public RectTransform rtTarget;
    public RectTransform rt;
    void OnEnable()
    {
        
        rtTarget.localPosition = new Vector3(-150f, 0f, 0f);
            
        LeanTween.moveSplineLocal(rtTarget.gameObject, new Vector3[] { new Vector3(-150f, 0f, 0f)  , new Vector3(-150f, 0f, 0f)  , new Vector3(0f, -50f, 0f)  , new Vector3(150f, 0f, 0f)  , new Vector3(0f, 50f, 0f)  , new Vector3(-150f, 0f, 0f)  , new Vector3(-150f, 0f, 0f)   }, 3f).setLoopCount(-1);
    }
}
