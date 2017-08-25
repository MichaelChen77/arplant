using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialSeq_Rotate : MonoBehaviour {

    public Image imgFinger;
    public Transform tCube;

    void OnEnable()
    {
        LeanTween.moveX(imgFinger.rectTransform, 100f, 2f).setFrom(-100f).setLoopPingPong(-1).setEaseInOutQuad();
        LeanTween.rotateLocal(tCube.gameObject, new Vector3(45f, 0f, 135f), 2f).setFrom(new Vector3(45f, 0f, 45f)).setLoopPingPong(-1).setEaseInOutQuad();
    }
}
