using UnityEngine;
using System.Text;
using System.Collections;

namespace Kudan.AR
{
	[AddComponentMenu("Kudan AR/Transform Drivers/Markerless Driver")]
	/// <summary>
	/// The Markerless Transform Driver, which is moved by the tracker when using the Markerless Tracking Method.
	/// </summary>
	public class MarkerlessTransformDriver : TransformDriverBase
	{
		/// <summary>
		/// Reference to the Markerless Tracking Method.
		/// </summary>
		private TrackingMethodMarkerless _tracker;

		/// <summary>
		/// Finds the tracker.
		/// </summary>
		protected override void FindTracker()
		{
			_trackerBase = _tracker = (TrackingMethodMarkerless)Object.FindObjectOfType(typeof(TrackingMethodMarkerless));
		}

		/// <summary>
		/// Register this instance with the Tracking Method class for event handling.
		/// </summary>
		protected override void Register()
		{
			if (_tracker != null)
			{
				_tracker._updateMarkerEvent.AddListener(OnTrackingUpdate);
			}
            this.gameObject.SetActive(false);

		}

		/// <summary>
		/// Unregister this instance with the Tracking Method class for event handling.
		/// </summary>
		protected override void Unregister()
		{
			if (_tracker != null)
			{
				_tracker._updateMarkerEvent.RemoveListener(OnTrackingUpdate);
			}
		}

        /// <summary>
        /// Method called every frame ArbiTrack is running.
        /// Updates the position and orientation of the trackable.
        /// </summary>
        /// <param name="trackable">Trackable.</param>
        public void OnTrackingUpdate(Trackable trackable)
        {

            filterUpdate(trackable);
            if (IMAV.DataUtility.TrackingMode == ARTrackingMode.Placement)
                gameObject.SetActive(true);
            else
                this.gameObject.SetActive(trackable.isDetected);
        }

        Vector3 frameDiff;
        float angleDiff;
        float distanceDiff;

        string str;
        public string DiffString
        {
            get { str = frameDiff + " ; " + distanceDiff + " ; " + angleDiff; return str; }
        }
        void filterUpdate(Trackable trackable)
        {
            frameDiff = transform.localPosition - trackable.position;
            distanceDiff = Vector3.Distance(transform.localPosition, trackable.position);
            angleDiff = Quaternion.Angle(transform.localRotation, trackable.orientation);

            this.transform.localPosition = trackable.position;
            this.transform.localRotation = trackable.orientation;
        }
	}
};