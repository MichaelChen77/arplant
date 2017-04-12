using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneObject : MonoBehaviour {

    int id = -1;
    public int ID
    {
        get { return id; }
    }
    bool isLocal = false;
    public bool IsLocal
    {
        get { return isLocal; }
    }
    string localID = "";
    public string LocalID
    {
        get { return localID; }
    }

    public int materialID = -1;
    List<MeshRenderer> renders = new List<MeshRenderer>();
    List<Material[]> originMt = new List<Material[]>();

    public void Init(bool _islocal, int _id, string str)
    {
        id = _id;
        isLocal = _islocal;
        localID = str;
        InitObject();
        InitCollider();
    }

    public string ToDataString()
    {
        string idstr = isLocal ? localID : id.ToString();
        string str = string.Format("{0},{1},{2},{3},{4};", idstr, VectorToString(transform.position), VectorToString(transform.eulerAngles), VectorToString(transform.localScale), materialID);
        return str;
    }

    public string VectorToString(Vector3 vec)
    {
        return vec.x + "," + vec.y + "," + vec.z;
    }

    public void InitCollider()
    {
        if (gameObject.GetComponent<Collider>() == null)
        {
            foreach (MeshRenderer mr in renders)
            {
                mr.gameObject.AddComponent<BoxCollider>();
            }
        }
    }

    public void SetMaterial(int ID, Material mt)
    {
        if (renders.Count == 0)
            InitObject();
        foreach (MeshRenderer mr in renders)
        {
            mr.material = mt;
        }
        materialID = ID;
    }

    public void ResumeMaterial()
    {
        for(int i=0; i<renders.Count; i++)
        {
            renders[i].materials = originMt[i];
        }
        materialID = -1;
    }

    public Material GetMaterial()
    {
        if (renders.Count == 0)
            InitObject();
        foreach (MeshRenderer mr in renders)
        {
            if (mr.material != null)
                return mr.material;
        }
        return null;
    }

    void InitObject()
    {
        renders.Clear();
        //        MeshRenderer render = GetComponent<MeshRenderer>();
        //        if (render != null)
        //        {
        //            Outline ot = render.gameObject.AddComponent<Outline>();
        //            ot.enabled = false;
        //            render.gameObject.AddComponent<MeshCollider>();
        //            OutlineRender outRender = new OutlineRender(ot, render, render.transform.position);
        //            renders.Add(outRender);
        //        }
        MeshRenderer[] childRenders = GetComponentsInChildren<MeshRenderer>();
        if (childRenders != null)
        {
            foreach (MeshRenderer mr in childRenders)
            {
                mr.gameObject.AddComponent<MeshCollider>();
                originMt.Add(mr.materials);
                renders.Add(mr);
            }
        }
    }
}
