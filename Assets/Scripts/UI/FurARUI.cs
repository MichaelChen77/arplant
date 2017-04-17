using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using Kudan.AR;

namespace IMAV.UI
{
    public class FurARUI : MonoBehaviour
    {
        public InsertForm insertform;
        public SearchForm searchform;
        public Text markerHint;
        public CaptureAndSave snapShot;
        public ToggleButton touchBtn;
        public ToggleButton UIToggle;
        public StatusButton constraintBtn;
        public Button detailsBtn;
        public GameObject imageViewDlg;
        public ImageGallery imageGallery;
        public Animator ctrlBtnPanelAnim;
        public TargetNode targetNode;

        public void Init(ToggleButton markerBtn)
        {
            markerBtn.SetToggle(ResourceManager.Singleton.marker);
            if (markerBtn.onToggleClick == null)
                markerBtn.onToggleClick = DoAfterSetMarker;
        }

        void Start()
        {
            try {
                touchBtn.SetToggle(ResourceManager.Singleton.touchMove);
                touchBtn.onToggleClick = SetTouchMove;
                constraintBtn.SetStatus(ResourceManager.Singleton.constraintID);
                constraintBtn.onButtonClick = SetConstraintMode;
                UIToggle.onToggleClick = ShowControlButtons;
                ResourceManager.Singleton.SetMarker(ResourceManager.Singleton.marker);
                ResourceManager.Singleton.Reset();
                StartCoroutine(resetObject());
            }
            catch(System.Exception ex)
            {
                ResourceManager.Singleton.DebugString("error: " + ex.Message);
            }
        }

        void ShowControlButtons(bool flag)
        {
            ctrlBtnPanelAnim.SetBool("Show", flag);
        }

        public void ShowMarlessArrow()
        {
            targetNode.targetAwaysOn = !targetNode.targetAwaysOn;
        }

        void Update()
        {
            if (detailsBtn.interactable == (ResourceManager.Singleton.CurrentObject == null))
                detailsBtn.interactable = !detailsBtn.interactable;
            //			if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) {
            //				if (IsFormActived () && !TouchOnUI ()) {
            //					Debug.Log ("close form");
            //					insertform.Close ();
            //					searchform.Close ();
            //				}
            //			}
            //select ();
            //if (UIToggle.IsOn)
            //{
            //    if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            //    {
            //        Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            //        RaycastHit hit;

            //        if (!Physics.Raycast(ray, out hit))
            //        {
            //            if (!EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
            //            {
            //                touchBtn.gameObject.SetActive(!touchBtn.gameObject.activeSelf);
            //                constraintBtn.gameObject.SetActive(!constraintBtn.gameObject.activeSelf);
            //                insertBtn.gameObject.SetActive(!insertBtn.gameObject.activeSelf);
            //                deleteBtn.gameObject.SetActive(!deleteBtn.gameObject.activeSelf);
            //                resetBtn.gameObject.SetActive(!resetBtn.gameObject.activeSelf);
            //                snapshotBtn.gameObject.SetActive(!snapshotBtn.gameObject.activeSelf);
            //                imagegalleryBtn.gameObject.SetActive(!imagegalleryBtn.gameObject.activeSelf);
            //                UIToggle.gameObject.SetActive(!UIToggle.gameObject.activeSelf);
            //                detailCheckBtn.gameObject.SetActive(!detailCheckBtn.gameObject.activeSelf);
            //                heightInputBtn.gameObject.SetActive(!heightInputBtn.gameObject.activeSelf);
            //            }
            //        }
            //    }
            //}
        }

        void SetTouchMove(bool flag)
        {
            ResourceManager.Singleton.touchMove = flag;
            constraintBtn.Switch(flag);
            constraintBtn.SetStatus(0);
            ResourceManager.Singleton.constraintID = 0;
        }

        void SetConstraintMode(int _id)
        {
            ResourceManager.Singleton.constraintID = _id;
        }

