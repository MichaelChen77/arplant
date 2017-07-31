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
    }

    public void LoadIcon()
    {
        DataCenter.Singleton.GetProductImage(this, null);
    }
}
