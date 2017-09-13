using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IMAV.Model;
using IMAV.Service;
using UnityEngine.XR.iOS;

namespace IMAV.Controller
{
    public class ARKitSceneController : SceneController
    {
        UnityARSessionNativeInterface m_session;
        ARKitWorldTackingSessionConfiguration m_arconfig;

        protected override void Awake()
        {
            base.Awake();
            if (mSingleton != null)
            {
                Destroy(gameObject);
            }
            else
            {
                mSingleton = this;
            }
        }

        // Use this for initialization
        void Start()
        {
#if !UNITY_EDITOR
            Application.targetFrameRate = 60;
            Debug.Log("Unity: scenecontorller0");
            InitARConfig();
            Debug.Log("Unity: scenecontorller1");
            m_session = UnityARSessionNativeInterface.GetARSessionNativeInterface();
            Debug.Log("Unity: scenecontorller2");
            m_session.RunWithConfig(m_arconfig);
            Debug.Log("Unity: scenecontorller3");
            UnityARSessionNativeInterface.ARSessionFailedEvent += ARSessionFailed;
            UnityARSessionNativeInterface.ARSessionTrackingChangedEvent += UpdateSessionState;
            UnityARSessionNativeInterface.ARAnchorAddedEvent += ARPlaneAnchorAdded;
#endif
		}

        void InitARConfig()
        {
            m_arconfig = new ARKitWorldTackingSessionConfiguration();
			m_arconfig.planeDetection = UnityARPlaneDetection.Horizontal;
			m_arconfig.alignment = UnityARAlignment.UnityARAlignmentGravity;
			m_arconfig.getPointCloudData = true;
			m_arconfig.enableLightEstimation = true;
        }

		void ARSessionFailed(string error)
		{
			showInform("AR Session Failed: " + error);
		}

		void ARPlaneAnchorAdded(ARPlaneAnchor anchorData)
		{
			showInform(ARConstantValue.AR_PLANEDETECTED, true);
		}

		public override void ResetARSession()
		{
            m_session.RunWithConfigAndOptions(m_arconfig, UnityARSessionRunOption.ARSessionRunOptionRemoveExistingAnchors);
		}

        void UpdateSessionState(UnityARCamera cam)
        {
            switch (cam.trackingState)
            {
                case ARTrackingState.ARTrackingStateNormal: showInform(ARConstantValue.AR_NORMAL, true); break;
                case ARTrackingState.ARTrackingStateLimited:
                    {
                        switch (cam.trackingReason)
                        {
                            case ARTrackingStateReason.ARTrackingStateReasonExcessiveMotion: showInform(ARConstantValue.AR_EXCESSIVEMOTION); break;
                            case ARTrackingStateReason.ARTrackingStateReasonInsufficientFeatures: showInform(ARConstantValue.AR_INSUFFICIENTFEATURES); break;
                            case ARTrackingStateReason.ARTrackingStateReasonInitializing: showInform(ARConstantValue.AR_INITIALIZING, true); break;
                            case ARTrackingStateReason.ARTrackingStateReasonNone: MediaController.Singleton.textInform.Hide(); break;
                        }
                        break;
                    }
            }
        }

		public override void AddProductToScene(Product p)
		{
			GameObject obj = Instantiate(p.model, Vector3.zero, Quaternion.identity);
			ProductController ctrl = obj.AddComponent<ProductController>();
			ctrl.SetTouchService(new ARkitTouchService(ctrl));
			objlist.Add(ctrl);
            SetCurrentProduct(ctrl);
		}

        public override void Pause()
        {
            m_session.Pause();
        }

        public override void Resume()
        {
            m_session.Run();
        }
    }
}
