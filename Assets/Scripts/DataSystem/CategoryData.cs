using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using IMAV;

public class CategoryData{
    public Sprite icon;
    Category cat;
    public Category Cat
    {
        get { return cat; }
    }

    List<CategoryProduct> products = new List<CategoryProduct>();
    public List<CategoryProduct> Products
    {
        get { return products; }
        set { products = value; }
    }

    bool isloaded = false;

    public CategoryData(Category c)
    {
        cat = c;
    }

    public void LoadIcon()
    {
        DataCenter.Singleton.GetCatImage(this);
    }

    public void LoadProducts()
    {
        DataCenter.Singleton.GetCatProduct(this);
    }

    public bool IsLoaded()
    {
        return isloaded;
    }

    public void SetLoaded(bool flag)
    {
        isloaded = flag;
    }
}
