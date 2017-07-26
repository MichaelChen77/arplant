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
    [SerializeField]
    bool trigger = true;
    public bool TriggerFlag
    {
        get { return trigger; }
    }
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
            if (trigger)
                targetImage.color = toggleOnVar.color;
            else
                targetImage.color = toggleOffVar.color;
        }
    }

    void UpdateSprite()
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
