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
                nameText.text = _data.Name;
                categoryText.text = _data.Category;
                brandText.text = _data.Brand;
                manuText.text = _data.Manufacturer;
                placeText.text = _data.ProductionSite;
                dateText.text = _data.ProductionDate;
                priceText.text = _data.Price.ToString();
                Thumbnail.sprite = _data.thumbnail;
            }
        }

        public void OnPointerClick(PointerEventData data)
        {
            DataManager.Singleton.LoadDataModle(storeData);
        }
    }
}
