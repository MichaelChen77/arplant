using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IMAV.Effect;

public class OutlineRender
{
    public Outline outline;
    public MeshRenderer render;
    public Vector3 originPos;

    public OutlineRender(Outline l, MeshRenderer r, Vector3 pos)
    {
        outline = l;
        render = r;
        originPos = pos;
    }

    public OutlineRender(MeshRenderer r)
    {
        render = r;
        outline = r.GetComponent<Outline>();
        if (outline == null)
            outline = r.gameObject.AddComponent<Outline>();
        originPos = r.transform.position;
    }

    public void UpdatePos()
    {
        originPos = render.transform.position;
    }
}

public class ARObject : MonoBehaviour {

    float timer;
    const float time = 1;
    public int moveFrame = 30;
    bool startRotate = false;

    Vector3 originPos;
    List<OutlineRender> renders = new List<OutlineRender>();
    bool selected = false;
    int scatterCount = 0;

    public bool IsScattered
    {
        get { return scatterCount > 0; }
    }

    Quaternion originRot;
	float originScale;
    public bool Selected
    {
        get { return selected; }
        set {
            selected = value;
            ShowOutline(value);
            if (selected)
                ForcusObject();
            else
                ResumeObject();
        }
    }

    public void ShowOutline(bool flag)
    {
        foreach (OutlineRender ot in renders)
        {
            if (selected)
                ot.outline.enabled = flag;
            else
                ot.outline.enabled = false;
        }
    }

    // Use this for initialization
    void Start () {
		Invoke ("InitObject", 0.1f);
		Debug.Log ("object: " + name + " ; pos: " + transform.position+ " ; "+transform.localPosition);
	}
	
	void Update () {
        if (startRotate)
            AutoRotate();
	}

    void AutoRotate()
    {
        transform.Rotate(Vector3.up, 1, Space.World);
        timer -= Time.deltaTime;
        if (timer < 0)
        {
            timer = time;
        }
    }

    void ForcusObject()
    {
        originPos = transform.position;
		originScale = transform.localScale.x;
        Vector3 targetpos = UIManager.Singleton.target.position;
		float target = transform.localScale.x * 4 / GetComponent<BoxCollider> ().bounds.size.magnitude;
		StartCoroutine(moveObjectTo(transform, targetpos, originScale, target, moveFrame, StartRotateObject));
    }

    void StartRotateObject()
    {
        originRot = transform.rotation;
        startRotate = true;
    }

    void StopRotateObject()
    {
        startRotate = false;
        transform.rotation = originRot;
    }

    void ResumeObject()
    {
        StopRotateObject();
        StartCoroutine(startResume());
    }

    IEnumerator startResume()
    {
        if (scatterCount > 0)
            Gather();
        while (scatterCount > 0)
            yield return new WaitForEndOfFrame();
		StartCoroutine(moveObjectTo(transform, originPos, transform.localScale.x, originScale, moveFrame));
    }

	IEnumerator moveObjectTo(Transform tran, Vector3 targetPos, float scale, float targetScale, int frameCount, System.Action run = null)
    {
        Vector3 vec = targetPos - tran.position;
        Vector3 unitVec = vec / frameCount;
		float unitScale = (targetScale - scale) / frameCount;
        for (int i = 0; i < frameCount; i++)
        {
            yield return new WaitForFixedUpdate();
            tran.position += unitVec;
			tran.localScale += unitScale * Vector3.one;
        }
        tran.position = targetPos;
		tran.localScale = targetScale * Vector3.one;
        if (run != null)
            run();
    }

    public void SetMaterial(Material mt)
    {
        if (renders.Count == 0)
            InitObject();
        foreach(OutlineRender mr in renders)
        {
            mr.render.material = mt;
        }
    }

    public Material GetMaterial()
    {
        if (renders.Count == 0)
            InitObject();
        foreach (OutlineRender mr in renders)
        {
            if (mr.render.material != null)
                return mr.render.material;
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
                Outline ot = mr.gameObject.AddComponent<Outline>();
                ot.enabled = false;
                mr.gameObject.AddComponent<MeshCollider>();
                OutlineRender outRender1 = new OutlineRender(ot, mr, mr.transform.position);
                renders.Add(outRender1);
            }
        }
        Debug.Log("count: " + renders.Count);
    }

    public void Scatter()
    {
        if(renders.Count > 0)
        { 
            StopRotateObject();
            foreach (OutlineRender mr in renders)
            {
                Vector3 target = mr.render.transform.localPosition.normalized * mr.render.transform.localPosition.magnitude * 0.5f + mr.render.transform.position;
                mr.UpdatePos();
				StartCoroutine(moveObjectTo(mr.render.transform, target, transform.localScale.x, transform.localScale.x, 20));
            }
            scatterCount = renders.Count;
        }
    }

    public void Gather()
    {
        foreach(OutlineRender mr in renders)
        {
			StartCoroutine(moveObjectTo(mr.render.transform, mr.originPos, transform.localScale.x, transform.localScale.x, 20, gatherOneRender));
        }
    }

    void gatherOneRender()
    {
        scatterCount--;
    }
}
