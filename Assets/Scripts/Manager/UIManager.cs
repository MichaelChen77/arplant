using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour {

    public DataPresentForm nameForm;
    //public DataPresentForm brandForm;
    //public DataPresentForm sizeForm;
    public MaterialForm mtForm;
    public MainCamCtrl camCtrl;
    public Transform target;

    bool isLock = false;
    public bool IsLock
    {
        get { return isLock; }
        set {
            isLock = value;
            if (camCtrl != null)
            {
                camCtrl.enabled = isLock;
                camCtrl.orbitPivot = selectedObj.transform;
            }
        }
    }


    private static UIManager mSingleton;
    public static UIManager Singleton
    {
        get
        {
            return mSingleton;
        }
    }

    ARObject selectedObj;
    ARObject dragObj;

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

    public void SelectObject(ARObject obj)
    {
        if (selectedObj != obj)
        {
            if (selectedObj != null)
                selectedObj.Selected = false;
            if (obj != null)
            {
                obj.Selected = true;
            }
            selectedObj = obj;
            nameForm.Show(obj);
            //brandForm.Show(obj);
            //sizeForm.Show(obj);
            mtForm.Open(obj);
        }
    }

	public void UnSelect()
	{
		if (selectedObj != null)
			selectedObj.Selected = false;
	}

    bool startDrag = false;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (selectedObj != null)
            {
                selectedObj.Scatter();
                nameForm.Hide();
                //brandForm.Hide();
                //sizeForm.Hide();
            }
        }
        if(Input.GetKeyDown(KeyCode.G))
        {
            if(selectedObj != null)
            {
                selectedObj.Gather();
                nameForm.Show(selectedObj);
                //brandForm.Show(selectedObj);
                //sizeForm.Show(selectedObj);
            }
        }
        if (Input.GetMouseButtonDown(0))
        {
            if (!isLock)
            {
                RaycastHit hitInfo = new RaycastHit();
                ARObject ao = null;
                bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);
                if (hit)
                {
                    ao = hitInfo.transform.GetComponent<ARObject>();
                    if (ao == null)
                    {
                        ao = hitInfo.transform.GetComponentInParent<ARObject>();
                    }
                }
				if (!EventSystem.current.IsPointerOverGameObject(Input.GetTouch (0).fingerId))
                    SelectObject(ao);
            }
        }
        //if(Input.GetMouseButtonDown(0))
        //{
        //    RaycastHit hitInfo = new RaycastHit();
        //    ARObject ao = null;
        //    bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);
        //    if (hit)
        //    {
        //        ao = hitInfo.transform.GetComponent<ARObject>();
        //        if (ao == null)
        //        {
        //            ao = hitInfo.transform.GetComponentInParent<ARObject>();
        //        }
        //        dragObj = ao;
        //    }
        //}
    }

}
