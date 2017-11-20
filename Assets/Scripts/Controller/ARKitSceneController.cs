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
        ARKitWorldTrackingSessionConfiguration m_arconfig;

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
            InitARConfig();
            m_session = UnityARSessionNativeInterface.GetARSessionNativeInterface();
            m_session.RunWithConfig(m_arconfig);
            UnityARSessionNativeInterface.ARSessionFailedEvent += ARSessionFailed;
            UnityARSessionNativeInterface.ARSessionTrackingChangedEvent += ARSessionTrackingChanged;
            UnityARSessionNativeInterface.ARAnchorAddedEvent += ARPlaneAnchorAdded;
#endif
        }

        void InitARConfig()
        {
            m_arconfig = new ARKitWorldTrackingSessionConfiguration();
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

        void ARSessionTrackingChanged(UnityARCamera arcam)
        {
            try
            {
                switch (arcam.trackingState)
                {
                    case ARTrackingState.ARTrackingStateNotAvailable: showTrackingReason(arcam.trackingReason); break;
                    case ARTrackingState.ARTrackingStateNormal: showInform(ARConstantValue.AR_NORMAL, true); break;
                    case ARTrackingState.ARTrackingStateLimited: showTrackingReason(arcam.trackingReason); break;
                }
            }
            catch (System.Exception ex)
            {
                Debug.Log("Unity: tracking changed error - " + ex.Message);
            }
        }

        void showTrackingReason(ARTrackingStateReason res)
        {
            switch (res)
            {
                case ARTrackingStateReason.ARTrackingStateReasonExcessiveMotion: showInform(ARConstantValue.AR_EXCESSIVEMOTION); break;
                case ARTrackingStateReason.ARTrackingStateReasonInsufficientFeatures: showInform(ARConstantValue.AR_INSUFFICIENTFEATURES); break;
                case ARTrackingStateReason.ARTrackingStateReasonInitializing: showInform(ARConstantValue.AR_INITIALIZING, true); break;
                case ARTrackingStateReason.ARTrackingStateReasonNone: MediaController.Singleton.textInform.Hide(); break;
            }
        }

        public override void ResetARSession()
        {
            m_session.RunWithConfigAndOptions(m_arconfig, UnityARSessionRunOption.ARSessionRunOptionRemoveExistingAnchors);
        }


        public override void AddProductToScene(Product p)
        {
            GameObject obj = Instantiate(p.model);
            obj.transform.position = Vector3.zero;
            ProductController ctrl = obj.AddComponent<ProductController>();
            ctrl.SetTouchService(new ARkitTouchService(ctrl));
            objlist.Add(ctrl);
            SetCurrentProduct(ctrl);
        }

        public override void Pause()
        {
            if (m_session != null)
                m_session.Pause();
        }

        public override void Resume()
        {
            if (m_session != null)
                m_session.Run();
        }
    }
}
