using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kudan.AR;

namespace IMAV
{
    public class ResetButton : MonoBehaviour
    {
        public KudanTracker _kudan;
        public GameObject triggerImage;

        void Update()
        {
            triggerImage.SetActive(!_kudan.ArbiTrackIsTracking());
        }
    }
}
