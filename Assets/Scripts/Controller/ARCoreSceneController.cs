using System.Collections.Generic;
using IMAV.Model;
using GoogleARCore;
using GoogleARCoreInternal;
using UnityEngine;
using IMAV.Service;
using IMAV.Util;

namespace IMAV.Controller
{
    public class ARCoreSceneController : SceneController
    {
        public GameObject m_trackedPlanePrefab;
        private List<TrackedPlane> m_allPlanes = new List<TrackedPlane>();
        private List<TrackedPlane> m_newPlanes = new List<TrackedPlane>();
        const int LOST_TRACKING_SLEEP_TIMEOUT = 15;
        bool tracking = false;

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

        private void Update()
        {
            _QuitOnConnectionErrors();

            if (Frame.TrackingState != FrameTrackingState.Tracking)
            {
                Screen.sleepTimeout = LOST_TRACKING_SLEEP_TIMEOUT;
                return;
            }

            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            Frame.GetNewPlanes(ref m_newPlanes);

            // Iterate over planes found in this frame and instantiate corresponding GameObjects to visualize them.
            for (int i = 0; i < m_newPlanes.Count; i++)
            {
                GameObject planeObject = Instantiate(m_trackedPlanePrefab, Vector3.zero, Quaternion.identity,
                    transform);
                planeObject.GetComponent<ARCoreTrackedPlane>().SetTrackedPlane(m_newPlanes[i]);
            }

            Frame.GetAllPlanes(ref m_allPlanes);
            for (int i = 0; i < m_allPlanes.Count; i++)
            {
                if (m_allPlanes[i].IsValid)
                {
                    hideInform();
                    return;
                }
            }
            showInform(ARConstantValue.AR_FINDPLANE);
        }

        /// <summary>
        /// Quit the application if there was a connection error for the ARCore session.
        /// </summary>
        private void _QuitOnConnectionErrors()
        {
            // Do not update if ARCore is not tracking.
            if (Session.ConnectionState == SessionConnectionState.DeviceNotSupported)
            {
                _ShowAndroidToastMessage("This device does not support ARCore.");
                Application.Quit();
            }
            else if (Session.ConnectionState == SessionConnectionState.UserRejectedNeededPermission)
            {
                _ShowAndroidToastMessage("Camera permission is needed to run this application.");
                Application.Quit();
            }
            else if (Session.ConnectionState == SessionConnectionState.ConnectToServiceFailed)
            {
                _ShowAndroidToastMessage("ARCore encountered a problem connecting.  Please start the app again.");
                Application.Quit();
            }
        }

        /// <summary>
        /// Show an Android toast message.
        /// </summary>
        /// <param name="message">Message string to show in the toast.</param>
        /// <param name="length">Toast message time length.</param>
        private static void _ShowAndroidToastMessage(string message)
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

            if (unityActivity != null)
            {
                AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
                unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
                {
                    AndroidJavaObject toastObject = toastClass.CallStatic<AndroidJavaObject>("makeText", unityActivity,
                        message, 0);
                    toastObject.Call("show");
                }));
            }
        }

        public override void ResetARSession()
        {
            Pause();
        }

        public override void AddProductToScene(Product p)
        {
            GameObject obj = Instantiate(p.model);
            obj.transform.position = Vector3.zero;
            ProductController ctrl = obj.AddComponent<ProductController>();
            ctrl.SetTouchService(new ARCoreTouchService(ctrl));
            objlist.Add(ctrl);
            SetCurrentProduct(ctrl);
        }

        public override void Pause()
        {
            if (SessionManager.ConnectionState != SessionConnectionState.Connected)
            {
                return;
            }

            SessionManager.Instance.OnApplicationPause(true);
        }

        public override void Resume()
        {
            if (SessionManager.ConnectionState != SessionConnectionState.Connected)
            {
                return;
            }

            SessionManager.Instance.OnApplicationPause(false);
        }
    }
}
