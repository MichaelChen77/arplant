using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Kudan.AR;
using System.Linq;

public enum ResType
{
    Bed, Bath, Statue, Desk, Decoration, Other
}

[System.Serializable]
public struct ResObject
{
    public GameObject resource;
    public string name;
    public Sprite thumbnail;
    public ResType type;
}

public class ResourceManager : MonoBehaviour {

    public ResObject[] PresetObjects;
	public Kudan.AR.Samples.SampleApp appui;
	public bool marker = true;
	public KudanTracker _kudanTracker;
	public TrackingMethodMarker _markerTracking;
	public TrackingMethodMarkerless _markerlessTracking;	
	//public Transform idleTranfrom;
	public Transform markerTransform;
	public Transform markerlessTransform;
	public Transform driverTransform;
	public float defaultMinSize;
	public float defaultMaxSize;
	public bool touchMove = true;
	public int constraintID = 0;
	Shader oringin;

	Vector3 startFloorPos;
	Quaternion startFloorRot;
	public Vector3 StartFloorPos {
		get{ return startFloorPos; }
	}

	public Quaternion StartFloorOrientation
	{
		get{ return startFloorRot; }
	}

	GameObject currentObj;
	public GameObject CurrentObject
	{
		get{ return currentObj; }
	}

    List<GameObject> objlist = new List<GameObject>();
	public List<GameObject> ObjList
	{
		get{ return objlist; }
	}

    private static ResourceManager mSingleton;
    public static ResourceManager Singleton
    {
        get
        {
            return mSingleton;
        }
    }

    void Awake()
    {
        if (mSingleton != null)
        {
            Destroy(gameObject);
        }
        else
        {
            mSingleton = this;
        }
    }
		
    public GameObject GetGameObject(ResType _type, string str)
    {
        foreach (ResObject obj in PresetObjects)
        {
            if (obj.type == _type && obj.name == str)
            {
                return obj.resource;
            }
        }
        return null;
    }

	public void loadnextlevel()
	{
		UnityEngine.SceneManagement.SceneManager.LoadScene ("test");
	}


    public void LoadGameObject(ResType _type, string str)
    {
        GameObject obj = GetGameObject(_type, str);
        if (obj != null)
        {
            LoadGameObject(obj);
        }
    }

    public void LoadGameObject(GameObject obj)
    {
        GameObject newObj = Instantiate(obj);
		AddMarkerlessObject(newObj);
        //newObj.transform.position = Vector3.zero;
//        MainCamCtrl cam = Camera.main.GetComponent<MainCamCtrl>();
//        if (cam != null)
//            cam.orbitPivot = obj.transform;
    }

	void ResetTouchMode()
	{
		touchMove = true;
		constraintID = 0;
		appui.touchBtn.SetToggle (touchMove);
		appui.constraintBtn.SetStatus (constraintID);
	}

	public void SetDefaultSize(GameObject obj)
	{
		BoxCollider box = obj.GetComponent<BoxCollider> ();
		float _f = box.bounds.size.magnitude;
		if (box != null) {
			if (box.bounds.size.x > defaultMaxSize) {
				float rate = defaultMaxSize / _f;
				obj.transform.localScale = obj.transform.localScale * rate;
			} 
//			else if (box.bounds.size.x > defaultMinSize) {
//				float rate = defaultMinSize / _f;
//				obj.transform.localScale = obj.transform.localScale * rate;
//			}
		}
//		if (currentObj != null) {
//			obj.transform.position = new Vector3 (currentObj.transform.position.x + 80, currentObj.transform.position.y, obj.transform.position.z);
//		}
		objlist = objlist.OrderBy(element => element.transform.position.x).ToList();
		foreach (GameObject item in objlist){
			if(item.GetComponent<BoxCollider> ().bounds.Contains(obj.transform.position))
			{
				Bounds bound = obj.GetComponent<BoxCollider> ().bounds;
				float _x1 = (bound.max.x - bound.min.x) * 0.5f;
				Bounds bound2 = item.GetComponent<BoxCollider> ().bounds;
				float _x2 = (bound2.max.x - bound2.min.x) * 0.5f;
				obj.transform.position = new Vector3 (item.transform.position.x + _x1 + _x2, item.transform.position.y, item.transform.position.z);
			}
		}
	}

	public void SetMarker(bool flag)
	{
		marker = flag;
		if (marker) {
			_kudanTracker.ChangeTrackingMethod (_markerTracking);
		}
		else {
			_kudanTracker.ChangeTrackingMethod (_markerlessTracking);
		}
	}

	public void disableHighlight()
	{
		if (currentObj != null) {
			if (currentObj.GetComponent<Renderer> () != null) {
				currentObj.GetComponent<Renderer> ().material.shader = oringin;
			} else {
				for (int i = 1; i < currentObj.transform.childCount; ++i) {
					if (currentObj.transform.GetChild (i).GetComponent<Renderer> () != null) {
						currentObj.transform.GetChild (i).GetComponent<Renderer> ().material.shader = oringin;
					}
				}
			}
		}
	}

