using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

        public void Open(ProductData data)
        {
            base.Open();
            productImage.sprite = data.icon;
            nameText.text = data.ProductInfo.name;
            catText.text = data.ProductInfo.type_id;
            skuText.text = data.ProductInfo.sku;
            weightText.text = data.ProductInfo.weight.ToString();
            timeText.text = data.ProductInfo.updated_at;
            priceText.text = data.ProductInfo.price.ToString();
        }
    }
}
