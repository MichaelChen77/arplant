using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;


public class FurnitureSet{
    List<FurnitureData> data = new List<FurnitureData>();
    public List<FurnitureData> Data
    {
        get { return data; }
        set { data = value; }
    }

    public int status { get; set; }
}
