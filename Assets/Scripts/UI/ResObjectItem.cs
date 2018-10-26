using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

namespace IMAV.UI
{
    public class ResObjectItem : MonoBehaviour, IPointerClickHandler
    {
        string category;
		long subCatID;
        System.Object currentObj;
        public Image IconImage;
        System.Action<string, System.Object> OnItemClick;
		System.Action<long, System.Object> OnItemClick_long;

        public void Init(string cat, System.Object obj, Sprite sp, System.Action<string, System.Object> itemClick)
        {
            category = cat;
            currentObj = obj;
            IconImage.sprite = sp;
            OnItemClick = itemClick;
        }

		public void Init(long SubCatID, System.Object obj, Sprite sp, System.Action<long, System.Object> itemClick_long)
		{
			subCatID = SubCatID;
			currentObj = obj;
			IconImage.sprite = sp;
			OnItemClick_long = itemClick_long;
		}

        public void OnPointerClick(PointerEventData data)
        {
            if (OnItemClick != null)
                OnItemClick(category, currentObj);
			if (OnItemClick_long != null)
				OnItemClick_long(subCatID, currentObj);
        }
    }
}
