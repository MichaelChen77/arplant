using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.iOS;
using IMAV.Controller;

namespace IMAV.Service
{
    public class ARkitTouchService : ITouchService
    {
        ProductController controller;
        Vector2 offset;

        public ARkitTouchService(ProductController ctrl)
        {
            controller = ctrl;
        }

        bool HitTestWithResultType(ARPoint point, ARHitTestResultType resultTypes)
        {
            List<ARHitTestResult> hitResults = UnityARSessionNativeInterface.GetARSessionNativeInterface().HitTest(point, resultTypes);
            if (hitResults.Count > 0)
            {
                foreach (var hitResult in hitResults)
                {
                    Debug.Log("Unity: Got hit!");
                    controller.transform.position = UnityARMatrixOps.GetPosition(hitResult.worldTransform);
                    //controller.transform.rotation = UnityARMatrixOps.GetRotation(hitResult.worldTransform);
                    Debug.Log("Unity: " + controller.transform.position + " ; " + controller.transform.eulerAngles);
                    return true;
                }
            }
            return false;
        }

		void processRotate(Touch fing1, Touch fing2)
		{
            if (fing1.phase == TouchPhase.Moved && fing2.phase == TouchPhase.Moved)
            {
                float deltaRot = fing1.deltaPosition.x * DataUtility.ProductRotateSpeed * Mathf.Deg2Rad * Time.deltaTime;
                controller.transform.Rotate(0, -deltaRot, 0);
            }
		}

        public void Start()
        {
            PlaceOnPlane(new Vector2(0.5f, 0.5f));
        }

        float touchTime = 0;
        bool beginTouch = false;
        public void Update()
        {
            if (Input.touchCount == 1)
            {
                var touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began)
                {
                    beginTouch = true;
                    touchTime = 0;
                    Vector3 pos = Camera.main.WorldToScreenPoint(controller.transform.position);
                    offset = new Vector2(pos.x - touch.position.x, pos.y - touch.position.y);
                }
                if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved)
                {
                    var screenPosition = Camera.main.ScreenToViewportPoint(touch.position + offset);
                    PlaceOnPlane(screenPosition);
                }
                else if (beginTouch && touch.phase == TouchPhase.Stationary)
                {
                    touchTime += touch.deltaTime;
                    if (touchTime > 1f)
                    {
                        SceneController.Singleton.ChooseProduct(!controller.Choosed);
                        beginTouch = false;
                    }
                }
            }
            else if (Input.touchCount == 2)
            {
                processRotate(Input.GetTouch(0), Input.GetTouch(1));
            }
        }

        void PlaceOnPlane(Vector2 screenPosition)
        {
			ARPoint point = new ARPoint
			{
				x = screenPosition.x,
				y = screenPosition.y
			};
			// prioritize reults types
			ARHitTestResultType[] resultTypes = {
						//ARHitTestResultType.ARHitTestResultTypeExistingPlaneUsingExtent, 
                        // if you want to use infinite planes use this:
                        ARHitTestResultType.ARHitTestResultTypeExistingPlane,
                        ARHitTestResultType.ARHitTestResultTypeHorizontalPlane,
						ARHitTestResultType.ARHitTestResultTypeFeaturePoint
					};

			foreach (ARHitTestResultType resultType in resultTypes)
			{
				if (HitTestWithResultType(point, resultType))
				{
					return;
				}
			}
        }

    }
}
