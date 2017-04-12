using UnityEngine;
using System.Collections;

namespace IMAV
{
    public class DontDestroy : MonoBehaviour
    {
        void Awake()
        {
            if (DataUtility.dontdestroy == null)
            {
                DontDestroyOnLoad(gameObject);
                DataUtility.dontdestroy = this;
            }
            else if (DataUtility.dontdestroy != this)
            {
                Destroy(gameObject);
            }
        }
    }
}
