using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtomObject : MonoBehaviour {

    ARObject parent;
    Outline atomOutline;
    public Outline AtomOutline
    {
        get { return atomOutline; }
    }
    MeshRenderer render;
    public MeshRenderer Render
    {
        get { return render; }
    }
    public Vector3 originPos;

    public void Init(MeshRenderer r, ARObject p)
    {
        render = r;
        atomOutline = r.GetComponent<Outline>();
        if (atomOutline == null)
            atomOutline = r.gameObject.AddComponent<Outline>();
        originPos = r.transform.position;
    }

    public void select()
    {
        if(parent != null && parent.IsScattered)
        {

        }
    }

}
