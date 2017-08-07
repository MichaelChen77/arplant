using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LightingTest : MonoBehaviour {

    public Slider BiasSlider;
    public Slider NBiasSlider;
    public Slider NearPlaneSlider;
    public Slider StrengthSlider;
    public Light targetLight;

    public void SetValue()
    {
        targetLight.shadowBias = BiasSlider.value;
        targetLight.shadowNormalBias = NBiasSlider.value;
        targetLight.shadowNearPlane = NearPlaneSlider.value;
        targetLight.shadowStrength = StrengthSlider.value;
    }

    public void SetShadowType(bool flag)
    {
        if (flag)
            targetLight.shadows = LightShadows.Hard;
        else
            targetLight.shadows = LightShadows.Soft;
    }
}
