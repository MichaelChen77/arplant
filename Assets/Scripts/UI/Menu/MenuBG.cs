using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuBG : MonoBehaviour {

    public Image imgBG_A;
    public Image imgBG_B;

    public Sprite[] bgSprites;

    private bool boolSwap = false;
    private int bgCurrent = 0;

	// Use this for initialization
	void Start () {
        InvokeRepeating("SwapBG", 0f, 10f);
	}
	
    private void SwapBG()
    {
        if (!boolSwap)
        {
            imgBG_B.sprite = bgSprites[bgCurrent % bgSprites.Length];
            LeanTween.alpha(imgBG_A.rectTransform, 0f, 3f);
            LeanTween.alpha(imgBG_B.rectTransform, 1f, 3f);
        }
        else
        {
            imgBG_A.sprite = bgSprites[bgCurrent % bgSprites.Length];
            LeanTween.alpha(imgBG_B.rectTransform, 0f, 3f);
            LeanTween.alpha(imgBG_A.rectTransform, 1f, 3f);
        }
        boolSwap = !boolSwap;
        bgCurrent++;
    }
    
}
