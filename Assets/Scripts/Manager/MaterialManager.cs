using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialManager : MonoBehaviour {

    public Material[] materails;

    public int MaterialCount
    {
        get { return materails.Length; }
    }

    private static MaterialManager mSingleton;
    public static MaterialManager Singleton
    {
        get
        {
            return mSingleton;
        }
    }

    void Awake()
    {
        if (mSingleton)
        {
            Destroy(gameObject);
        }
        else
        {
            mSingleton = this;
        }
    }
}
