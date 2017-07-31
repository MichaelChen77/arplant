using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

namespace IMAV.UI
{
    public class ResObjectItem : MonoBehaviour, IPointerClickHandler
    {
        string category;
        System.Object currentObj;
        public Image IconImage;
        System.Action<string, System.Object> OnItemClick;

        public void Init(string cat, System.Object obj, Sprite sp, System.Action<string, System.Object> itemClick)
        {
            category = cat;
            currentObj = obj;
            IconImage.sprite = sp;
            OnItemClick = itemClick;
        }

        public void OnPointerClick(PointerEventData data)
        {
            if (OnItemClick != null)
                OnItemClick(category, currentObj);
        }
    }
}
