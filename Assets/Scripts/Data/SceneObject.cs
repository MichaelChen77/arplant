using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneObject : MonoBehaviour {

    string id = string.Empty;
    public string ID
    {
        get { return id; }
    }

    public int materialID = -1;
    List<MeshRenderer> renders = new List<MeshRenderer>();
    List<Material[]> originMt = new List<Material[]>();

    [SerializeField]
    Vector3 originScale = Vector3.one;
	public Vector3 OriginalScale
	{
		get{ return originScale; }
	}
    [SerializeField]
    Quaternion originRotation = Quaternion.identity;
	public Quaternion OriginalRotation
	{
		get{ return originRotation; }
	}

    public void Init(string _id)
	{
        id = _id;
        InitObject();
        //InitCollider();
    }

    public void ResumeTransform()
    {
        transform.localScale = originScale;
        transform.localRotation = originRotation;
    }

	public void SetTransform(Vector3 _scale, Quaternion _rot)
	{
		originScale = _scale;
		originRotation = _rot;
		transform.localScale = originScale;
		transform.localRotation = originRotation;
	}

    public string ToDataString()
    {
        string idstr = id.ToString();
        string str = string.Format("{0},{1},{2},{3},{4};", idstr, VectorToString(transform.localPosition), VectorToString(transform.localEulerAngles), VectorToString(transform.localScale), materialID);
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
            if (gameObject.GetComponent<MeshRenderer>() == null)
            {
                foreach (MeshRenderer mr in renders)
                {
                    mr.gameObject.AddComponent<MeshCollider>();
                }
            }
            else
                gameObject.AddComponent<MeshCollider>();
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
        originScale = transform.localScale;
        originRotation = transform.rotation;
        renders.Clear();
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
