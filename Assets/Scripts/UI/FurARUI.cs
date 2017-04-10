using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace IMAV.UI
{
    public class FurARUI : MonoBehaviour
    {

        public InsertForm insertform;
        public SearchForm searchform;
        //public Text objText;
        public Text markerHint;
        public CaptureAndSave snapShot;
        public ToggleButton touchBtn;
        public ToggleButton showUnitBtn;
        public ToggleButton UIToggle;
        public StatusButton constraintBtn;
        public GameObject imageViewDlg;
        public ImageGallery imageGallery;
        public Button insertBtn;
        public Button deleteBtn;
        public Button resetBtn;
        public Button snapshotBtn;
        public Button imagegalleryBtn;
        public Button detailCheckBtn;
        public Button heightInputBtn;
        GameObject UnitObject;

        public void Init(ToggleButton markerBtn)
        {
            markerBtn.SetToggle(ResourceManager.Singleton.marker);
            if (markerBtn.onToggleClick == null)
                markerBtn.onToggleClick = DoAfterSetMarker;
        }

        void Start()
        {
            touchBtn.SetToggle(ResourceManager.Singleton.touchMove);
            touchBtn.onToggleClick = SetTouchMove;
            constraintBtn.SetStatus(ResourceManager.Singleton.constraintID);
            constraintBtn.onButtonClick = SetConstraintMode;
            UnitObject = GameObject.FindGameObjectWithTag("MarkerlessUnit");
            showUnitBtn.SetToggle(false);
            showUnitBtn.onToggleClick = setUnitObjectShow;
            UIToggle.SetToggle(true);
            ResourceManager.Singleton.SetMarker(ResourceManager.Singleton.marker);
            ResourceManager.Singleton.Reset();
            ResourceManager.Singleton.StartPlaceObject();

            Invoke("resetObject", 0.1f);
        }

        void Update()
        {
            //			if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) {
            //				if (IsFormActived () && !TouchOnUI ()) {
            //					Debug.Log ("close form");
            //					insertform.Close ();
            //					searchform.Close ();
            //				}
            //			}
            //select ();
            if (UIToggle.IsOn)
            {
                if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
                    RaycastHit hit;

                    if (!Physics.Raycast(ray, out hit))
                    {
                        if (!EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
                        {
                            touchBtn.gameObject.SetActive(!touchBtn.gameObject.activeSelf);
                            showUnitBtn.gameObject.SetActive(!showUnitBtn.gameObject.activeSelf);
                            constraintBtn.gameObject.SetActive(!constraintBtn.gameObject.activeSelf);
                            insertBtn.gameObject.SetActive(!insertBtn.gameObject.activeSelf);
                            deleteBtn.gameObject.SetActive(!deleteBtn.gameObject.activeSelf);
                            resetBtn.gameObject.SetActive(!resetBtn.gameObject.activeSelf);
                            snapshotBtn.gameObject.SetActive(!snapshotBtn.gameObject.activeSelf);
                            imagegalleryBtn.gameObject.SetActive(!imagegalleryBtn.gameObject.activeSelf);
                            UIToggle.gameObject.SetActive(!UIToggle.gameObject.activeSelf);
                            detailCheckBtn.gameObject.SetActive(!detailCheckBtn.gameObject.activeSelf);
                            heightInputBtn.gameObject.SetActive(!heightInputBtn.gameObject.activeSelf);
                        }
                    }
                }
            }
        }

        void SetTouchMove(bool flag)
        {
            ResourceManager.Singleton.touchMove = flag;
            constraintBtn.Switch(flag);
            constraintBtn.SetStatus(0);
            ResourceManager.Singleton.constraintID = 0;
        }

        void setUnitObjectShow(bool flag)
        {
            if (UnitObject != null)
                UnitObject.SetActive(flag);
        }

        void SetConstraintMode(int _id)
        {
            ResourceManager.Singleton.constraintID = _id;
        }

        //		public void PrintTest()
        //		{
        //			GameObject unitObj = GameObject.FindGameObjectWithTag ("MarkerlessUnit");
        //			if (unitObj != null)
        //			{
        //				Transform unit = unitObj.transform;
        //				Debug.Log ("# unit: " + unit.position + " ; " + unit.rotation.eulerAngles + " ; " + unit.localPosition + " ; " + unit.localEulerAngles + " . ");
        //			}
        //			else
        //				Debug.Log ("# null unit");
        //			if (ResourceManager.Singleton.CurrentObject != null) {
        //				Transform cur = ResourceManager.Singleton.CurrentObject.transform;
        //				Debug.Log ("# object: " + cur.position + " ; " + cur.eulerAngles + " ; " + cur.localPosition + " ; " + cur.localEulerAngles);
        //			}
        //			else
        //				Debug.Log ("# null current Object");
        //		}

        string currentfile = "";
        public void CaptureScreen()
        {
            if (!Directory.Exists(DataUtility.GetScreenShotPath()))
            {
                Directory.CreateDirectory(DataUtility.GetScreenShotPath());
            }
            currentfile = string.Format("ScreenShot_{0}_{1}_{2}({3}:{4}:{5}).png", System.DateTime.Now.Year, System.DateTime.Now.Month, System.DateTime.Now.Day, System.DateTime.Now.Hour,
                System.DateTime.Now.Minute, System.DateTime.Now.Second);

            //ResourceManager.Singleton.disableHighlight ();
            //snapShot.SetAlbumPath (DataUtility.GetScreenShotPath ());
            StartCoroutine(StartCaptureScreen());
            //ResourceManager.Singleton.highlightObject ();
            //snapShot.CaptureAndSaveToAlbum(ImageType.PNG);
            //imageViewDlg.SetActive (true);
        }

        IEnumerator StartCaptureScreen()
        {
            snapShot.CaptureAndSaveAtPath(DataUtility.GetScreenShotPath() + currentfile, ImageType.PNG);
            snapShot.SetAlbumPath(DataUtility.GetScreenShotPath());
            snapShot.CaptureAndSaveToAlbum(ImageType.PNG);

            //			Texture2D tex = snapShot.GetScreenShot (Screen.width, Screen.height, Camera.main, ImageType.PNG);
            //			byte[] bytes = tex.EncodeToPNG();
            //			Object.Destroy(tex);
            //			File.WriteAllBytes(DataUtility.GetScreenShotPath()+currentfile, bytes);
            yield return new WaitForSeconds(0.5f);
            imageViewDlg.SetActive(true);
        }

        public void ViewImage()
        {
            imageViewDlg.SetActive(false);
            ResourceManager.Singleton.Pause();
            if (File.Exists(DataUtility.GetScreenShotPath() + currentfile))
                imageGallery.Open(DataUtility.GetScreenShotPath() + currentfile);
            else
                imageGallery.Open();
        }

        public void OpenImageGallery()
        {
            imageGallery.Open();
        }

        void DoAfterSetMarker(bool flag)
        {
            ResourceManager.Singleton.SetMarker(flag);
            SetMarkerHint();
        }

        void SetMarkerHint()
        {
            markerHint.gameObject.SetActive(true);
            Animator anim = markerHint.GetComponent<Animator>();
            if (ResourceManager.Singleton.marker)
                markerHint.text = "Marker On";
            else
                markerHint.text = "Marker Off";
            StartCoroutine(closeHint());
        }

        IEnumerator closeHint()
        {
            yield return new WaitForSeconds(1f);
            markerHint.gameObject.SetActive(false);
        }

        bool IsFormActived()
        {
            return insertform.gameObject.activeSelf || searchform.gameObject.activeSelf;
        }

        public void OpenDetailMode()
        {
            ResourceManager.Singleton.disableHighlight();
            GameObject markless = GameObject.Find("Markerless");
            GameObject dontDestroy = GameObject.Find("DontDestroyModels");
            List<Transform> temp = new List<Transform>();
            foreach (Transform tr in markless.transform)
            {
                temp.Add(tr);
            }
            foreach (Transform tran in temp)
            {
                tran.parent = dontDestroy.transform;
                if (tran.GetComponent<ARObject>() == null)
                {
                    tran.gameObject.AddComponent<ARObject>();
                }
                tran.GetComponent<ARObject>().enabled = true;
                Destroy(tran.GetComponent<ObjectTouchControl>());
                //tran.GetComponent<MarkerlessTouchControl> ().enabled = false;
            }

            SceneManager.LoadSceneAsync("ShowDetails");
        }

        public void resetObject()
        {

            GameObject markless = GameObject.Find("Markerless");
            GameObject dontDestroy = GameObject.Find("DontDestroyModels");
            List<Transform> temp = new List<Transform>();
            foreach (Transform tr in dontDestroy.transform)
            {
                temp.Add(tr);
            }
            foreach (Transform tran in temp)
            {
                tran.gameObject.AddComponent<ObjectTouchControl>();
                ResourceManager.Singleton.AddMarkerlessObject(tran.gameObject);
                //tran.GetComponent<MarkerlessTouchControl> ().enabled = true;
                if (tran.GetComponent<ARObject>() != null)
                {
                    tran.GetComponent<ARObject>().enabled = false;
                }
            }
        }

        //		bool TouchOnUI()
        //		{
        //			if (EventSystem.current.IsPointerOverGameObject (Input.GetTouch (0).fingerId)) {
        //				Debug.Log ("touch something");
        //				return true;
        //			} else {
        //				Debug.Log ("touch nothing");
        //				return false;
        //			}
        //		}
        //		int _count = 0;
        //		void select()
        //		{
        //			if (Input.touchCount == 1) {
        //				RaycastHit hit1; 
        //				Ray ray1 = Camera.main.ScreenPointToRay(Input.GetTouch (0).position); 
        //				if ( Physics.Raycast (ray1,out hit1, Mathf.Infinity)) {
        //					//objText.text = hit1.transform.gameObject.name;
        //					//Debug.Log ("select object by 1; " + hit1.transform.gameObject.name);
        //					ResourceManager.Singleton.SetCurrentObject(hit1.transform.gameObject);
        //				}
        //			}
        //		}

        //        public void MarkerClicked()
        //        {
        //			marker = true;
        //			_kudanTracker.ChangeTrackingMethod(_markerTracking);	// Change the current tracking method to marker tracking
        //        }

        //        public void MarkerlessClicked()
        //		{
        //			marker = false;
        //			_kudanTracker.ChangeTrackingMethod(_markerlessTracking);	// Change the current tracking method to markerless tracking
        //        }

        //        public void StartClicked()
        //        {
        //            // from the floor placer.
        //            Vector3 floorPosition;			// The current position in 3D space of the floor
        //            Quaternion floorOrientation;	// The current orientation of the floor in 3D space, relative to the device
        //
        //            _kudanTracker.FloorPlaceGetPose(out floorPosition, out floorOrientation);	// Gets the position and orientation of the floor and assigns the referenced Vector3 and Quaternion those values
        //            _kudanTracker.ArbiTrackStart(floorPosition, floorOrientation);				// Starts markerless tracking based upon the given floor position and orientations
        //        }
        //
        //
        //
        //
        //		public void button2Clicked()
        //		{
        //			if (marker) {
        //				GameObject obj1 = GameObject.Find ("object1");
        //				GameObject obj2 = GameObject.Find ("object2");
        //				GameObject obj3 = GameObject.Find ("object3");
        //				obj1.GetComponent<MarkerTouchControl> ().enabled = false;
        //				obj2.GetComponent<MarkerTouchControl> ().enabled = true;
        //				obj3.GetComponent<MarkerTouchControl> ().enabled = false;
        //			} else {
        //				GameObject mobj1 = GameObject.Find("MLobject1");
        //				GameObject mobj2 = GameObject.Find("MLobject2");
        //				GameObject mobj3 = GameObject.Find("MLobject3");
        //				mobj1.GetComponent<MarkerlessTouchControl>().enabled = false;
        //				mobj2.GetComponent<MarkerlessTouchControl>().enabled = true;
        //				if (mobj3 != null) {
        //					mobj3.GetComponent<MarkerlessTouchControl>().enabled = false;
        //				}
        //			}
        //		}
        //
        //		public void button3Clicked()
        //		{
        //			if (marker) {
        //				GameObject obj1 = GameObject.Find ("object1");
        //				GameObject obj2 = GameObject.Find ("object2");
        //				GameObject obj3 = GameObject.Find ("object3");
        //				obj1.GetComponent<MarkerTouchControl> ().enabled = false;
        //				obj2.GetComponent<MarkerTouchControl> ().enabled = false;
        //				obj3.GetComponent<MarkerTouchControl> ().enabled = true;
        //			} else {
        //				GameObject mobj1 = GameObject.Find("MLobject1");
        //				GameObject mobj2 = GameObject.Find("MLobject2");
        //				GameObject mobj3 = GameObject.Find("MLobject3");
        //				mobj1.GetComponent<MarkerlessTouchControl>().enabled = false;
        //				mobj2.GetComponent<MarkerlessTouchControl>().enabled = false;
        //				if (mobj3 != null) {
        //					mobj3.GetComponent<MarkerlessTouchControl>().enabled = true;
        //				}
        //			}
        //		}

        //		public void reset()
        //		{
        //			GameObject obj1 = GameObject.Find("object1");
        //			GameObject obj2 = GameObject.Find("object2");
        //			GameObject obj3 = GameObject.Find("object3");
        //			GameObject mobj1 = GameObject.Find("MLobject1");
        //			GameObject mobj2 = GameObject.Find("MLobject2");
        //			GameObject mobj3 = GameObject.Find("MLobject3");
        //			if (marker) {
        //				if (obj1.GetComponent<MarkerTouchControl> ().enabled == true) {
        //					obj1.transform.localPosition = new Vector3 (0, 0, 0);
        //					obj1.transform.localScale = new Vector3 (1000F, 1000F, 1000F);
        //					obj1.transform.localRotation = Quaternion.Euler (-90, 0, 0);
        //				} else if (obj2.GetComponent<MarkerTouchControl> ().enabled == true) {
        //					obj2.transform.localPosition = new Vector3 (0, 0, -800F);
        //					obj2.transform.localScale = new Vector3 (1000F, 1000F, 1000F);
        //					obj2.transform.localRotation = Quaternion.Euler (-90, 0, 0);
        //				} else if (obj3.GetComponent<MarkerTouchControl> ().enabled == true) {
        //					obj3.transform.localPosition = new Vector3 (0, 0, 800F);
        //					obj3.transform.localScale = new Vector3 (1000F, 1000F, 1000F);
        //					obj3.transform.localRotation = Quaternion.Euler (-90, 180, 0);
        //				}
        //			} else {
        //				if (mobj1.GetComponent<MarkerlessTouchControl> ().enabled == true) {
        //					mobj1.transform.localPosition = new Vector3 (0, 0, 0);
        //					mobj1.transform.localScale = new Vector3 (50F, 50F, 50F);
        //					mobj1.transform.localRotation = Quaternion.Euler(0, 0, 0);
        //				} else if (mobj2.GetComponent<MarkerlessTouchControl> ().enabled == true) {
        //					mobj2.transform.localPosition = new Vector3 (300F, 0, 0);
        //					mobj2.transform.localScale = new Vector3 (200F, 200F, 200F);
        //					mobj2.transform.localRotation = Quaternion.Euler(-90, 0, 0);
        //				} else {
        //					if(mobj3 != null){
        //						mobj3.transform.localPosition = new Vector3 (600F, 0, 0);
        //						mobj3.transform.localScale = new Vector3 (200F, 200F, 200F);
        //						mobj3.transform.localRotation = Quaternion.Euler(0, 0, 0);
        //					}
        //				}
        //			}
        //		}

        public void object1Toggle()
        {
            GameObject mobj1 = GameObject.Find("MLobject1");
            mobj1.GetComponent<Renderer>().enabled = GameObject.Find("object1Toggle").GetComponent<Toggle>().isOn;
        }
        //
        //		public void object2Toggle()
        //		{
        //			if (marker) {
        //				GameObject obj2 = GameObject.Find ("object2");
        //				obj2.GetComponent<Renderer> ().enabled = GameObject.Find ("object2Toggle").GetComponent<Toggle> ().isOn;
        //			} else {
        //				GameObject mobj2 = GameObject.Find ("MLobject2");
        //				mobj2.GetComponent<Renderer> ().enabled = GameObject.Find ("object2Toggle").GetComponent<Toggle> ().isOn;
        //			}
        //		}
        //
        //		public void object3Toggle()
        //		{
        //			if (marker) {
        //				GameObject obj3 = GameObject.Find ("object3");
        //				obj3.GetComponent<Renderer> ().enabled = GameObject.Find ("object3Toggle").GetComponent<Toggle> ().isOn;
        //			} else {
        //				GameObject mobj3 = GameObject.Find ("MLobject3");
        //				mobj3.GetComponent<Renderer> ().enabled = GameObject.Find ("object3Toggle").GetComponent<Toggle> ().isOn;
        //			}
        //		}

        public void autoAdjust()
        {
            GameObject current = ResourceManager.Singleton.CurrentObject;
            float posChangeX;
            float posChangeY;
            float posChangeZ;
            posChangeX = current.transform.localPosition.x;
            posChangeY = current.transform.localPosition.y - 500;
            posChangeZ = current.transform.localPosition.z + 500;
            this.transform.localPosition = new Vector3(posChangeX, posChangeY, posChangeZ);
        }
    }
}
