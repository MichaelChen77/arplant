using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

namespace IMAV.UI
{
    public class ResObjectItem : MonoBehaviour, IPointerClickHandler
    {
        string currentObj;
        string category;
        public Image IconImage;
        System.Action<string, string> OnItemClick;

        public void Init(string cat, string str, Sprite sp, System.Action<string, string> itemClick)
        {
            category = cat;
            currentObj = str;
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
