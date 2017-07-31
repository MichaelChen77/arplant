using UnityEngine;
using UnityEngine.UI;

    [System.Serializable]
    public struct VariableUI
    {
        public Sprite sprite;
        public Color color;
        public string text;
    }

    public enum ToggleConstraint
    {
        None, Sprite, Color, Text, SpriteAndColor, SpriteAndText, ColorAndText, All
    }

public class GToggleButton : MonoBehaviour
{
    public Image targetImage;
    public Text targetText;
    public float transferTime = 0.3f;
    [SerializeField]
    bool trigger = true;
    public bool TriggerFlag
    {
        get { return trigger; }
    }
    public bool IsAnimated = false;
    bool resetTrigger = true;
    public ToggleConstraint constraint;
    public VariableUI toggleOnVar;
    public VariableUI toggleOffVar;

    void Awake()
    {
        resetTrigger = trigger;
    }

    public void Reset()
    {
        setTrigger(resetTrigger);
    }

    public void setTrigger()
    {
        trigger = !trigger;
        UpdateToggle();
    }

    public void setTrigger(bool flag)
    {
        trigger = flag;
        UpdateToggle();
    }

    public void changeTrigger(bool flag)
    {
        if (trigger != flag)
            setTrigger(flag);
    }

    void UpdateToggle()
    {
        switch (constraint)
        {
            case ToggleConstraint.Color: UpdateColor(); break;
            case ToggleConstraint.Sprite: UpdateSprite(); break;
            case ToggleConstraint.Text: UpdateText(); break;
            case ToggleConstraint.ColorAndText: UpdateColor(); UpdateText(); break;
            case ToggleConstraint.SpriteAndColor: UpdateSprite(); UpdateColor(); break;
            case ToggleConstraint.SpriteAndText: UpdateSprite(); UpdateText(); break;
            case ToggleConstraint.All: UpdateSprite(); UpdateColor(); UpdateText(); break;
        }
    }

    void UpdateColor()
    {
        if (targetImage != null)
        {
            Color c = trigger?toggleOnVar.color:toggleOffVar.color;
            if (IsAnimated)
                LeanTween.color(targetImage.GetComponent<RectTransform>(), c, transferTime);
            else
                targetImage.color = c;
        }
    }

    void UpdateSprite()
    {
        if (IsAnimated)
            LeanTween.rotateAround(targetImage.gameObject, Vector3.forward, 180, transferTime).setOnComplete(ChangeSprite);
        else
            ChangeSprite();
    }

    void ChangeSprite()
    {
        if (targetImage != null)
        {
            if (trigger)
                targetImage.sprite = toggleOnVar.sprite;
            else
                targetImage.sprite = toggleOffVar.sprite;
        }
    }

    void UpdateText()
    {
        if (trigger)
            targetText.text = toggleOnVar.text;
        else
            targetText.text = toggleOffVar.text;
        Debug.Log("update text: " + toggleOffVar.text + " ; " + targetText.text);
    }
}
