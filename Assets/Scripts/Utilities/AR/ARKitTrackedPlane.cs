using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.iOS;

namespace IMAV.Util
{
    public class ARKitTrackedPlane : MonoBehaviour
    {
        MeshRenderer m_meshRenderer;

        void Start()
        {
            m_meshRenderer = GetComponent<MeshRenderer>();
        }

        public void Show()
        {
            m_meshRenderer.enabled = true;
        }

		//protected void PlaceOnPlane(Vector2 screenPosition)
		//{
		//	ARPoint point = new ARPoint
		//	{
		//		x = screenPosition.x,
		//		y = screenPosition.y
		//	};
		//	// prioritize reults types
		//	ARHitTestResultType[] resultTypes = {
  //                      //ARHitTestResultType.ARHitTestResultTypeExistingPlaneUsingExtent, 
  //                      // if you want to use infinite planes use this:
  //                      ARHitTestResultType.ARHitTestResultTypeExistingPlane,
		//				ARHitTestResultType.ARHitTestResultTypeHorizontalPlane,
		//				ARHitTestResultType.ARHitTestResultTypeFeaturePoint
		//			};

		//	foreach (ARHitTestResultType resultType in resultTypes)
		//	{
		//		if (HitTestWithResultType(point, resultType))
		//		{
		//			return;
		//		}
		//	}
		//}

		//bool HitTestWithResultType(ARPoint point, ARHitTestResultType resultTypes)
		//{
		//	List<ARHitTestResult> hitResults = UnityARSessionNativeInterface.GetARSessionNativeInterface().HitTest(point, resultTypes);
		//	if (hitResults.Count > 0)
		//	{
		//		foreach (var hitResult in hitResults)
		//		{
		//			Debug.Log("Unity: Got hit!");
		//			controller.transform.position = UnityARMatrixOps.GetPosition(hitResult.worldTransform);
		//			//controller.transform.rotation = UnityARMatrixOps.GetRotation(hitResult.worldTransform);
		//			Debug.Log("Unity: " + controller.transform.position + " ; " + controller.transform.eulerAngles);
		//			return true;
		//		}
		//	}
		//	return false;
		//}
    }
}
