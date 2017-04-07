using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelData {

    public string updateTime;
    public float leghth;
    public float width;
    public float height;

    protected GameObject model;
    public GameObject Model
    {
        get { return model; }
        set { model = value; }
    }  
}
