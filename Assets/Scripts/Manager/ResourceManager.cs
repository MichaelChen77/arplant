using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Kudan.AR;
using System.Linq;
using IMAV.UI;

[System.Serializable]
public class FurCategory
{
    public string name;
    public Sprite thumbnail;
    List<ResObject> furObjects = new List<ResObject>();
    public List<ResObject> Furnitures
    {
        get
        {
            if (furObjects == null)
                furObjects = new List<ResObject>();
            return furObjects;
        }
    }

    public FurCategory(string _name, Sprite sp)
    {
        name = _name;
        thumbnail = sp;
    }
}

public class ResObject
{
    public GameObject resource;
    public string name;
    public Sprite thumbnail;
    string cat;
    public string Category
    {
        get { return cat; }
        set { cat = value; }
    }

    public ResObject(string _cat, string _name)
    {
        cat = _cat;
        name = _name;
    }
}

public enum VirtualMode
{
    Markerless, Marker, Placement
}

namespace IMAV
{
    public class ResourceManager : MonoBehaviour {
        public FurCategory[] FurCategories;
        public FurARUI appui;
        public BoundFrame frame;
        public DebugView debugview;
        //public bool marker = true;
        public KudanTracker _kudanTracker;
        public TrackingMethodMarker _markerTracking;
        public TrackingMethodMarkerless _markerlessTracking;
        public Transform markerTransform;
        public Transform markerlessTransform;
        public Transform driverTransform;
        public float defaultMinSize;
        public float defaultMaxSize;
        public bool touchMove = true;
        public int constraintID = 0;
        VirtualMode vMode = VirtualMode.Markerless;
        public VirtualMode VMode
        {
            get { return vMode; }
        }
        Vector3 trackPos;
        Quaternion trackRot;
        public Vector3 TrackPos {
            get { return trackPos; }
        }

        public Quaternion TrackRotation
        {
            get { return trackRot; }
        }

		ARModel currentObj;
		public ARModel CurrentObject
        {
            get { return currentObj; }
        }

        List<GameObject> objlist = new List<GameObject>();
        public List<GameObject> ObjList
        {
            get { return objlist; }
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

        void Start()
        {
            LoadResources();
            if (DataUtility.WorkOnLocal)
            {
                DataManager.Singleton.FurnitureDatas.Clear();
                int k = 0;
                for(int i=0; i<FurCategories.Length; i++)
                {
                    for (int j = 0; j < FurCategories[i].Furnitures.Count; j++)
                    {
                        DataManager.Singleton.Init(k, FurCategories[i].Furnitures[j]);
                        k++;
                    }
                }
            }
        }

        void LoadResources()
        {
            foreach (FurCategory cat in FurCategories)
            {
                Object[] objs = Resources.LoadAll("Furnitures/" + cat.name);
                cat.Furnitures.Clear();
                if (objs != null)
                {
                    for (int i = 0; i < objs.Length; i++)
                    {
                        ResObject o = new ResObject(cat.name, objs[i].name);
                        o.resource = (GameObject)objs[i];
                        o.thumbnail = Resources.Load<Sprite>("FurImages/" + cat.name + "/" + objs[i].name);
                        cat.Furnitures.Add(o);
                    }
                }
            }
        }

        public void SetVirtualMode(VirtualMode flag)
        {
            vMode = flag;
            if (vMode == VirtualMode.Placement)
            {
                _kudanTracker.ChangeTrackingMethod(_markerlessTracking);
                _kudanTracker.ArbiTrackStop();
            }
            else if(vMode == VirtualMode.Markerless)
            {
                _kudanTracker.ChangeTrackingMethod(_markerlessTracking);
                StartPlaceObject();
            }
            else if(vMode == VirtualMode.Marker)
            {
                _kudanTracker.ChangeTrackingMethod(_markerTracking);
            }
        }

        public void DebugString(string str)
        {
            if (debugview != null)
                debugview.AppendTextLog(str);
        }

        public GameObject GetGameObject(string _type, string str)
        {
            FurCategory cat = GetCategory(_type);
            if (cat != null)
            {
                foreach (ResObject obj in cat.Furnitures)
                {
                    if (obj.name == str)
                        return obj.resource;
                }
            }
            return null;
        }

        public FurCategory GetCategory(string _type)
        {
            foreach(FurCategory cat in FurCategories)
            {
                if (cat.name == _type)
                    return cat;
            }
            return null;
        }

        public void loadnextlevel()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("ARScene");
        }

        public void LoadGameObject(string _type, string str)
        {
            GameObject obj = GetGameObject(_type, str);
            string _content = _type + "-" + str;
            if (obj != null)
            {
                LoadGameObject(obj, _content);
            }
        }

        public void LoadGameObject(GameObject obj, string str)
        {
            GameObject newObj = Instantiate(obj);
            AddMarkerlessLocalObject(str, newObj, true);
        }

