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
		public FurnitureForm furform;
        public SearchForm searchform;
        public Text markerHint;
        public CaptureAndSave snapShot;
        public Button detailsBtn;
        public GameObject imageViewDlg;
        public ImageGallery imageGallery;
        public Animator ctrlBtnPanelAnim;
        public TargetNode targetNode;
		public DisableSelf imageSaved;
        public GToggleButton markerlessBtn;
        public GToggleButton placementBtn;

        public RectTransform menuRect;
        public UIPanel helpPanel;
        public GameObject showMenuButton;

        void Start()
        {
            try {
                ResourceManager.Singleton.Reset();
                SetVirtualMode(DataUtility.VirtualModeInt, false);
                StartCoroutine(resetObject());
            }
            catch(System.Exception ex)
            {
                ResourceManager.Singleton.DebugString("error: " + ex.Message);
            }
        }

        public void SetVirtualMode(int m)
        {
            SetVirtualMode(m, true);
        }

        public void SetVirtualMode(int m, bool showHint)
        {
            if (System.Enum.IsDefined(typeof(VirtualMode), m))
            {
                DataUtility.VirtualModeInt = m;
                VirtualMode vm = (VirtualMode)m;
                if (vm == VirtualMode.Markerless)
                {
                    markerlessBtn.setTrigger(true);
                    placementBtn.setTrigger(false);
                }
                else if (vm == VirtualMode.Placement)
                {
                    markerlessBtn.setTrigger(false);
                    placementBtn.setTrigger(true);
                }
                ResourceManager.Singleton.SetVirtualMode(vm);
                if (showHint)
                    SetMarkerHint();
            }
        }

        public void ShowControlButtons(bool flag)
        {
            ctrlBtnPanelAnim.SetBool("Show", flag);
        }

        void Update()
        {
            if (detailsBtn.interactable == (ResourceManager.Singleton.CurrentObject == null))
                detailsBtn.interactable = !detailsBtn.interactable;
			CheckTouch ();
        }

		void CheckTouch()
		{
			if (Input.touchCount > 0) {
				foreach (Touch touch in Input.touches) {
					int id = touch.fingerId;
					if (EventSystem.current.IsPointerOverGameObject (id)) {
						ResourceManager.Singleton.SetCurrentObjectState(SelectState.Pause);
						return;
					}
				}

				if (Input.touchCount == 1 && Input.GetTouch (0).phase == TouchPhase.Began) {
					Ray ray = Camera.main.ScreenPointToRay (Input.GetTouch (0).position);
					RaycastHit hit;
					if (Physics.Raycast (ray, out hit)) {
						GameObject touchedObject = hit.transform.gameObject;
						if (touchedObject != null) {
							ResourceManager.Singleton.SetCurrentObject (touchedObject.GetComponent<ARModel> ());
						} else {
							ResourceManager.Singleton.SetCurrentObjectState (SelectState.None);
						}
					}
					HideUI ();
				}
			}
		}

		void HideUI()
		{
            showMenu(false);

            //cschen0705 furform.Close();
        }

        public void HideAllUI()
        {

        }

        public void showMenu(bool flag)
        {
            if (flag)
                LeanTween.move(menuRect, Vector2.zero, 0.25f).setEaseOutQuad();
            else
                LeanTween.move(menuRect, new Vector2(menuRect.rect.width, 0f), 0.25f).setEaseInQuad();
            showMenuButton.SetActive(!flag);
        }

        void SetTouchMove(bool flag)
        {
            ResourceManager.Singleton.touchMove = flag;
//            constraintBtn.Switch(flag);
//            constraintBtn.SetStatus(0);
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

			System.DateTime dt = System.DateTime.Now.ToLocalTime ();
			string filePath = DataUtility.GetScreenShotPath() + "FurAR " + System.DateTime.Now.ToLocalTime().ToString("yyyy-M-d H:mm:ss") + ".jpg";
			ResourceManager.Singleton._kudanTracker.takeScreenshot (filePath, PostScreenShot);
        }

		void PostScreenShot(Texture2D tex)
		{
			snapShot.SaveTextureToGallery (tex, ImageType.JPG);
			imageSaved.Open ();
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

        void SetMarkerHint()
        {
            markerHint.gameObject.SetActive(true);
            if (ResourceManager.Singleton.VMode == VirtualMode.Marker)
                markerHint.text = "AR Mode Marker On";
            else if(ResourceManager.Singleton.VMode == VirtualMode.Markerless)
                markerHint.text = "AR Mode Marker Off";
            else if(ResourceManager.Singleton.VMode == VirtualMode.Placement)
                markerHint.text = "Simple Placement Mode";
            StartCoroutine(closeHint());
        }

        IEnumerator closeHint()
        {
            yield return new WaitForSeconds(1f);
            markerHint.gameObject.SetActive(false);
        }

        public void OpenDetailMode()
        {
			DataUtility.CurrentObject = ResourceManager.Singleton.CurrentObject.gameObject;
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

                SceneManager.LoadScene("VRRoom");
            }
        }

        public void TestMode()
        {
            SceneManager.LoadSceneAsync("VRRoom");
        }

        public void GotoHomeScene()
        {
            SceneManager.LoadSceneAsync("Start");
        }

        IEnumerator resetObject()
        {
            if (ResourceManager.Singleton.VMode == VirtualMode.Markerless && !ResourceManager.Singleton._kudanTracker.ArbiTrackIsTracking())
            {
                yield return new WaitUntil(ResourceManager.Singleton._kudanTracker.ArbiTrackIsTracking);
            }
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
			GameObject current = ResourceManager.Singleton.CurrentObject.gameObject;
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
