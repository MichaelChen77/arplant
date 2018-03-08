using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabTest : MonoBehaviour {

	public GameObject prefab;
	public bool showflag = true;

	public Transform originTran;
	int index = 1;

	// Use this for initialization
	void Start () {
		
	}

	public void CreatePrefab()
	{
		GameObject obj = Instantiate(prefab);
		obj.transform.position += originTran.position + new Vector3(20*index, 0, 0);
        obj.transform.rotation = originTran.rotation;
        index++;
	}

	public void showMesh()
	{
		showflag = !showflag;
		MeshRenderer render = prefab.gameObject.GetComponent<MeshRenderer>();
		render.enabled = showflag;
	}
}
