using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using IMAV.UI;

namespace IMAV
{
    public class UIManager : MonoBehaviour {
        //public MaterialForm mtForm;
        public MainCamCtrl camCtrl;
        public Transform room;
        //public Transform target;
        public Text lightText;
        public GameObject[] lights;

        //bool isLock = false;
        //public bool IsLock
        //{
        //    get { return isLock; }
        //    set {
        //        isLock = value;
        //        if (camCtrl != null)
        //        {
        //            camCtrl.enabled = isLock;
        //            camCtrl.orbitPivot = selectedObj.transform;
        //        }
        //    }
        //}

        private static UIManager mSingleton;
        public static UIManager Singleton
        {
            get
            {
                return mSingleton;
            }
        }

        SceneObject selectedObj;
        public SceneObject SelectedObj
        {
            get { return selectedObj; }
        }
        SceneObject dragObj;
        int lightIndex = 0;

        void Awake()
        {
            if (mSingleton)
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
            GameObject obj = Instantiate(DataUtility.CurrentObject);
            obj.transform.parent = room;
            selectedObj = obj.GetComponent<SceneObject>();
            selectedObj.transform.localPosition = new Vector3(0, 1.4f, 0);
            selectedObj.transform.localEulerAngles = new Vector3(0, 210f, 0);
            //selectedObj.transform.localScale = new Vector3(0.03f, 0.03f, 0.03f);
            camCtrl.orbitPivot = selectedObj.transform;
            camCtrl.SetorbitDistance();
            lightText.text = lights[0].name;
        }

        public void SetNextLight()
        {
            if(lightIndex < lights.Length)
            {
                lights[lightIndex].SetActive(false);
                lightIndex++;
                if (lightIndex >= lights.Length)
                    lightIndex = 0;
                lights[lightIndex].SetActive(true);
                lightText.text = lights[lightIndex].name;
            }
        }

        public void SetPrevLight()
        {
            if (lightIndex > -1)
            {
                lights[lightIndex].SetActive(false);
                lightIndex--;
                if (lightIndex < 0)
                    lightIndex = lights.Length - 1;
                lights[lightIndex].SetActive(true);
                lightText.text = lights[lightIndex].name;
            }
        }



        public void UnSelect()
        {
            //if (selectedObj != null)
            //	selectedObj.Selected = false;
        }

        bool startDrag = false;
        void Update()
        {
            //    if (Input.GetKeyDown(KeyCode.F))
            //    {
            //        if (selectedObj != null)
            //        {
            //            selectedObj.Scatter();
            //            nameForm.Hide();
            //            //brandForm.Hide();
            //            //sizeForm.Hide();
            //        }
            //    }
            //    if(Input.GetKeyDown(KeyCode.G))
            //    {
            //        if(selectedObj != null)
            //        {
            //            selectedObj.Gather();
            //            nameForm.Show(selectedObj);
            //            //brandForm.Show(selectedObj);
            //            //sizeForm.Show(selectedObj);
            //        }
            //    }
            //    if (Input.GetMouseButtonDown(0))
            //    {
            //        if (!isLock)
            //        {
            //            RaycastHit hitInfo = new RaycastHit();
            //            ARObject ao = null;
            //            bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);
            //            if (hit)
            //            {
            //                ao = hitInfo.transform.GetComponent<ARObject>();
            //                if (ao == null)
            //                {
            //                    ao = hitInfo.transform.GetComponentInParent<ARObject>();
            //                }
            //            }
            //if (!EventSystem.current.IsPointerOverGameObject(Input.GetTouch (0).fingerId))
            //                SelectObject(ao);
            //        }
            //    }
            //    //if(Input.GetMouseButtonDown(0))
            //    //{
            //    //    RaycastHit hitInfo = new RaycastHit();
            //    //    ARObject ao = null;
            //    //    bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);
            //    //    if (hit)
            //    //    {
            //    //        ao = hitInfo.transform.GetComponent<ARObject>();
            //    //        if (ao == null)
            //    //        {
            //    //            ao = hitInfo.transform.GetComponentInParent<ARObject>();
            //    //        }
            //    //        dragObj = ao;
            //    //    }
            //    //}
        }

    }
}
