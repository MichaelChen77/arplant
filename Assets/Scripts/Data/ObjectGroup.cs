using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectGroup : MonoBehaviour {

    List<GameObject> objects = new List<GameObject>();
    int objectIndex = 0;
	// Use this for initialization
	void Awake () {
        Init();
	}

    public void Init()
    {
        foreach (Transform tran in transform)
        {
            objects.Add(tran.gameObject);
        }
    }

    public GameObject GetCurrentObject()
    {
        if(objectIndex > -1 && objectIndex < objects.Count)
        {
            return objects[objectIndex];
        }
        return null;
    }

    public GameObject ActiveNextObject()
    {
        if (objectIndex < objects.Count)
        {
            objects[objectIndex].SetActive(false);
            objectIndex++;
            if (objectIndex >= objects.Count)
                objectIndex = 0;
            objects[objectIndex].SetActive(true);
            return objects[objectIndex];
        }
        return null;
    }

    public GameObject ActivePrevObject()
    {
        if (objectIndex > -1)
        {
            objects[objectIndex].SetActive(false);
            objectIndex--;
            if (objectIndex < 0)
                objectIndex = objects.Count - 1;
            objects[objectIndex].SetActive(true);
            return objects[objectIndex];
        }
        return null;
    }
}