	public void highlightObject()
	{
		if (currentObj != null) {
			if (currentObj.GetComponent<Renderer> () != null) {
				oringin = currentObj.GetComponent<Renderer> ().material.shader;
				currentObj.GetComponent<Renderer> ().material.shader = Shader.Find ("Outlined/Texture");
			} else {
				if (currentObj.transform.GetChild (1).GetComponent<Renderer> () != null) {
					oringin = currentObj.transform.GetChild (1).GetComponent<Renderer> ().material.shader;
					currentObj.transform.GetChild (1).GetComponent<Renderer> ().material.shader = Shader.Find ("Outlined/Texture");
				}
				for (int i = 2; i < currentObj.transform.childCount; ++i) {
					if (currentObj.transform.GetChild (i).GetComponent<Renderer> () != null) {
						currentObj.transform.GetChild (i).GetComponent<Renderer> ().material.shader = Shader.Find ("Outlined/Texture");
					}
				}
			}
		}
	}
		
	public void SetCurrentObject(GameObject obj)
	{
		if (currentObj == obj)
			return;
		if (currentObj != null) {
			if (marker)
				currentObj.GetComponent<MarkerTouchControl> ().enabled = false;
			else
				currentObj.GetComponent<MarkerlessTouchControl> ().enabled = false;
			disableHighlight ();
		}
		currentObj = obj;
		if (marker) {
			currentObj.GetComponent<MarkerTouchControl> ().enabled = true;
		} else {
			currentObj.GetComponent<MarkerlessTouchControl> ().enabled = true;
		}
		Debug.Log ("Set current object : " + currentObj.name);
		highlightObject ();
	}

	public void AddMarkerlessObject(GameObject obj, bool init = true)
	{
		StartCoroutine (AddingMarkerlessObject (obj, init));
	}

	IEnumerator AddingMarkerlessObject(GameObject obj, bool init)
	{
		if (!marker && !_kudanTracker.ArbiTrackIsTracking ()) {
			StartPlaceObject ();
			yield return new WaitUntil (_kudanTracker.ArbiTrackIsTracking);
		}
		DataUtility.SetAsMarkerlessObject (obj,init);
		objlist.Add (obj);
		if(init)
			SetCurrentObject (obj);
		ResetTouchMode ();
	}

	void OnApplicationPause(bool isPause)
	{
		if(isPause)
		{
			Reset();
			Debug.Log ("# pause app");
		}
	}

	GameObject storeObj = null;
	public void Pause()
	{
		if (currentObj != null) {
			storeObj = currentObj;
			currentObj.GetComponent<MarkerlessTouchControl> ().enabled = false;
			currentObj = null;
		}
	}

	public void Resume()
	{
		if(storeObj != null)
		{
			currentObj = storeObj;
			currentObj.GetComponent<MarkerlessTouchControl> ().enabled = true;
		}
	}


//	public void StartMarkerlessTracker()
//	{
//		
//		//ResetTouchMode ();
//	}

//	public void AddMarkerlessDriver()
//	{
//		GameObject obj = new GameObject ();
//		obj.name = "Markerless";
//		obj.tag = "Markerless";
//		obj.AddComponent<MarkerlessTransformDriver> ();
//		markerlessTransform = obj.transform;
//		if (!marker) {
//			_kudanTracker.StopTracking ();
//			_kudanTracker.StartTracking ();
//		}
//	}

	public void StartPlaceObject()
	{
		Vector3 floorPosition;			// The current position in 3D space of the floor
		Quaternion floorOrientation;	// The current orientation of the floor in 3D space, relative to the device

		_kudanTracker.FloorPlaceGetPose(out floorPosition, out floorOrientation);	// Gets the position and orientation of the floor and assigns the referenced Vector3 and Quaternion those values
		startFloorPos = floorPosition;
		startFloorRot = floorOrientation;
		_kudanTracker.ArbiTrackStart(startFloorPos, startFloorRot);
		Debug.Log ("# Start place object");
		//StartMarkerlessTracker ();
		//StartCoroutine (recreator ());
	}

//	public void ReInit()
//	{
//		StartCoroutine (recreator ());
//	}
//
//	IEnumerator recreator()
//	{
//		yield return new WaitForSeconds (0.2f);
//		GameObject obj = new GameObject ();
//		DataUtility.SetAsMarkerlessObject (obj);
//		StartMarkerlessTracker ();
//		yield return new WaitForSeconds (0.1f);
//		Destroy (obj);
//	}
		
    public void Clear()
    {
        foreach (GameObject obj in objlist)
        {
            Destroy(obj);
        }
        objlist.Clear();
    }

	public void Reset()
	{
		Clear ();
		ResetTouchMode ();
		//StartPlaceObject ();
	}

	public void DeleteCurrentObject()
	{
		if (currentObj != null) {
			Destroy (currentObj);
		}
		objlist.Remove (currentObj);
		currentObj = null;
	}
}
