using UnityEngine;
using System.Collections;

public class MainCamCtrl : MonoBehaviour {

    public enum CameraMode
    {
        free, thirdPerson, RTS, Fix
    }

    public enum camCtrlMode
    {
        move, orbit, rotate, zoom, flythrough
    }

    public bool enabled = true;
    public CameraMode currentMode = CameraMode.free;
    public float camMoveSpeed = 50f;
    public float camPanSpeed = 75f;
    public float camZoomSpeed = 10f;
    public float camDragSpeed = 30f;
    public float shiftRate = 2f;
    public Transform orbitPivot;
    public float camTargetDistance = 10;
    public bool useOCD = true;

    public Texture2D camDragCursor;
    public Texture2D camOrbitCursor;
    public Texture2D camRotateCursor;
    public Texture2D camZoomCursor;

    //FreeCam variables
    private float camXRot = 0;
    private float camYRot = 0;
    private float camZRot = 0;
    static MainCamCtrl mSingleton;
    public static MainCamCtrl Singleton
    {
        get
        {
            return mSingleton;
        }
    }
    void Awake()
    {
        mSingleton = this;
    }
	// Use this for initialization
	void Start () {
        camXRot = transform.eulerAngles.x;
        camYRot = transform.eulerAngles.y;
        camZRot = transform.eulerAngles.z;
    }
	
	// Update is called once per frame
	void Update () {
        if (enabled)
        {
            FreeCam();

            //if (Input.GetKey(KeyCode.F))
            //{
            //    orbitCamera();
            //}
        }
	}

    public float ClampAngle(float angle, float min, float max)
    {
        if(angle < -360)
            angle += 360;
        else if(angle > 360)
            angle -= 360;
        return Mathf.Clamp(angle, min, max);
    }

    void updateCamXYRot()
    {
        camYRot += deltaVec.x * (Time.deltaTime * camPanSpeed);
        //camYRot += Input.GetAxis("Mouse X") * (Time.deltaTime * camPanSpeed);
        camYRot = ClampAngle(camYRot, -360, 360);
        camXRot -= deltaVec.y * (Time.deltaTime * camPanSpeed);
        //camXRot -= Input.GetAxis("Mouse Y") * (Time.deltaTime * camPanSpeed);
        camXRot = ClampAngle(camXRot, -90, 90);
        //Debug.Log(Input.GetAxis("Mouse X") + " ; " + Input.GetAxis("Mouse Y"));
    }

    public void SetorbitDistance()
    {
        if (orbitPivot != null && useOCD)
        {
            camTargetDistance = Vector3.Distance(orbitPivot.position, transform.position);
        }
    }

    Vector2 deltaVec = Vector2.zero;
    void FreeCam()
    {
        if (Input.touchCount == 1)
        {
            Touch fing1 = Input.GetTouch(0);
            if (fing1.phase == TouchPhase.Moved)
            {
                deltaVec = fing1.deltaPosition * 0.1f;
                orbitCamera();
            }
        }
        processZoom();

                //if (Input.GetKeyDown(KeyCode.LeftAlt))
                //{
                //    if (orbitPivot != null && useOCD)
                //        camTargetDistance = Vector3.Distance(orbitPivot.position, transform.position);
                //}
                //if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
                //{
                //    shiftUpSpeed();
                //}
                //if (Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.RightShift))
                //{
                //    shiftDownSpeed();
                //}
                //if (Input.GetMouseButton(0))
                //{
                //    if (Input.GetKey(KeyCode.LeftAlt))
                //    {
                //        orbitCamera();
                //    }
                //    else if (Input.GetKey(KeyCode.LeftControl))
                //    {
                //        dragCamera();
                //    }
                //}
                //if (Input.GetMouseButton(1))
                //{
                //    if (Input.GetKey(KeyCode.LeftAlt))
                //    {
                //        zoomCamera();
                //    }
                //    else
                //    {
                //        rotatAndFlyCamera();
                //    }
                //}
                //updateCursor();
                //if(Input.GetMouseButtonUp(0)||Input.GetMouseButtonUp(1))
                //{
                //    Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
                //}
                moveCamera();
        transform.position = transform.position + transform.forward * Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * camZoomSpeed*30f;
    }

