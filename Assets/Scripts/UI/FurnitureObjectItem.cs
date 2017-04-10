using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class FurnitureObjectItem : MonoBehaviour, IPointerClickHandler {

    public Image IconImage;
    public Text objectText;
    ResObject currentObj;
    System.Action OnItemClick;

    public void Init(ResObject obj, System.Action itemClick)
    {
        currentObj = obj;
        objectText.text = obj.name;
        IconImage.sprite = obj.thumbnail;
        OnItemClick = itemClick;
    }

    public void OnPointerClick(PointerEventData data)
    {
        string str = currentObj.type + "-" + currentObj.name;
        ResourceManager.Singleton.LoadGameObject(currentObj.resource, str);
        if (OnItemClick != null)
            OnItemClick();
    }
}
