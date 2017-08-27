using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraFlash : MonoBehaviour {

    public Image imgFlash;

    private void OnEnable()
    {
        LeanTween.alpha(imgFlash.rectTransform, 0.8f, 0.2f).setFrom(0f).setLoopPingPong(1).setOnComplete(Hide);
    }

    private void Hide()
    {
        this.gameObject.SetActive(false);
    }
}
