using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.iOS;
using IMAV.Controller;
using System;

namespace IMAV.Service
{
    public class ARkitTouchService : TouchService
    {
        public ARkitTouchService(ProductController ctrl) : base(ctrl)
        {
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

        public override void Start()
        {
            PlaceOnPlane(new Vector2(0.5f, 0.5f));
            controller.transform.LookAt(Camera.main.transform);
            controller.transform.rotation = Quaternion.Euler(0.0f, controller.transform.rotation.eulerAngles.y, controller.transform.rotation.z);
        }

        protected override void MoveTouch(Vector3 pos)
        {
            var screenPosition = Camera.main.ScreenToViewportPoint(pos);
            PlaceOnPlane(screenPosition);
        }

        protected void PlaceOnPlane(Vector2 screenPosition)
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
