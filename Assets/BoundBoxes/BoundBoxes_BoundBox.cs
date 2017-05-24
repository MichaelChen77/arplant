using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class BoundBoxes_BoundBox : MonoBehaviour {
	
//	public bool colliderBased = false;
//	public bool permanent = false; //permanent//onMouseDown
	
	public Color lineColor = new Color(0f,1f, 0.4f,0.74f);

	//private Bounds bound;

	private Vector3 boundDiff;

	private Vector3 boundExtents;

	private float originalSize;
	
	private Vector3[] corners;
	
	private Vector3[,] lines;
	
	private Quaternion quat;
	
	private BoundBoxes_drawLines cameralines;
	
	private Renderer[] renderers;
	private MeshFilter[] meshes;
	
	private Material[][] Materials;
	
	private Vector3 topFrontLeft;
	private Vector3 topFrontRight;
	private Vector3 topBackLeft;
	private Vector3 topBackRight;
	private Vector3 bottomFrontLeft;
	private Vector3 bottomFrontRight;
	private Vector3 bottomBackLeft;
	private Vector3 bottomBackRight;
	[SerializeField]
	private int lineID = 0;

	void Awake () {	
		renderers = GetComponentsInChildren<Renderer>();
		meshes = GetComponentsInChildren<MeshFilter>();
		Materials = new Material[renderers.Length][];
		for(int i = 0; i < renderers.Length; i++) {
			Materials[i]= renderers[i].materials;
		}
	}
	
	public void init() { 
		if (cameralines != null) {
			originalSize = transform.localScale.x;
			cameralines = Camera.main.GetComponent<BoundBoxes_drawLines> ();
			calculateBounds ();
			setPoints ();
			setLines ();
			if (lineID == 0)
				lineID = cameralines.AddOutlines (lines, lineColor);
			else
				cameralines.SetOutlines (lineID, lines, lineColor);
		}
	}

	void LateUpdate()
	{
		UpdateTransform ();
		setPoints ();
		setLines ();
		if (cameralines != null)
			cameralines.SetOutlines (lineID, lines, lineColor);
	}
	
	public void calculateBounds() {
		quat = transform.rotation;//object axis AABB
		if (renderers [0].isPartOfStaticBatch)
			quat = Quaternion.Euler (0f, 0f, 0f);//world axis
		Bounds bound;
		BoxCollider coll = GetComponent<BoxCollider> ();
		if (coll) {
			GameObject co = new GameObject ("dummy");
			co.transform.position = transform.position;
			co.transform.localScale = transform.lossyScale;
			BoxCollider cobc = co.AddComponent<BoxCollider> ();
			quat = transform.rotation;
			cobc.center = coll.center;
			cobc.size = coll.size;
			bound = cobc.bounds;
			Destroy (co);
			boundDiff = bound.center - transform.position;
			boundExtents = bound.extents;
			return;
		}
		bound = new Bounds ();
		if (renderers [0].isPartOfStaticBatch) {
			bound = renderers [0].bounds;
			for (int i = 1; i < renderers.Length; i++) {
				bound.Encapsulate (renderers [i].bounds);
			}
			return;
		}
		transform.rotation = Quaternion.Euler (0f, 0f, 0f);
		for (int i = 0; i < meshes.Length; i++) {
			Mesh ms = meshes [i].mesh;
			Vector3 tr = meshes [i].gameObject.transform.position;
			Vector3 ls = meshes [i].gameObject.transform.lossyScale;
			Quaternion lr = meshes [i].gameObject.transform.rotation;
			int vc = ms.vertexCount;
			for (int j = 0; j < vc; j++) {
				if (i == 0 && j == 0) {
					bound = new Bounds (tr + lr * Vector3.Scale (ls, ms.vertices [j]), Vector3.zero);
				} else {
					bound.Encapsulate (tr + lr * Vector3.Scale (ls, ms.vertices [j]));
				}
			}
		}
		transform.rotation = quat;
		boundDiff = bound.center - transform.position;
		boundExtents = bound.extents;
	}

	void UpdateTransform()
	{
		if (!renderers [0].isPartOfStaticBatch)
			quat = transform.rotation;
		float rate = transform.localScale.x / originalSize;
		originalSize = transform.localScale.x;
		boundDiff = boundDiff * rate;
		boundExtents = boundExtents * rate;
	}
	
	void setPoints() {
		Vector3 bc = transform.position + quat*boundDiff;
		topFrontRight = bc +  quat *Vector3.Scale(boundExtents, new Vector3(1, 1, 1)); 
		topFrontLeft = bc +  quat *Vector3.Scale(boundExtents, new Vector3(-1, 1, 1)); 
		topBackLeft = bc +  quat *Vector3.Scale(boundExtents, new Vector3(-1, 1, -1));
		topBackRight = bc +  quat *Vector3.Scale(boundExtents, new Vector3(1, 1, -1)); 
		bottomFrontRight = bc +  quat *Vector3.Scale(boundExtents, new Vector3(1, -1, 1)); 
		bottomFrontLeft = bc +  quat *Vector3.Scale(boundExtents, new Vector3(-1, -1, 1)); 
		bottomBackLeft = bc +  quat *Vector3.Scale(boundExtents, new Vector3(-1, -1, -1));
		bottomBackRight = bc +  quat *Vector3.Scale(boundExtents, new Vector3(1, -1, -1)); 
		corners = new Vector3[]{topFrontRight,topFrontLeft,topBackLeft,topBackRight,bottomFrontRight,bottomFrontLeft,bottomBackLeft,bottomBackRight};
		
	}
	
	void setLines() {
		
		int i1;
		int linesCount = 12;

		lines = new Vector3[linesCount,2];
		for (int i=0; i<4; i++) {
			i1 = (i+1)%4;//top rectangle
			lines[i,0] = corners[i];
			lines[i,1] = corners[i1];
			//break;
			i1 = i + 4;//vertical lines
			lines[i+4,0] = corners[i];
			lines[i+4,1] = corners[i1];
			//bottom rectangle
			lines[i+8,0] = corners[i1];
			i1 = 4 + (i+1)%4;
			lines[i+8,1] = corners[i1];
		}
	}

	public void Delete()
	{
		if(cameralines != null)
			cameralines.DeleteOutlines (lineID);
	}
}
