using UnityEngine.UI;
using IMAV.Model;

namespace IMAV.UI
{
    public class UIProductInforDialog : UIControl
    {
        public Image productImage;
        public Text nameText;
        public Text catText;
        public Text skuText;
        public Text weightText;
        public Text timeText;
        public Text priceText;

        public void Open(Product data)
        {
            base.Open();
            productImage.sprite = data.icon;
            nameText.text = data.name;
            catText.text = data.type_id;
            skuText.text = data.sku;
            weightText.text = data.weight.ToString();
            timeText.text = data.updated_at;
            priceText.text = data.price.ToString();
        }
    }
}
