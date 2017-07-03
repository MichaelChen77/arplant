using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GizmoTrigger : MonoBehaviour {

    public ObjectGizmo objGiz;

	// Use this for initialization
	void OnMouseDown () {
        objGiz.Select(transform);
        

    }
}
