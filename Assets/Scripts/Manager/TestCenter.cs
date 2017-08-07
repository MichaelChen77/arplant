using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IMAV
{
    public class TestCenter : MonoBehaviour
    {
        public DebugView debugview;

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
            if (debugview != null)
                debugview.Log(str);
        }

        public void ShowLog()
        {
            if (debugview != null)
                debugview.Open();
        }
    }
}
