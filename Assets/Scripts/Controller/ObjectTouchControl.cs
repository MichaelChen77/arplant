using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IMAV
{
    public class ObjectTouchControl : MonoBehaviour
    {
#if UNITY_ANDROID || UNITY_IOS

        //public KudanTracker _kudanTracker;	// The tracker to be referenced in the inspector. This is the Kudan Camera object.

        /// <summary>
        /// The rate at which the pinch control scales the object.
        /// </summary>
        float zoomSpeed;

        /// <summary>
        /// The rate at which the swipe control rotates the object.
        /// </summary>
        float moveSpeed;

        /// <summary>
        /// The amount that the finger can move on the screen before it is considered to be a movement and not a tap.
        /// </summary>
        float roughDiff;

        /// <summary>
        /// Save the orignalSize.
        /// </summary>
        float orignalSize;

        /// <summary>
        /// The time duration that a finger touched on the screen.
        /// </summary>
        float touchDuration;

        /// <summary>
        /// The touch of finger.
        /// </summary>
        Touch touch;
        int prevTrouchConstraint = 0;
        Quaternion originalRot;
        float rotSpeed = 6f;
        bool inverseXY = false;

        /// <summary>
        /// The script that activates when the sreen is tapped.
        /// </summary>
        //MarklessActivated1Script activate1;
        /// <summary>
        /// The script that activates when the sreen is tapped.
        /// </summary>
        //MarklessActivated2Script activate2;

        /// <summary>
        /// Start this instance.
        /// </summary>
        void Start()
        {
            zoomSpeed = 0.5f;
            moveSpeed = 8f;
            roughDiff = 3f;
            orignalSize = this.transform.localScale.x;
            originalRot = transform.localRotation;
            if (tag == "Inverse")
                inverseXY = true;
            else
                inverseXY = false;
            //		activate1 = this.GetComponent<MarklessActivated1Script>();
            //		activate1.enabled = false;
            //		activate2 = this.GetComponent<MarklessActivated2Script>();
            //		activate2.enabled = false;
        }

        /// <summary>
        /// Update this instance.
        /// </summary>
        void Update()
        {
            TouchModeUpdate();
            processDrag();
            processZoom();
            processTap();
        }



        /// <summary>
        /// Checks for pinch controls.
        /// </summary>
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
                    float deltaDistance = prevTouchDeltaMag - touchDeltaMag;

                    //Create appropriate vector
                    float scaleChange = this.transform.localScale.x - this.transform.localScale.x * deltaDistance * zoomSpeed / 200;

                    //To avoid the scale being negative
                    if (scaleChange < 0)
                    {
                        scaleChange = 0;
                    }

                    //Scale object
                    this.transform.localScale = new Vector3(scaleChange, scaleChange, scaleChange);
                }
            }
        }


        /// <summary>
        /// Checks for drag controls.
        /// </summary>
        void processDrag()
        {
            if (Input.touchCount == 1)
            {
                //Store input
                Touch fing = Input.GetTouch(0);
                if (ResourceManager.Singleton.touchMove)
                {
                    Drag(fing, ResourceManager.Singleton.constraintID);
                }
                else {
                    Rotate(fing, ResourceManager.Singleton.constraintID);
                }
            }
        }
        //		if (Input.touchCount == 2)
        //		{
        //			//Store inputs
        //			Touch fing1 = Input.GetTouch (0);
        //			Touch fing2 = Input.GetTouch (1);
        //
        //			if (fing1.phase == TouchPhase.Moved || fing2.phase == TouchPhase.Moved)	//If either finger has moved since the last frame
        //			{
        //				//Get previous positions
        //				Vector2 fing1Prev = fing1.position - fing1.deltaPosition;
        //				Vector2 fing2Prev = fing2.position - fing2.deltaPosition;
        //
        //				//Find vector magnitude between touches in each frame
        //				float prevTouchDeltaMag = (fing1Prev - fing2Prev).magnitude;		
        //				float touchDeltaMag = (fing1.position - fing2.position).magnitude;
        //
        //				//Find difference in distances
        //				float deltaDistance = prevTouchDeltaMag - touchDeltaMag;
        //
        //				if (deltaDistance < 2 * roughDiff) {
        //					//translate object
        //					Vector2 fingMove = fing1.deltaPosition;
        //					Vector3 deltMove;
        //
        //					float[] result1 = new float[7];
        //					Kudan.AR.NativeInterface.ArbiTrackGetPose(result1);
        //					Quaternion ARBorientation = new Quaternion(result1[3], result1[4], result1[5], result1[6]); // The current orientation of the floor in 3D space, relative to the devic
        //					Vector3 rotation = ARBorientation.eulerAngles;
        //
        //					deltMove.x = 0;
        //					deltMove.y = 0;
        //					deltMove.z = fingMove.y * moveSpeed * Time.deltaTime;
        //					deltMove = Quaternion.Euler(0, -rotation.y, 0) * deltMove;
        //
        //					//Create appropriate vector, y is height in local the position
        //					//When scale larger, move larger. Scale smaller, move smaller
        //					float positionChangeX = this.transform.localPosition.x + this.transform.localScale.x * deltMove.x / orignalSize;
        //					float positionChangeY = this.transform.localPosition.y + this.transform.localScale.x * deltMove.y / orignalSize;
        //					float positionChangeZ = this.transform.localPosition.z + this.transform.localScale.x * deltMove.z / orignalSize;
        //
        //					this.transform.localPosition = new Vector3 (positionChangeX, positionChangeY, positionChangeZ);
        //					//Debug.Log ("change Z: " + transform.localPosition + " ; " + transform.position);
        //				}
        //			}

        void Drag(Touch fing, int constrainted)
        {
            if (fing.phase == TouchPhase.Moved)
            {
                Vector2 fingMove = fing.deltaPosition;
                Vector3 deltMove = Vector3.zero;

                //float[] result1 = new float[7];
                //Kudan.AR.TrackerAndroid.ArbiTrackGetPose (result1);
                //Quaternion ARBorientation = new Quaternion (result1 [3], result1 [4], result1 [5], result1 [6]); // The current orientation of the floor in 3D space, relative to the devic
                //Vector3 rotation = ARBorientation.eulerAngles;
                //Debug.Log ("Rotation: " + rotation);
                AndroidJavaClass kudanArClass = new AndroidJavaClass("eu.kudan.androidar.KudanAR");
                AndroidJavaObject m_KudanAR_Instance = kudanArClass.CallStatic<AndroidJavaObject>("getInstance");
                Quaternion orientation = new Quaternion();
                if (m_KudanAR_Instance != null)
                {
                    m_KudanAR_Instance.Call("updateArbi", 200.0f);

                    AndroidJavaObject arbiPosition = m_KudanAR_Instance.Get<AndroidJavaObject>("m_ArbiPosition");
                    AndroidJavaObject arbiOrientation = m_KudanAR_Instance.Get<AndroidJavaObject>("m_ArbiOrientation");

                    orientation.x = arbiOrientation.Get<float>("x");
                    orientation.y = arbiOrientation.Get<float>("y");
                    orientation.z = arbiOrientation.Get<float>("z");
                    orientation.w = arbiOrientation.Get<float>("w");
                }
                Vector3 rotation = orientation.eulerAngles;

                switch (constrainted)
                {
                    case 0:
                        deltMove.x = fingMove.x * moveSpeed * Time.deltaTime;
                        deltMove.y = fingMove.y * moveSpeed * Time.deltaTime;
                        deltMove.z = 0;
                        break;
                    case 1:
                        deltMove.x = 0;
                        deltMove.y = 0;
                        deltMove.z = fingMove.y * moveSpeed * Time.deltaTime;
                        break;
                    case 2:
                        deltMove.x = fingMove.x * moveSpeed * Time.deltaTime;
                        deltMove.y = 0;
                        deltMove.z = 0;
                        break;
                    case 3:
                        deltMove.x = 0;
                        deltMove.y = fingMove.y * moveSpeed * Time.deltaTime;
                        deltMove.z = 0;
                        break;
                }
                deltMove = Quaternion.Euler(0, -rotation.y, 0) * deltMove;

                //float posChangeX, posChangeY, posChangeZ;
                //			switch (Screen.orientation) {
                //			case ScreenOrientation.Portrait:
                //				posChangeX = this.transform.localPosition.x - deltMove.y;
                //				posChangeY = this.transform.localPosition.y + deltMove.x;
                //				break;
                //			case ScreenOrientation.LandscapeRight:
                //				posChangeX = this.transform.localPosition.x - deltMove.x;
                //				posChangeY = this.transform.localPosition.y - deltMove.y;
                //				break;
                //			case ScreenOrientation.PortraitUpsideDown:
                //				posChangeX = this.transform.localPosition.x + deltMove.y;
                //				posChangeY = this.transform.localPosition.y - deltMove.x;
                //				break;
                //			default: 
                //				posChangeX = this.transform.localPosition.x + deltMove.x;
                //				posChangeY = this.transform.localPosition.y + deltMove.y;
                //				break;
                //			}
                //			posChangeZ = this.transform.localPosition.z + deltMove.z;

                //Create appropriate vector, y is height in local the position
                //When scale larger, move larger. Scale smaller, move smaller
                float posChangeX = this.transform.localPosition.x + deltMove.x * transform.position.z * 0.01f;
                float posChangeY = this.transform.localPosition.y + deltMove.y * transform.position.z * 0.01f;
                float posChangeZ = this.transform.localPosition.z + deltMove.z * transform.position.z * 0.01f;

                this.transform.localPosition = new Vector3(posChangeX, posChangeY, posChangeZ);
            }
        }

        void Rotate(Touch fing, int constrainted)
        {
            if (fing.phase == TouchPhase.Moved)
            {
                Vector2 deltaRot = fing.deltaPosition * rotSpeed * Mathf.Deg2Rad;
                //			Vector3 axisX, axisY;
                //			switch (Screen.orientation) {
                //			case ScreenOrientation.Portrait:
                //				axisX = Vector3.left;
                //				axisY = Vector3.up;
                //				break;
                //			case ScreenOrientation.LandscapeRight:
                //				axisX = Vector3.down;
                //				axisY = Vector3.left;
                //				break;
                //			case ScreenOrientation.PortraitUpsideDown:
                //				axisX = Vector3.right;
                //				axisY = Vector3.down;
                //				break;
                //			default:
                //				axisX = Vector3.up;
                //				axisY = Vector3.right;
                //				break;
                //			}
                Vector3 axisX = Vector3.up;
                Vector3 axisY = Vector3.right;
                switch (constrainted)
                {
                    case 0:
                        deltaRot = deltaRot * Time.deltaTime;
                        transform.RotateAround(axisX, -deltaRot.x);
                        transform.RotateAround(axisY, deltaRot.y);
                        break;
                    case 1:
                        if (resetRot)
                        {
                            transform.localRotation = originalRot;
                            //transform.localEulerAngles = new Vector3 (-90, transform.localEulerAngles.y, transform.localEulerAngles.z);
                            resetRot = false;
                        }
                        else {
                            if (inverseXY)
                                transform.Rotate(0, 0, -deltaRot.x);
                            else
                                transform.Rotate(0, -deltaRot.x, 0);
                        }
                        break;
                    case 2:
                        if (inverseXY)
                            transform.Rotate(0, -deltaRot.y, 0);
                        else
                            transform.Rotate(0, 0, -deltaRot.y);
                        break;
                    case 3:
                        transform.Rotate(deltaRot.y, 0, 0, Space.World);
                        break;
                }
                //Debug.Log ("#rotation: " + transform.rotation.eulerAngles+" delta: "+deltaRot+" final: "+deltaRot*Time.deltaTime+ " ; "+constrainted);
            }
        }

        bool resetRot = false;
        void TouchModeUpdate()
        {
            if (!ResourceManager.Singleton.touchMove && ResourceManager.Singleton.constraintID != prevTrouchConstraint)
            {
                prevTrouchConstraint = ResourceManager.Singleton.constraintID;
                if (prevTrouchConstraint == 1)
                {
                    resetRot = true;
                }
            }
        }

        void OnMouseDown()
        {
            ResourceManager.Singleton.SetCurrentObject(gameObject);
        }

        void processTap()
        {
            if (Input.touchCount == 1) //if there is a touch
            {
                touchDuration += Time.deltaTime;
                touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Ended && touchDuration < 0.2f) //making sure it only check the touch once && it was a short touch/tap and not a dragging.
                    StartCoroutine("singleOrDouble");
            }
            else
                touchDuration = 0.0f;
        }

        IEnumerator singleOrDouble()
        {
            yield return new WaitForSeconds(0.3f);
            if (touch.tapCount == 1)
                Debug.Log("Single");
            else if (touch.tapCount == 2)
            {
                //this coroutine has been called twice. We should stop the next one here otherwise we get two double tap
                StopCoroutine("singleOrDouble");
                Debug.Log("Double");
            }
        }
#endif
    }
}