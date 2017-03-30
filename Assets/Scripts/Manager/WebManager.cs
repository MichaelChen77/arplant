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

    public IEnumerator UserRegister(string firstName, string lastName, string email, string pw, ReceiveStringData getInfo)
    {
        WWWForm form = new WWWForm();
        form.AddField("first_name", firstName);
        form.AddField("last_name", lastName);
        form.AddField("email", email);
        form.AddField("password", pw);

        WWW w = new WWW(Tags.UserRegUrl, form);
        yield return w;
        if (!string.IsNullOrEmpty(w.error))
            Debug.Log("user register error: " + w.error);
        else
        {
            Debug.Log("return: " + w.text);
            getInfo(w.text);
        }
        //else if (w.text.StartsWith("ok"))
        //{
        //    getInfo(w.text.Substring(3));
        //}
        //else
        //    Debug.Log("GetGameInfo error:" + w.text);
    }

    public IEnumerator UserLogin(string emailStr, string pw, ReceiveStringData getInfo)
    {
        Debug.Log("login: " + emailStr + " ; " + pw);
        WWWForm form = new WWWForm();
        //form.AddField("first_name", firstName);
        //form.AddField("last_name", lastName);
        //form.AddField("email", email);
        //form.AddField("password", pw);

        WWW w = new WWW(Tags.UserRegUrl, form);
        yield return w;
        //if (!string.IsNullOrEmpty(w.error))
        //    Debug.Log("user register error: " + w.error);
        //else
        //{
        //    Debug.Log("return: " + w.text);
        //    getInfo(w.text);
        //}
    }

}
