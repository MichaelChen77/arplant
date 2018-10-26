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

        public void SwitchTrackPlane()
        {
            showTrackedPlane = !showTrackedPlane;
			ShowTrackPlane (showTrackedPlane);
        }

		public void ShowTrackPlane(bool flag)
		{
            showTrackedPlane = flag;
			List<ARPlaneAnchorGameObject> arpags = unityARAnchorManager.GetCurrentPlaneAnchors();
			foreach (ARPlaneAnchorGameObject p in arpags)
			{
                MeshRenderer render = p.gameObject.GetComponentInChildren<MeshRenderer>();
				if (render != null)
                    render.material = showTrackedPlane ? gridMat : shadowMat;
			}
            MeshRenderer prefabRender = planePrefab.GetComponentInChildren<MeshRenderer>();
            prefabRender.material = showTrackedPlane ? gridMat : shadowMat;
		}

		void OnDestroy()
		{
			unityARAnchorManager.Destroy ();
		}
	}
}

