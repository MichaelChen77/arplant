﻿using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Kudan.AR;
using System.Linq;
using IMAV.UI;

public enum ResType
{
    Bed, Bath, Statue, Desk, Decoration, Other, Sunray
}

[System.Serializable]
public struct ResObject
{
    public GameObject resource;
    public string name;
    public Sprite thumbnail;
    public ResType type;
}

namespace IMAV
{
    public class ResourceManager : MonoBehaviour {

        public ResObject[] PresetObjects;
        public ResObject[] LocalObjects;
        public FurARUI appui;
        public DebugView debugview;
        public bool marker = true;
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
        public GameObject hlightPrefab;
        Shader oringin;

        GameObject hightlightAuro;
        Vector3 trackPos;
        Quaternion trackRot;
        public Vector3 TrackPos {
            get { return trackPos; }
        }

        public Quaternion TrackRotation
        {
            get { return trackRot; }
        }

        GameObject currentObj;
        public GameObject CurrentObject
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
            if(DataUtility.WorkOnLocal)
            {
                DataManager.Singleton.FurnitureDatas.Clear();
                for(int i=0; i<LocalObjects.Length; i++)
                {
                    DataManager.Singleton.Init(i, LocalObjects[i]);
                }
            }
        }

        public void DebugString(string str)
        {
            if (debugview != null)
                debugview.AppendTextLog(str);
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
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainAR");
        }

        public void LoadGameObject(ResType _type, string str)
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
            appui.touchBtn.SetToggle(touchMove);
            appui.constraintBtn.SetStatus(constraintID);
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

        public void SetMarker(bool flag)
        {
            marker = flag;
            if (marker) {
                _kudanTracker.ChangeTrackingMethod(_markerTracking);
            }
            else {
                _kudanTracker.ChangeTrackingMethod(_markerlessTracking);
            }
        }

        public void disableHighlight()
        {
            if (currentObj != null) {
                DestroyImmediate(hightlightAuro);
                hightlightAuro = null;
                //if (currentObj.GetComponent<Renderer>() != null) {
                //    currentObj.GetComponent<Renderer>().material.shader = oringin;
                //} else {
                //    for (int i = 1; i < currentObj.transform.childCount; ++i) {
                //        if (currentObj.transform.GetChild(i).GetComponent<Renderer>() != null) {
                //            currentObj.transform.GetChild(i).GetComponent<Renderer>().material.shader = oringin;
                //        }
                //    }
                //}
            }
        }

        public void highlightObject()
        {
            if (currentObj != null)
            {
                hightlightAuro = Instantiate(hlightPrefab);
                hightlightAuro.transform.parent = currentObj.transform;
                hightlightAuro.transform.localPosition = Vector3.zero;
                hightlightAuro.transform.localScale = Vector3.one;
                //if (currentObj.GetComponent<Renderer>() != null) {
                //    oringin = currentObj.GetComponent<Renderer>().material.shader;
                //    currentObj.GetComponent<Renderer>().material.shader = Shader.Find("Outlined/Texture");
                //} else {
                //    if (currentObj.transform.GetChild(1).GetComponent<Renderer>() != null) {
                //        oringin = currentObj.transform.GetChild(1).GetComponent<Renderer>().material.shader;
                //        currentObj.transform.GetChild(1).GetComponent<Renderer>().material.shader = Shader.Find("Outlined/Texture");
                //    }
                //    for (int i = 2; i < currentObj.transform.childCount; ++i) {
                //        if (currentObj.transform.GetChild(i).GetComponent<Renderer>() != null) {
                //            currentObj.transform.GetChild(i).GetComponent<Renderer>().material.shader = Shader.Find("Outlined/Texture");
                //        }
                //    }
                //}
                
            }
        }

        public void SetCurrentObject(GameObject obj)
        {
            if (currentObj == obj)
                return;
            if (currentObj != null) {
                currentObj.GetComponent<ObjectTouchControl>().enabled = false;
                disableHighlight();
            }
            currentObj = obj;
            currentObj.GetComponent<ObjectTouchControl>().enabled = true;
            highlightObject();
        }

        public void AddMarkerlessObject(GameObject obj)
        {
            StartCoroutine(AddingMarkerlessObject(obj, false, false, 0, ""));
        }

        public void AddMarkerlessRemoteObject(int id, GameObject obj, bool init)
        {
            StartCoroutine(AddingMarkerlessObject(obj, init, false, id, ""));
        }

        public void AddMarkerlessLocalObject(string _content, GameObject obj, bool init)
        {
            StartCoroutine(AddingMarkerlessObject(obj, init, true, 0, _content));
        }

        IEnumerator AddingMarkerlessObject(GameObject obj, bool init, bool _islocal, int _id, string _content)
        {
            objlist.Add(obj);
            if (!marker && !_kudanTracker.ArbiTrackIsTracking())
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
                DataUtility.SetAsMarkerlessObject(obj, init, _islocal, _id, _content);
                SetCurrentObject(obj);
                ResetTouchMode();
            }
            catch (System.Exception ex)
            {
                DebugString("error: " + ex.Message);
            }
        }

        void OnApplicationPause(bool isPause)
        {
            if (isPause)
            {
                DebugString("pausing "+markerlessTransform.gameObject.activeSelf);
                Reset();
            }
        }

        GameObject storeObj = null;
        public void Pause()
        {
            if (currentObj != null) {
                storeObj = currentObj;
                currentObj.GetComponent<ObjectTouchControl>().enabled = false;
                currentObj = null;
            }
        }

        public void Resume()
        {
            if (storeObj != null)
            {
                currentObj = storeObj;
                currentObj.GetComponent<ObjectTouchControl>().enabled = true;
            }
        }

        //public void StartMarkerlessTracker()
        //{
        //    ResetTouchMode();
        //}

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
            Vector3 floorPosition;          // The current position in 3D space of the floor
            Quaternion floorOrientation;    // The current orientation of the floor in 3D space, relative to the device

            _kudanTracker.FloorPlaceGetPose(out floorPosition, out floorOrientation);   // Gets the position and orientation of the floor and assigns the referenced Vector3 and Quaternion those values
            _kudanTracker.ArbiTrackStart(floorPosition, floorOrientation);
        }

        //	public void ReInit()
        //	{
        //		StartCoroutine (recreator ());
        //	}
        //
        //IEnumerator recreator()
        //{
        //    yield return new WaitForSeconds(0.2f);
        //    GameObject obj = new GameObject();
        //    DataUtility.SetAsMarkerlessObject(obj, false, false, );
        //    StartMarkerlessTracker();
        //    yield return new WaitForSeconds(0.1f);
        //    Destroy(obj);
        //}

        public void Clear()
        {
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
            StartPlaceObject();
        }

        //IEnumerator WaitToStartArbiTracking()
        //{
        //    DebugString("reset 0");
        //    yield return new WaitWhile(_kudanTracker.ArbiTrackIsTracking);
        //    yield return new WaitForEndOfFrame();
        //    DebugString("reset 1");

        //}

        public void DeleteCurrentObject()
        {
            if (currentObj != null) {
                Destroy(currentObj);
                objlist.Remove(currentObj);
                currentObj = null;
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
