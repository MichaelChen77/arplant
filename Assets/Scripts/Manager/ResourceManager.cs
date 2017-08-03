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
    Markerless = 0, Marker, Placement
}

namespace IMAV
{
    public class ResourceManager : MonoBehaviour {
        //public ResController localResCtrl;
        public BoundFrame frame;
        public DebugView debugview;
        public UIARSceneController arui;
        public UIControl selectedFrame;
        public KudanTracker _kudanTracker;
        public TrackingMethodMarker _markerTracking;
        public TrackingMethodMarkerless _markerlessTracking;
        public Transform markerTransform;
        public Transform markerlessTransform;
        public Transform driverTransform;
        public float defaultMinSize;
        public float defaultMaxSize;
        public Transform testTransform;

        FurCategory[] furcategories;
        public FurCategory[] FurCategories
        {
            get { return furcategories; }
        }

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

        public bool ExistObject
        {
            get { return objlist.Count != 0; }
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
            //if (DataUtility.WorkOnLocal)
            //{
            //    localResCtrl.LoadLocalResource(furcategories);
            //}
            _kudanTracker.ChangeTrackingMethod(_markerlessTracking);
            //StartPlaceObject();
        }

        public void SetVirtualMode(VirtualMode flag)
        {
            vMode = flag;
            if (vMode == VirtualMode.Placement)
            {
                _kudanTracker.StopTracking();
                //_kudanTracker.ArbiTrackStop();
            }
            else if(vMode == VirtualMode.Markerless)
            {
                _kudanTracker.ChangeTrackingMethod(_markerlessTracking);
                StartPlaceObject();
            }
            else if(vMode == VirtualMode.Marker)
            {
                _kudanTracker.ChangeTrackingMethod(_markerTracking);
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
            foreach(FurCategory cat in furcategories)
            {
                if (cat.name == _type)
                    return cat;
            }
            return null;
        }

        public void loadnextlevel()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("AR");
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

        //void ResetTouchMode()
        //{
        //    touchMove = true;
        //    constraintID = 0;
        //}

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
                frame.SetObject(currentObj);
                arui.OpenUI(selectedFrame);
            }
            else
            {
                frame.SetObject(null);
                arui.CloseUI(false);
            }
        }

		public void SetCurrentObjectState(SelectState st)
		{
            if (st == SelectState.None)
                SetCurrentObject(null);
            else if (currentObj != null)
                currentObj.Selected = st;
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
                DebugString("has trackingdata0: " + _kudanTracker.HasActiveTrackingData() + " ; " + _markerlessTracking.TrackingEnabled);
            }
            try
            {
                //Vector3 trackPos1;
                //Quaternion trackRot1;
                _kudanTracker.FloorPlaceGetPose(out trackPos, out trackRot);
                DebugString("trackPos: "+trackPos+" ; test: " + testTransform.position + " ; " + testTransform.localPosition);
                if (currentObj != null)
                    DebugString("current: " + currentObj.transform.position + " ; " + currentObj.transform.localPosition);
                DebugString("has trackingdata1: " + _kudanTracker.HasActiveTrackingData() + " ; " + _markerlessTracking.TrackingEnabled);
                if (init)
                {
                    SceneObject sobj = obj.AddComponent<SceneObject>();
                    sobj.Init(_islocal, _id, _content);
                }
                ARModel m = DataUtility.SetAsMarkerlessObject(obj, init, _islocal, _content);
                ResourceManager.Singleton.DebugString("set as markerless");
                SetCurrentObject(m);
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
            Vector3 floorPosition;
            Quaternion floorOrientation;

            _kudanTracker.FloorPlaceGetPose(out floorPosition, out floorOrientation);   // Gets the position and orientation of the floor and assigns the referenced Vector3 and Quaternion those values
            _kudanTracker.ArbiTrackStart(floorPosition, floorOrientation);              // Starts markerless tracking based upon the given floor position and orientations
        }

        public void StopTracking()
        {
            _kudanTracker.ArbiTrackStop();
        }

        public void Clear()
        {
            try
            {
                SetCurrentObject(null);
                foreach (GameObject obj in objlist)
                {
                    Destroy(obj);
                }
                objlist.Clear();
            }
            catch(System.Exception ex)
            {
                DebugString("Clear error: " + ex.Message);
            }
        }

        public void Reset()
        {
            Clear();
            StopTracking();
        }

        public void Quit()
        {
            AndroidJavaClass cls = new AndroidJavaClass("eu.kudan.ar.UnityPlayerActivity");
            cls.Call("quitActivity", "unityquit");
        }


        public void Browse()
        {
            AndroidJavaClass cls = new AndroidJavaClass("eu.kudan.ar.UnityPlayerActivity");
            cls.Call("browse");
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
