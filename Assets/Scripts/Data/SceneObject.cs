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

    public void Init(bool _islocal, int _id, string str)
    {
        id = _id;
        isLocal = _islocal;
        localID = str;
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
}
