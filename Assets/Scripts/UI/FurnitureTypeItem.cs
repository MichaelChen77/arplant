using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class FurnitureTypeItem : MonoBehaviour,  IPointerClickHandler{

    public Image TypeImage;
    public Text TypeText;
    public InsertForm form;

    public void OnPointerClick(PointerEventData data)
    {
        //ResType _type = (ResType)System.Enum.Parse(typeof(ResType), TypeText.text);
        //form.ShowObjects(_type);
    }
}
