using UnityEngine;
using System.Collections;

public class WebManager : MonoBehaviour {

    public delegate void ReceiveStringData(string str);

    private static WebManager mSingleton;
    public static WebManager Singleton
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


    public IEnumerator SearchFurniture(string _type, string _name, ReceiveStringData getInfo)
    { 
        WWWForm form = new WWWForm();
        form.AddField("category", _type);
        form.AddField("name", _name);

        WWW w = new WWW(Tags.SearchUrl, form);
        yield return w;
        if (!string.IsNullOrEmpty(w.error))
            Debug.Log("Get Game Info error: " + w.error);
        else if (w.text.StartsWith("ok"))
        {
            getInfo(w.text.Substring(3));
        }
        else
            Debug.Log("GetGameInfo error:" + w.text);
    }

}
