using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IMAV
{
    public class DisableSelf : MonoBehaviour
    {
        public float duration = 1f;
        public void Open()
        {
			gameObject.SetActive (true);
            Invoke("Close", duration);
        }

        public void Open(float t)
        {
            duration = t;
            Open();
        }

        void Close()
        {
            gameObject.SetActive(false);
        }
    }
}
