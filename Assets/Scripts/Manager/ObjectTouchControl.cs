using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IMAV
{
    public class ObjectTouchControl : MonoBehaviour
    {
        //public KudanTracker _kudanTracker;	// The tracker to be referenced in the inspector. This is the Kudan Camera object.

        /// <summary>
        /// The rate at which the pinch control scales the object.
        /// </summary>
        public float zoomSpeed = 0.5f;

        /// <summary>
        /// The rate at which the swipe control moves the object.
        /// </summary>
        public float moveSpeed = 12f;

		/// <summary>
		/// The rate at which the swipe control rotates the object.
		/// </summary>
		public float rotSpeed = 15f;

		ARModel target;
		bool startDrag = false;

        //float heightPos;

        /// <summary>
        /// Start this instance.
        /// </summary>
		public void Init(ARModel model)
		{
			target = model;
		}

        /// <summary>
        /// Update this instance.
        /// </summary>
        void Update()
        {
            if (target.Selected == SelectState.Actived)
            {
                if (Input.touchCount == 1) {
					Drag (Input.GetTouch (0));
				} else if (Input.touchCount == 2) {
					processZoomAndRotate (Input.GetTouch (0), Input.GetTouch (1));
				}
            }
        }
			
		void processZoomAndRotate (Touch fing1, Touch fing2)
		{
			if (fing1.phase == TouchPhase.Moved && fing2.phase == TouchPhase.Moved) {
				float delta1 = fing1.deltaPosition.sqrMagnitude;
				float delta2 = fing2.deltaPosition.sqrMagnitude;
				if (delta1 > 1f && delta2 > 1f) {	//If either finger has moved since the last frame
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
					if (scaleChange < 1) {
						scaleChange = 1;
					}
                    //float rate = scaleChange / transform.localScale.x;
                    //transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y * rate, transform.localPosition.z);
                    //heightPos = heightPos * rate;
					this.transform.localScale = new Vector3 (scaleChange, scaleChange, scaleChange);
				} else {
					if (delta1 > 1f)
						TouchRotate (fing1);
					else
						TouchRotate (fing2);
				}
			} else if (fing1.phase == TouchPhase.Moved || fing2.phase == TouchPhase.Moved) {
				if (fing1.phase != TouchPhase.Moved)
					TouchRotate (fing2);
				else
					TouchRotate (fing1);
			}
		}

		void TouchRotate(Touch fing)
		{
			float deltaRot = fing.deltaPosition.sqrMagnitude * rotSpeed * Mathf.Deg2Rad * Time.deltaTime;
			transform.Rotate (0, 0, -deltaRot);
		}

        void Drag(Touch fing)
        {
			if (fing.phase == TouchPhase.Began)
				startDrag = true;
			else if (fing.phase == TouchPhase.Ended)
				startDrag = false;
			else if (startDrag && fing.phase == TouchPhase.Moved)
            {
                Vector2 fingMove = fing.deltaPosition;
#if !UNITY_EDITOR && UNITY_ANDROID
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
#endif
                //float deltMoveZ = fingMove.y * moveSpeed * Time.deltaTime *10;
                //float positionChangeZ = this.transform.position.z + deltMoveZ /10;

                //Vector3 _pos = new Vector3 (fing.position.x, fing.position.y, positionChangeZ);
                //Vector3 pos1 = Camera.main.ScreenToWorldPoint (_pos);
                //pos1.z = positionChangeZ;
                //transform.position = pos1;

                //hPlane = new Plane(ResourceManager.Singleton.markerlessTransform.position.normalized, transform.position);
                
                Ray ray = Camera.main.ScreenPointToRay(new Vector3(fing.position.x, fing.position.y, 0));
                RaycastHit hit;
                float f = transform.localPosition.y;
                if (Physics.Raycast(ray, out hit, float.MaxValue, 1<<11))
                {
                    if (hit.collider != null)
                    {
                        transform.position = hit.point;
                        transform.localPosition = new Vector3(transform.localPosition.x, f, transform.localPosition.z);
                    }
                    //transform.position = ray.GetPoint(rayDistance);
                    //ResourceManager.Singleton.DebugString("Plane: " + hPlane.normal + " ; " + transform.position + " ; ");
                }
            }
        }
    }
}