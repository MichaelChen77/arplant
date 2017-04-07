using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace IMAV.UI
{
    public class FurnitureItem : MonoBehaviour, IPointerClickHandler
    {

        public Text nameText;
        public Text categoryText;
        public Text brandText;
        public Text manuText;
        public Text placeText;
        public Text dateText;
        public Text priceText;
        public Image Thumbnail;

        FurnitureData storeData;

        public void LoadData(FurnitureData _data)
        {
            storeData = _data;
            if (_data != null)
            {
                nameText.text = _data.name;
                categoryText.text = _data.category_name;
                brandText.text = _data.brand_name;
                manuText.text = _data.manufacturer_name;
                placeText.text = _data.production_site;
                dateText.text = _data.production_date;
                priceText.text = _data.price.ToString();
                Thumbnail.sprite = _data.Thumbnail;
            }
        }

        public void OnPointerClick(PointerEventData data)
        {
            DataManager.Singleton.LoadDataModle(storeData);
        }
    }
}
