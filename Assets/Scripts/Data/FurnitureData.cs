using UnityEngine;
using System.IO;

public class FurnitureData{
    protected uint id = 0;
    public uint ID
    {
        get { return id; }
    }

    protected string name;
    public string Name
    {
        get { return name; }
    }

    protected string category;
    public string Category
    {
        get { return category; }
        set { category = value; }
    }

    protected string brand;
    public string Brand
    {
        get { return brand; }
        set { brand = value; }
    }

    protected string manufacturer;
    public string Manufacturer
    {
        get { return manufacturer; }
        set { manufacturer = value; }
    }

    protected string place;
    public string ProductionSite
    {
        get { return place; }
        set { place = value; }
    }

    protected string date;
    public string ProductionDate
    {
        get { return date; }
        set { date = value; }
    }

    protected float price;
    public float Price
    {
        get { return price; }
        set { price = value; }
    }

    public Sprite thumbnail;
    public GameObject model;

    public FurnitureData(uint _id, string _name)
    {
        id = _id;
        name = _name;
    }

    public bool CheckModelExist()
    {
        return File.Exists(DataUtility.GetLocalModelFile(this));
    }
}
