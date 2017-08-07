using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class sliderTest : MonoBehaviour {

    public Slider slider;
    public Text text;

    public void UpdateSliderValue(float f)
    {
        text.text = f.ToString();
    }


}