    void updateCursor()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (Input.GetKey(KeyCode.LeftAlt))
                Cursor.SetCursor(camOrbitCursor, Vector2.zero, CursorMode.Auto);
            else if (Input.GetKey(KeyCode.LeftControl))
                Cursor.SetCursor(camDragCursor, Vector2.zero, CursorMode.Auto);
        }
        if (Input.GetMouseButtonDown(1))
        {
            if (Input.GetKey(KeyCode.LeftAlt))
                Cursor.SetCursor(camZoomCursor, Vector2.zero, CursorMode.Auto);
            else
                Cursor.SetCursor(camRotateCursor, Vector2.zero, CursorMode.Auto);
        }
    }

    void shiftUpSpeed()
    {
        camMoveSpeed = camMoveSpeed * shiftRate;
        camPanSpeed = camPanSpeed * shiftRate;
        camZoomSpeed = camZoomSpeed * shiftRate;
        camDragSpeed = camDragSpeed * shiftRate;
    }

    void shiftDownSpeed()
    {
        camMoveSpeed = camMoveSpeed / shiftRate;
        camPanSpeed = camPanSpeed / shiftRate;
        camZoomSpeed = camZoomSpeed / shiftRate;
        camDragSpeed = camDragSpeed / shiftRate;
    }

    void orbitCamera()
    {
        updateCamXYRot();
        Quaternion rotation = Quaternion.Euler(camXRot, camYRot, camZRot);
        if (orbitPivot != null)
        {
            Vector3 negDistance = new Vector3(0.0f, 0.0f, -camTargetDistance);
            transform.position = rotation * negDistance + orbitPivot.position;
        }
        transform.rotation = rotation;
    }

    void zoomCamera()
    {
        float _speed = (Input.GetAxis("Mouse X") - Input.GetAxis("Mouse Y")) * Time.deltaTime * camZoomSpeed;
        transform.position = transform.position + transform.forward * _speed;
    }

    void processZoom()
    {
        if (Input.touchCount == 2)
        {
            //Store inputs
            Touch fing1 = Input.GetTouch(0);
            Touch fing2 = Input.GetTouch(1);

            if (fing1.phase == TouchPhase.Moved || fing2.phase == TouchPhase.Moved) //If either finger has moved since the last frame
            {
                //Get previous positions
                Vector2 fing1Prev = fing1.position - fing1.deltaPosition;
                Vector2 fing2Prev = fing2.position - fing2.deltaPosition;

                //Find vector magnitude between touches in each frame
                float prevTouchDeltaMag = (fing1Prev - fing2Prev).magnitude;
                float touchDeltaMag = (fing1.position - fing2.position).magnitude;

                //Find difference in distances
                float deltaDistance = (prevTouchDeltaMag - touchDeltaMag) * 0.1f;

                float _speed = deltaDistance - deltaDistance * Time.deltaTime * camZoomSpeed;
                transform.position = transform.position + transform.forward * _speed;

                //SetorbitDistance();
            }

            if(fing1.phase == TouchPhase.Ended || fing2.phase == TouchPhase.Ended)
            {
                SetorbitDistance();
            }
        }
    }

    void rotatAndFlyCamera()
    {
        updateCamXYRot();
        transform.eulerAngles = new Vector3(camXRot, camYRot, camZRot);
        if (Input.GetKey(KeyCode.W))
        {
            float rate = Mathf.Clamp(transform.position.y, 0, 500)/500;
            transform.position = transform.position + transform.forward * (Time.deltaTime * camMoveSpeed)*rate;
        }
        if (Input.GetKey(KeyCode.S))
        {
            float rate = Mathf.Clamp(transform.position.y, 0, 500) / 500;
            transform.position = transform.position - transform.forward * (Time.deltaTime * camMoveSpeed) * rate;
        }
        if (Input.GetKey(KeyCode.A))
        {
            float rate = Mathf.Clamp(transform.position.y, 0, 500) / 500;
            transform.position = transform.position - transform.right * (Time.deltaTime * camMoveSpeed) * rate;
        }
        if (Input.GetKey(KeyCode.D))
        {
            float rate = Mathf.Clamp(transform.position.y, 0, 500) / 500;
            transform.position = transform.position + transform.right * (Time.deltaTime * camMoveSpeed) * rate;
        }
        if (Input.GetKey(KeyCode.Q))
        {
            float rate = Mathf.Clamp(transform.position.y, 0, 500) / 500;
            transform.position = transform.position - transform.up * (Time.deltaTime * camMoveSpeed) * rate;
        }
        if (Input.GetKey(KeyCode.E))
        {
            float rate = Mathf.Clamp(transform.position.y, 0, 500) / 500;
            transform.position = transform.position + transform.up * (Time.deltaTime * camMoveSpeed) * rate;
        }
    }

    void dragCamera()
    {
        Vector3 dragDir = Input.GetAxis("Mouse Y") * transform.up + Input.GetAxis("Mouse X") * transform.right;
        transform.position = transform.position - dragDir * Time.deltaTime * camDragSpeed;
    }

    void moveCamera()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            float rate = Mathf.Clamp(transform.position.y, 0, 500) / 500;
            Vector3 camdir = new Vector3(transform.forward.x, 0, transform.forward.z);
            transform.position = transform.position + camdir.normalized * (Time.deltaTime * camMoveSpeed) * rate;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            float rate = Mathf.Clamp(transform.position.y, 0, 500) / 500;
            Vector3 camdir = new Vector3(-transform.forward.x, 0, -transform.forward.z);
            transform.position = transform.position + camdir.normalized * (Time.deltaTime * camMoveSpeed) * rate;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            float rate = Mathf.Clamp(transform.position.y, 0, 500) / 500;
            transform.position = transform.position - transform.right.normalized * (Time.deltaTime * camMoveSpeed) * rate;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            float rate = Mathf.Clamp(transform.position.y, 0, 500) / 500;
            transform.position = transform.position + transform.right.normalized * (Time.deltaTime * camMoveSpeed) * rate;
        }
    }
}
