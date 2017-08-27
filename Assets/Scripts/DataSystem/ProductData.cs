using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IMAV;

public class ProductData{

    public Sprite icon;
    Product product;
    public Product ProductInfo
    {
        get { return product; }
        set { product = value; }
    }

    public GameObject model;
    bool isloaded = false;

    public ProductData(Product p)
    {
        product = p;
        //string[] strs = new string[5] { "product_dynamic_16", "product_dynamic_19", "product_dynamic_22", "product_dynamic_25", "product_dynamic_28" };
        //int id = Random.Range(0, 9);
        //if (id < 4)
        //{
        //    product.product_links.Add(strs[id]);
        //    product.product_links.Add(strs[4]);
        //    if (id == 2)
        //    {
        //        product.product_links.Add(strs[0]);
        //        product.product_links.Add(strs[1]);
        //    }
        //    if (id == 1)
        //    {
        //        product.product_links.Add(strs[3]);
        //    }
        //}
    }

    public void LoadIcon()
    {
        DataCenter.Singleton.GetProductImage(this, null);
    }

    public void Delete()
    {
        Object.Destroy(model);
        Object.Destroy(icon);
    }


}
