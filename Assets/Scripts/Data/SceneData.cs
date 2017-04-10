﻿using System;
using System.Collections.Generic;
using UnityEngine;

public class SceneObjectData
{
    protected string id;
    public string ID
    {
        get { return id; }
    }

    public int matID = -1;
    public int MaterialID
    {
        get { return matID; }
        set { matID = value; }
    }

    public Vector3 position;
    public Vector3 rotation;
    public Vector3 scale;

    public SceneObjectData(string _id)
    {
        id = _id;
    }

    public SceneObjectData(string _id, Vector3 pos, Vector3 rot, Vector3 sc)
    {
        id = _id;
        position = pos;
        rotation = rot;
        scale = sc;
    }

    public SceneObjectData(string _id, Vector3 pos, Vector3 rot, Vector3 sc, int _mat)
    {
        id = _id;
        position = pos;
        rotation = rot;
        scale = sc;
        matID = _mat;
    }

    public bool isLocal()
    {
        if (id.Contains("-"))
            return true;
        return false;
    }
}

public class SceneData{

    public int id { get; set; }
    public int updated_at { get; set; }
    string sceneValue;
    public string raw_design
    {
        get { return sceneValue; }
        set { sceneValue = value; LoadData(sceneValue); }
    }
    public int created_at { get; set; }

    public string Name
    {
        get { return "Scene - "+id; }
    }

    protected Sprite icon;
    public Sprite Icon
    {
        get { return icon; }
        set { icon = value; }
    }

    public int ModelNum
    {
        get { return objects.Count; }
    }

    protected List<SceneObjectData> objects = new List<SceneObjectData>();
    public List<SceneObjectData> Objects
    {
        get { return objects; }
    }

    public void LoadData(string content)
    {
        try
        {
            objects.Clear();
            string[] strs = content.Split(';');
            if (strs.Length > 0)
            {
                for(int i=0; i< strs.Length; i++)
                {
                    if (strs[i] != "")
                    {
                        string[] subStr = strs[i].Split(',');
                        
                        Vector3 pos = DataUtility.ConvertToVector3(subStr[1], subStr[2], subStr[3]);
                        Vector3 rot = DataUtility.ConvertToVector3(subStr[4], subStr[5], subStr[6]);
                        Vector3 sca = DataUtility.ConvertToVector3(subStr[7], subStr[8], subStr[9]);
                        int _id = Convert.ToInt32(subStr[10]);
                        SceneObjectData obj = new SceneObjectData(subStr[0], pos, rot, sca, _id);
                        objects.Add(obj);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.Log("error: " + ex.Message);
        }
    }

    //public string ToDataString()
    //{
    //    string str = string.Format("{0},{1};", name, datetime);
    //    for(int i=0; i<objects.Count; i++)
    //    {
    //        str += string.Format("{0},{1},{2},{3},{4};", objects[i].ID, objects[i].position.ToString(), objects[i].rotation.ToString(), objects[i].scale.ToString(), objects[i].MaterialID);
    //    }
    //    return str;
    //}
}
