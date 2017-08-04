﻿using System.IO;
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
        //public GToggleButton markerlessBtn;
        //public GToggleButton placementBtn;

        public UIControl mainMenu;
        UIControl currentPanel;

        void Start()
        {
            //try
            //{
            //    //ResourceManager.Singleton.Clear();
            //    //SetVirtualMode(DataUtility.VirtualModeInt, false);
            //    //StartCoroutine(resetObject());
            //}
            //catch (System.Exception ex)
            //{
            //    ResourceManager.Singleton.DebugString("error: " + ex.Message);
            //}
        }

        #region UI
        public void OpenMenu()
        {
            closeCurrentPanel();
            mainMenu.Open();
        }

        void closeCurrentPanel()
        {
            if (currentPanel != null)
            {
                currentPanel.Close();
                currentPanel = null;
            }
        }

        public void OpenUI(UIControl ui)
        {
            mainMenu.Close();
            currentPanel = ui;
            ui.Open();
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
            clearBtn.changeTrigger(ResourceManager.Singleton.ExistObject);
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
            DataUtility.CurrentObject = ResourceManager.Singleton.CurrentObject;
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

                SceneManager.LoadSceneAsync("VRRoom");
            }
        }

        IEnumerator resetObject()
        {
            if (DataUtility.dontdestroy.transform.childCount > 0)
            {
                //if (ResourceManager.Singleton.VMode == VirtualMode.Markerless && !ResourceManager.Singleton._kudanTracker.ArbiTrackIsTracking())
                //{
                //    yield return new WaitUntil(ResourceManager.Singleton._kudanTracker.ArbiTrackIsTracking);
                //}
                yield return new WaitForSeconds(0.5f);
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
