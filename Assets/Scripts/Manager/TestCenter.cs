using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IMAV
{
    public class TestCenter : MonoBehaviour
    {
        public DebugView debugview;
        public bool showOnPanel = true;

        private static TestCenter mSingleton;
        public static TestCenter Singleton
        {
            get
            {
                return mSingleton;
            }
        }

        void Awake()
        {
            if (mSingleton != null)
            {
                Destroy(gameObject);
            }
            else
            {
                mSingleton = this;
            }
        }

        public void Log(string str)
        {
            if (showOnPanel && debugview != null)
                debugview.Log(str);
            else
                Debug.Log(str);
        }

        public void Log(string str, bool panelShow)
        {
            if (panelShow && debugview != null)
                debugview.Log(str);
            else
                Debug.Log(str);
        }

        public void ShowLog()
        {
            if (debugview != null)
                debugview.Open();
        }
    }
}
