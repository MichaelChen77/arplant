﻿using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Kudan.AR;
using System.Linq;
using IMAV.UI;

public enum ARTrackingMode
{
    Marker = 0, Markerless, Placement
}

namespace IMAV
{
    public class ResourceManager : MonoBehaviour {
        public BoundFrame frame;
        public UIARSceneController arui;
        public KudanTracker _kudanTracker;
        public TrackingMethodMarker _markerTracking;
        public TrackingMethodMarkerless _markerlessTracking;
        public Transform markerTransform;
        public Transform markerlessTransform;
        public float defaultMinSize;
        public float defaultMaxSize;
        public float markerSize;

        Vector3 trackPos;
        Quaternion trackRot;
        public Vector3 TrackPos {
            get { return trackPos; }
        }

        Vector3 originTrackPos;
        public Quaternion TrackRotation
        {
            get { return trackRot; }
        }

		ARProduct currentObj;
		public ARProduct CurrentObject
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
                Application.targetFrameRate = 45;
            }
        }

        void Start()
        {
            TestCenter.Singleton.Log("ResourceManager Start");
            SetTrackingMode(DataUtility.TrackingMode);
        }

        public void SetTrackingMode(ARTrackingMode flag)
        {
            if (flag == ARTrackingMode.Placement)
            {
                if (DataUtility.TrackingMode == ARTrackingMode.Marker)
                {
                    MoveChildrenTo(markerTransform, markerlessTransform);
                }
                _kudanTracker.ArbiTrackStop();
            }
            else if (flag == ARTrackingMode.Markerless)
            {
                if (DataUtility.TrackingMode == ARTrackingMode.Marker)
                {
                    MoveChildrenTo(markerTransform, markerlessTransform);
                }
                _kudanTracker.ChangeTrackingMethod(_markerlessTracking);
                if (DataUtility.TrackingMode == ARTrackingMode.Placement)
                    StartPlaceObject();
                else
                {
                    StopTracking();
                    TestCenter.Singleton.Log("SetTrackingMode: stop tracking");
                }
            }
            else if (flag == ARTrackingMode.Marker)
            {
                if(DataUtility.TrackingMode != ARTrackingMode.Marker)
                {
                    MoveChildrenTo(markerlessTransform, markerTransform);
                }
                _kudanTracker.ChangeTrackingMethod(_markerTracking);
            }
            DataUtility.TrackingMode = flag;
        }

        public void SetScaleFromMarkerSize(Transform tran)
        {
            if (markerSize > 0)
            {
                this.transform.localScale = this.transform.localScale * 12 * markerSize;
            }
            else
                tran.localScale = tran.localScale * 100;
        }

        void MoveChildrenTo(Transform from, Transform target)
        {
            int _count = from.childCount-1;
            for (int i = _count; i > -1; i--)
            {
                Transform tran = from.GetChild(i);
                Vector3 pos = tran.localPosition;
                Quaternion rot = tran.localRotation;
                tran.SetParent(target);
                tran.localPosition = pos;
                tran.localRotation = rot;
            }
            
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

        public void QuitApplication()
        {
            Application.Quit();
        }

        public void SetCurrentObject(ARProduct obj, SelectState st = SelectState.Actived)
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
                arui.OpenProductMenu();
            }
            else
            {
                frame.SetObject(null);
                arui.CloseProductMenu();
            }
        }

		public void SetCurrentObjectState(SelectState st)
		{
            if (st == SelectState.None)
                SetCurrentObject(null);
            else if (currentObj != null)
                currentObj.Selected = st;
		}

        public void SetAsARObject(GameObject obj)
        {
            obj.transform.parent = markerlessTransform;
            objlist.Add(obj);
            ARProduct m = obj.GetComponent<ARProduct>();
            SetCurrentObject(m);
        }

        public void AddARObject(string id, GameObject obj)
        {
            StartCoroutine(AddARObject(obj, id));
        }

        int index = 0;
        IEnumerator AddARObject(GameObject model, string _id)
        {
            if (DataUtility.TrackingMode == ARTrackingMode.Markerless && !_kudanTracker.ArbiTrackIsTracking())
            {
                StartPlaceObject();
                yield return new WaitUntil(_kudanTracker.ArbiTrackIsTracking);
            }
            try
            {
                ARProduct m = null;
                GameObject target = null;
                if (DataUtility.TrackingMode == ARTrackingMode.Marker)
                {
                    target = Instantiate(model, markerTransform);
                    m = DataUtility.InitARObject(target, Vector3.zero);
                }
                else
                {
                    target = Instantiate(model, markerlessTransform);
                    //_kudanTracker.ArbiTrackGetPose(out trackPos, out trackRot);
                    //trackPos = trackPos - originTrackPos;
                    //_kudanTracker.FloorPlaceGetPose(out trackPos, out trackRot);
                    //trackPos = trackPos - originTrackPos;
                    m = DataUtility.InitARObject(target, new Vector3(3*index, 0, 0));
                    index++;
                }
                //SceneObject s = target.AddComponent<SceneObject>();
                //s.Init(_id);
                if (m != null)
                {
                    objlist.Add(target);
                    m.SKU = _id;
                    SetCurrentObject(m);
                }
                TestCenter.Singleton.Log("ResourceManager Add object pos 1: " + target.transform.position + " ; " + target.transform.localPosition);
            }
            catch (System.Exception ex)
            {
                TestCenter.Singleton.Log("error: " + ex.Message);
            }
        }

        public void ChangeTestIndex()
        {
            Vector3 position;
            Quaternion orientation;

            _kudanTracker.ArbiTrackGetPose(out position, out orientation);
            _kudanTracker.FloorPlaceGetPose(out position, out orientation);
            TestCenter.Singleton.Log("tracking: " + _kudanTracker.ArbiTrackIsTracking()+" ; floor: "+position+" ; "+markerlessTransform.parent.localPosition);
        }

		ARProduct storeObj = null;
        int pauseIndex = 0;
        public void Pause()
        {
            if (pauseIndex == 0)
            {
                if (currentObj != null)
                {
                    storeObj = currentObj;
                    currentObj.Selected = SelectState.None;
                    currentObj = null;
                }
                _kudanTracker.enabled = false;
            }
            pauseIndex++;
        }

        public void Resume()
        {
            pauseIndex--;
            if (pauseIndex == 0)
            {
                if (storeObj != null)
                {
                    currentObj = storeObj;
                    currentObj.Selected = SelectState.Actived;
                }
                _kudanTracker.enabled = true;
            }
        }

        public void StartPlaceObject()
        {
            StartCoroutine(startArbiTracking());
        }

        IEnumerator startArbiTracking()
        {
            yield return new WaitForSeconds(0.2f);
            //Vector3 floorPos;
            Quaternion floorOrientation;
            _kudanTracker.FloorPlaceGetPose(out originTrackPos, out floorOrientation);
            _kudanTracker.ArbiTrackStart(originTrackPos, floorOrientation);
            _kudanTracker.ArbiTrackGetPose(out originTrackPos, out trackRot);
            TestCenter.Singleton.Log("start arbi: " + originTrackPos);
        }

        public void GetFloorPos()
        {
            _kudanTracker.FloorPlaceGetPose(out trackPos, out trackRot);
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
                TestCenter.Singleton.Log("Clear error: " + ex.Message);
            }
        }

        public void Reset()
        {
            Clear();
            StopTracking();
        }

        public void Quit()
        {
            using (AndroidJavaClass cls = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                using (AndroidJavaObject jo = cls.GetStatic<AndroidJavaObject>("currentActivity"))
                {
                    jo.Call("quitActivity", "unityquit");
                }
            }    
        }

        public void Browse()
        {
            using (AndroidJavaClass cls = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                using (AndroidJavaObject jo = cls.GetStatic<AndroidJavaObject>("currentActivity"))
                {
                    jo.Call("browse");
                }
            }
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
