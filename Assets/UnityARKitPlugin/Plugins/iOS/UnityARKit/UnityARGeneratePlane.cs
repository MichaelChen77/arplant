using System;
using System.Collections.Generic;

namespace UnityEngine.XR.iOS
{
	public class UnityARGeneratePlane : MonoBehaviour
	{
		public Material shadowMat;
		public Material gridMat;
		public GameObject planePrefab;
		public bool showTrackedPlane = false;
		private UnityARAnchorManager unityARAnchorManager;

		// Use this for initialization
		void Start () {
			unityARAnchorManager = new UnityARAnchorManager();
			UnityARUtility.InitializePlanePrefab (planePrefab);
		}

		public void ShowTrackPlane()
		{
			showTrackedPlane = !showTrackedPlane;
			List<ARPlaneAnchorGameObject> arpags = unityARAnchorManager.GetCurrentPlaneAnchors();
			foreach (ARPlaneAnchorGameObject p in arpags)
			{
				MeshRenderer render = p.gameObject.GetComponent<MeshRenderer>();
				if (render != null)
					render.enabled = showTrackedPlane ? gridMat : shadowMat;
			}
			MeshRenderer prefabRender = planePrefab.GetComponent<MeshRenderer>();
			prefabRender.enabled = showTrackedPlane ? gridMat : shadowMat;
		}

		void OnDestroy()
		{
			unityARAnchorManager.Destroy ();
		}
	}
}

