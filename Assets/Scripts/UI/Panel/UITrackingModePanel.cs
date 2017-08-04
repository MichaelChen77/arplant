using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace IMAV.UI
{
    public class UITrackingModePanel : UIControl
    {
        public TextDisableSelf markerHint;
        public Toggle markerOnButton;
        public Toggle markerOffButton;
        public Toggle placementButton;

        bool updateUI = false;

        public override void Open()
        {
            base.Open();
            UpdateToggle(DataUtility.TrackingMode);
        }

        void UpdateToggle(ARTrackingMode m)
        {
            updateUI = true;
            switch (m)
            {
                case ARTrackingMode.Marker: markerOnButton.isOn = true; break;
                case ARTrackingMode.Markerless: markerOffButton.isOn = true; break;
                case ARTrackingMode.Placement: placementButton.isOn = true; break;
            }
            updateUI = false;
        }

        public void SetTrackingMode(int m)
        {
            ARTrackingMode tm = (ARTrackingMode)m;
            if (tm != DataUtility.TrackingMode)
            {
                if (m == 0 && markerOnButton.isOn)
                    SetTrackingMode(ARTrackingMode.Marker, true);
                else if (m == 1 && markerOffButton.isOn)
                    SetTrackingMode(ARTrackingMode.Markerless, true);
                else if (placementButton.isOn)
                    SetTrackingMode(ARTrackingMode.Placement, true);
            }
        }

        public void SetTrackingMode(ARTrackingMode m, bool showHint)
        {
            DataUtility.TrackingMode = m;
            if (!updateUI)
            {
                if (showHint)
                    SetMarkerHint(m);
                Close();
            }
        }

        void SetMarkerHint(ARTrackingMode m)
        {
            if (m == ARTrackingMode.Marker)
                markerHint.Open("AR Mode Marker On");
            else if (m == ARTrackingMode.Markerless)
                markerHint.Open("AR Mode Marker Off");
            else if (m== ARTrackingMode.Placement)
                markerHint.Open("Placement Mode On");
        }
    }
}