        void ResetTouchMode()
        {
            touchMove = true;
            constraintID = 0;
        }

        public void SetDefaultSize(GameObject obj)
        {
            BoxCollider box = obj.GetComponent<BoxCollider>();
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
            foreach (GameObject item in objlist) {
                if (item.GetComponent<BoxCollider>().bounds.Contains(obj.transform.position))
                {
                    Bounds bound = obj.GetComponent<BoxCollider>().bounds;
                    float _x1 = (bound.max.x - bound.min.x) * 0.5f;
                    Bounds bound2 = item.GetComponent<BoxCollider>().bounds;
                    float _x2 = (bound2.max.x - bound2.min.x) * 0.5f;
                    obj.transform.position = new Vector3(item.transform.position.x + _x1 + _x2, item.transform.position.y, item.transform.position.z);
                }
            }
        }

        public void SetCurrentObject(ARModel obj, SelectState st = SelectState.Actived)
        {
            if (currentObj != null)
            {
                if (currentObj == obj)
                {
                    currentObj.Selected = st;
                    return;
                }
                else
                    currentObj.Selected = SelectState.None;
            }
            currentObj = obj;
            if (currentObj != null)
            {
                currentObj.Selected = st;
                frame.SetObject(currentObj.gameObject);
            }
            else
                frame.SetObject(null);
        }

		public void SetCurrentObjectState(SelectState st)
		{
			if(currentObj != null)
			{
				currentObj.Selected = st;
			}
		}

        public void AddMarkerlessObject(GameObject obj)
        {
            StartCoroutine(AddingMarkerlessObject(obj, false, false, "0", ""));
        }

        public void AddMarkerlessRemoteObject(string id, GameObject obj, bool init)
        {
            StartCoroutine(AddingMarkerlessObject(obj, init, false, id, ""));
        }

        public void AddMarkerlessLocalObject(string _content, GameObject obj, bool init)
        {
            StartCoroutine(AddingMarkerlessObject(obj, init, true, "0", _content));
        }

        IEnumerator AddingMarkerlessObject(GameObject obj, bool init, bool _islocal, string _id, string _content)
        {
			objlist.Add(obj);
            if (vMode == VirtualMode.Markerless && !_kudanTracker.ArbiTrackIsTracking())
            {
                StartPlaceObject();
                yield return new WaitUntil(_kudanTracker.ArbiTrackIsTracking);
            }
            try
            {
                _kudanTracker.FloorPlaceGetPose(out trackPos, out trackRot);
                if (init)
                {
                    SceneObject sobj = obj.AddComponent<SceneObject>();
                    sobj.Init(_islocal, _id, _content);
                }
                ARModel m = DataUtility.SetAsMarkerlessObject(obj, init, _islocal, _content);
                SetCurrentObject(m);
                ResetTouchMode();
            }
            catch (System.Exception ex)
            {
                DebugString("error: " + ex.Message);
            }
        }

		ARModel storeObj = null;
        public void Pause()
        {
            if (currentObj != null) {
                storeObj = currentObj;
				currentObj.Selected = SelectState.None;
                currentObj = null;
            }
        }

        public void Resume()
        {
            if (storeObj != null)
            {
                currentObj = storeObj;
				currentObj.Selected = SelectState.Actived;
            }
        }

        public void StartPlaceObject()
        {
            Vector3 floorPosition;          // The current position in 3D space of the floor
            Quaternion floorOrientation;    // The current orientation of the floor in 3D space, relative to the device
            _kudanTracker.FloorPlaceGetPose(out floorPosition, out floorOrientation);   // Gets the position and orientation of the floor and assigns the referenced Vector3 and Quaternion those values
            _kudanTracker.ArbiTrackStart(floorPosition, floorOrientation);
        }

        public void Clear()
        {
            SetCurrentObject(null);
            foreach (GameObject obj in objlist)
            {
                DestroyImmediate(obj);
            }
            objlist.Clear();
        }

        public void Reset()
        {
            Clear();
            ResetTouchMode();
            //StartPlaceObject();
        }

        public void Quit()
        {
            AndroidJavaClass cls = new AndroidJavaClass("eu.kudan.ar.UnityPlayerActivity");
            cls.Call("goToActivity");
        }

        public void DeleteCurrentObject()
        {
            if (currentObj != null) {
                currentObj.Delete ();
				objlist.Remove(currentObj.gameObject);
                currentObj = null;
                SetCurrentObject(null);
            }
        }

        #region scene

        public string GetSceneString()
        {
            string str = "";
            for (int i = 0; i < objlist.Count; i++)
            {
                SceneObject obj = objlist[i].GetComponent<SceneObject>();
                if (obj != null)
                    str += obj.ToDataString();
            }
            return str;
        }
        #endregion
    }
}
