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
    public class UIARSceneController : MonoBehaviour
    {
        public FurnitureForm furform;
        public SearchForm searchform;
        public Text markerHint;
        public CaptureAndSave snapShot;
        public Button detailsBtn;
        public GameObject imageViewDlg;
        public UIImageGallery imageGallery;
        public Animator ctrlBtnPanelAnim;
        public TargetNode targetNode;
        public DisableSelf imageSaved;
        public GToggleButton markerlessBtn;
        public GToggleButton placementBtn;

        public UIControl menuRect;
        public GameObject showMenuButton;

        void Start()
        {
            try
            {
                ResourceManager.Singleton.Reset();
                SetVirtualMode(DataUtility.VirtualModeInt, false);
                StartCoroutine(resetObject());
            }
            catch (System.Exception ex)
            {
                ResourceManager.Singleton.DebugString("error: " + ex.Message);
            }
        }

        void Update()
        {
            if (detailsBtn.interactable == (ResourceManager.Singleton.CurrentObject == null))
                detailsBtn.interactable = !detailsBtn.interactable;
            CheckTouch();
        }

        void CheckTouch()
        {
            if (Input.touchCount > 0)
            {
                foreach (Touch touch in Input.touches)
                {
                    int id = touch.fingerId;
                    if (EventSystem.current.IsPointerOverGameObject(id))
                    {
                        ResourceManager.Singleton.SetCurrentObjectState(SelectState.Pause);
                        return;
                    }
                }

                if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit))
                    {
                        GameObject touchedObject = hit.transform.gameObject;
                        if (touchedObject != null)
                        {
                            ResourceManager.Singleton.SetCurrentObject(touchedObject.GetComponent<ARModel>());
                        }
                        else
                        {
                            ResourceManager.Singleton.SetCurrentObjectState(SelectState.None);
                        }
                    }
                    HideUI();
                }
            }
        }

        void HideUI()
        {
            menuRect.Close();
        }

        UIControl currentPanel;
        public GameObject LockBackground;
        public void OpenPanelUniq(UIControl panel)
        {
            if (currentPanel != null)
                currentPanel.Close();
            OpenMenu(panel);
        }

        public void OpenPanel(UIControl panel)
        {
            OpenMenu(panel);
            HideUI();
        }

        public void OpenMenu(UIControl panel)
        {
            currentPanel = panel;
            if (currentPanel != null)
                currentPanel.Open();
        }

        public void OpenPanelLock(UIControl panel)
        {
            OpenPanel(panel);
            LockBackground.SetActive(true);
        }

        public void CloseCurrentPanel()
        {
            if (currentPanel != null)
                currentPanel.Close();
            currentPanel = null;
            LockBackground.SetActive(false);
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


        public void HideAllUI()
        {

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

            System.DateTime dt = System.DateTime.Now.ToLocalTime();
            string filePath = DataUtility.GetScreenShotPath() + "FurAR " + System.DateTime.Now.ToLocalTime().ToString("yyyy-M-d H:mm:ss") + ".jpg";
            ResourceManager.Singleton._kudanTracker.takeScreenshot(filePath, "", PostScreenShot);
        }

        void PostScreenShot(Texture2D tex, string path)
        {
            snapShot.SaveTextureToGallery(tex, ImageType.JPG);
            imageSaved.Open();
        }

        public void ViewImage()
        {
            //cschen0710
            //imageViewDlg.SetActive(false);
            //ResourceManager.Singleton.Pause();
            //if (File.Exists(DataUtility.GetScreenShotPath() + currentfile))
            //    imageGallery.Open(DataUtility.GetScreenShotPath() + currentfile);
            //else
            //    imageGallery.Open();
        }

        public void OpenImageGallery()
        {
            //cschen0710
            //imageGallery.Open();
        }

        void SetMarkerHint()
        {
            markerHint.gameObject.SetActive(true);
            if (ResourceManager.Singleton.VMode == VirtualMode.Marker)
                markerHint.text = "AR Mode Marker On";
            else if (ResourceManager.Singleton.VMode == VirtualMode.Markerless)
                markerHint.text = "AR Mode Marker Off";
            else if (ResourceManager.Singleton.VMode == VirtualMode.Placement)
                markerHint.text = "Simple Placement Mode";
            StartCoroutine(closeHint());
        }

        IEnumerator closeHint()
        {
            yield return new WaitForSeconds(1f);
            markerHint.gameObject.SetActive(false);
        }

        public void GotoVRRoom()
        {
            DataUtility.CurrentObject = ResourceManager.Singleton.CurrentObject.gameObject;
            if (DataUtility.CurrentObject != null)
            {
                List<Transform> temp = new List<Transform>();
                foreach (Transform tr in ResourceManager.Singleton.markerlessTransform)
                {
                    if (tr.tag != "static")
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
