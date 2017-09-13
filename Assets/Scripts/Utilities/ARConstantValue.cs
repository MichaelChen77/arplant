using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IMAV
{
    public class ARConstantValue
    {
        public const string AR_UNAVAILABLE = "TRACKING UNAVAILABLE";
        public const string AR_NORMAL = "TRACKING NORMAL";
        public const string AR_EXCESSIVEMOTION = "TRACKING LIMITED: Too much camera movement\nTry slowing down your movement, or reset the session.";
        public const string AR_INSUFFICIENTFEATURES = "TRACKING LIMITED: Not enough surface detail\nTry pointing at a flat surface, or reset the session.";
        public const string AR_INITIALIZING = "Initializing AR Session";
        public const string AR_PLANEDETECTED = "SURFACE DETECTED";
        public const string AR_FINDPLANE = "FIND A SURFACE TO PLACE AN OBJECT";
    }
}
