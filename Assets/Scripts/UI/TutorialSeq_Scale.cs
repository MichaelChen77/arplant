using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialSeq_Scale : MonoBehaviour {

    public RectTransform rtArrowL;
    public RectTransform rtArrowR;
//    public GameObject goCube;
	public RectTransform goCube;


    private void OnEnable()
    {
//        LeanTween.moveLocalX(rtArrowL.gameObject, -256f, 2f).setLoopPingPong(-1).setOnCompleteOnRepeat(true).setOnComplete(ScaleL);
//        LeanTween.moveLocalX(rtArrowR.gameObject, 256f, 2f).setLoopPingPong(-1).setOnCompleteOnRepeat(true).setOnComplete(ScaleR);
//        LeanTween.scale(goCube, new Vector3(200f, 200f, 200f),2f).setLoopPingPong(-1).setFrom(new Vector3(150f, 150f, 150f));

		LeanTween.scale(goCube.gameObject, new Vector3(1.5f, 1.5f, 1.5f),2f).setLoopPingPong(-1).setFrom(new Vector3(1f, 1f, 1f));
    }

    private void ScaleL()
    {
        rtArrowL.localScale *= -1;
    }

    private void ScaleR()
    {
        rtArrowR.localScale *= -1;
    }
}
