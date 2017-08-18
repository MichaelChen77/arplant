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
        public GToggleButton clearBtn;
        public Button VRButton;
        public UIObjectMenu productMenu;
        public UIMenuPanel mainMenu;
        UIControl currentPanel = null;

        private void Start()
        {
            //StartCoroutine(resetObject());
        }

        #region UI
        public void OpenMenu()
        {
            closeCurrentPanel();
            mainMenu.Open();
        }

        public void OpenProductMenu()
        {
            if (currentPanel == null || !currentPanel.gameObject.activeSelf)
            {
                float f = mainMenu.IsOpened ? -mainMenu.PanelRect.rect.width : 0;
                productMenu.Open(f);
            }
            VRButton.interactable = true;
        }

        public void CloseProductMenu()
        {
            productMenu.Close();
            VRButton.interactable = false;
        }

        void closeCurrentPanel()
        {
            if (currentPanel != null)
            {
                currentPanel.Close();
                currentPanel = null;
                OpenProductMenu();
            }
        }

        public void OpenUI(UIControl ui)
        {
            mainMenu.Close();
            currentPanel = ui;
            ui.Open();
            if (ui != productMenu)
                productMenu.Close();
        }

        public void CloseUI(bool openMenu)
        {
            closeCurrentPanel();
            if (openMenu)
                mainMenu.Open();
        }

        public void HideUI()
        {
            if (currentPanel is UIMediaCapture)
                return;
            mainMenu.Close();
            closeCurrentPanel();
        }
        #endregion

        
        void Update()
        {
            //---Stripped down version---0818
            //clearBtn.changeTrigger(ResourceManager.Singleton.ExistObject);
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
                            ResourceManager.Singleton.SetCurrentObject(touchedObject.GetComponent<ARProduct>());
                        }
                        else
                        {
                            ResourceManager.Singleton.SetCurrentObject(null);
                        }
                    }
                    HideUI();
                }
            }
        }

        #region functions

        public void ClearScene()
        {
            if (ResourceManager.Singleton.ExistObject)
                ResourceManager.Singleton.Reset();
        }

        public void GotoVRRoom()
        {
            MediaCenter.Singleton.msgDialog.Show("The VR function is temporary removed in this build!");
            //DataUtility.CurrentObject = ResourceManager.Singleton.CurrentObject;
            //if (DataUtility.CurrentObject != null)
            //{
            //    List<Transform> temp = new List<Transform>();
            //    foreach (Transform tr in ResourceManager.Singleton.markerlessTransform)
            //    {
            //        if (tr.tag != "static")
            //            temp.Add(tr);
            //    }
            //    foreach (Transform tran in temp)
            //    {
            //        tran.parent = DataUtility.dontdestroy.transform;
            //        tran.gameObject.SetActive(false);
            //    }

            //    SceneManager.LoadSceneAsync("VRPlugin");
            //}
        }

        IEnumerator resetObject()
        {
            if (DataUtility.dontdestroy.transform.childCount > 0)
            {
                if (DataUtility.TrackingMode == ARTrackingMode.Markerless && !ResourceManager.Singleton._kudanTracker.ArbiTrackIsTracking())
                {
                    ResourceManager.Singleton.StartPlaceObject();
                    yield return new WaitUntil(ResourceManager.Singleton._kudanTracker.ArbiTrackIsTracking);
                }
                yield return new WaitForSeconds(0.2f);
                List<Transform> temp = new List<Transform>();
                foreach (Transform tr in DataUtility.dontdestroy.transform)
                {
                    temp.Add(tr);
                }
                foreach (Transform tran in temp)
                {
                    tran.gameObject.SetActive(true);
                    ResourceManager.Singleton.SetAsARObject(tran.gameObject);
                }
                DataUtility.CurrentObject = null;
            }
        }

        public void GotoHomeScene()
        {
            SceneManager.LoadSceneAsync("Start");
        }

        #endregion

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