        string currentfile = "";
        public void CaptureScreen()
        {
            if (!Directory.Exists(DataUtility.GetScreenShotPath()))
            {
                Directory.CreateDirectory(DataUtility.GetScreenShotPath());
            }
            StartCoroutine(Screenshot());

            //cschen
            //currentfile = string.Format("ScreenShot_{0}_{1}_{2}({3}:{4}:{5}).png", System.DateTime.Now.Year, System.DateTime.Now.Month, System.DateTime.Now.Day, System.DateTime.Now.Hour,
            //    System.DateTime.Now.Minute, System.DateTime.Now.Second);

            //StartCoroutine(StartCaptureScreen());
        }

        #region Screenshot methods
        IEnumerator StartCaptureScreen()
        {
            //cschen
            //snapShot.CaptureAndSaveAtPath(DataUtility.GetScreenShotPath() + currentfile, ImageType.PNG);
            //snapShot.SetAlbumPath(DataUtility.GetScreenShotPath());
            //snapShot.CaptureAndSaveToAlbum(ImageType.PNG);

            snapShot.CaptureAndSaveToAlbum(ImageType.JPG);
            yield return new WaitForSeconds(0.5f);
            imageViewDlg.SetActive(true);
        }

        IEnumerator Screenshot()
        {
            List<GameObject> uiObjects = FindGameObjectsInUILayer();

            for (int i = 0; i < uiObjects.Count; i++)
            {
                uiObjects[i].SetActive(false);
            }
            yield return new WaitForEndOfFrame();

            RenderTexture RT = new RenderTexture(Screen.width, Screen.height, 24);

            GetComponent<Camera>().targetTexture = RT;

            Texture2D screen = new Texture2D(RT.width, RT.height, TextureFormat.RGB24, false);
            screen.ReadPixels(new Rect(0, 0, RT.width, RT.height), 0, 0);

            byte[] bytes = screen.EncodeToJPG();

            //string filePath = Application.dataPath + "/Screenshot - " + Time.unscaledTime + ".jpg";
            string filePath = DataUtility.GetScreenShotPath() + "/Screenshot - " + Time.unscaledTime + ".jpg";
            System.IO.File.WriteAllBytes(filePath, bytes);

            for (int i = 0; i < uiObjects.Count; i++)
            {
                uiObjects[i].SetActive(true);
            }

            GetComponent<Camera>().targetTexture = null;
            Destroy(RT);
            yield return new WaitForSeconds(0.3f);
            imageViewDlg.SetActive(true);
        }

        List<GameObject> FindGameObjectsInUILayer()
        {
            GameObject[] goArray = FindObjectsOfType<GameObject>();

            List<GameObject> uiList = new List<GameObject>();

            for (var i = 0; i < goArray.Length; i++)
            {
                if (goArray[i].layer == 5)
                {
                    uiList.Add(goArray[i]);
                }
            }

            if (uiList.Count == 0)
            {
                return null;
            }

            return uiList;
        }
        #endregion

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
            //ResourceManager.Singleton.disableHighlight();
            DataUtility.CurrentObject = ResourceManager.Singleton.CurrentObject;
            if (DataUtility.CurrentObject != null)
            {
                List<Transform> temp = new List<Transform>();
                foreach (Transform tr in ResourceManager.Singleton.markerlessTransform)
                {
                    temp.Add(tr);
                }
                foreach (Transform tran in temp)
                {
                    tran.parent = DataUtility.dontdestroy.transform;
                    tran.gameObject.SetActive(false);
                }

                SceneManager.LoadSceneAsync("ShowDetails");
            }
        }

        IEnumerator resetObject()
        {
            if (!ResourceManager.Singleton.marker && !ResourceManager.Singleton._kudanTracker.ArbiTrackIsTracking())
                yield return new WaitUntil(ResourceManager.Singleton._kudanTracker.ArbiTrackIsTracking);
            yield return new WaitForSeconds(1f);
            List<Transform> temp = new List<Transform>();
            ResourceManager.Singleton.DebugString("add object num: " + DataUtility.dontdestroy.transform.childCount);
            foreach (Transform tr in DataUtility.dontdestroy.transform)
            {
                temp.Add(tr);
            }
            foreach (Transform tran in temp)
            {
                tran.gameObject.SetActive(true);
                ResourceManager.Singleton.AddMarkerlessObject(tran.gameObject);
                ResourceManager.Singleton.DebugString("add object: " + tran.name);
            }
            DataUtility.CurrentObject = null;
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
