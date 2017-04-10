using UnityEngine;
using IMAV;
using System.Collections;
using Newtonsoft.Json;

public class WebManager : MonoBehaviour {

    public delegate void ReceiveStringData(string str);
    public delegate void ReceiveData(MessageData data);
    public delegate void RecObjectsData(int id, Object[] obj);
    public delegate void ReceiveBinary(int id, byte[] data);

    private static WebManager mSingleton;
    public static WebManager Singleton
    {
        get
        {
            return mSingleton;
        }
    }

    public static UserData CurrentUser = new UserData();

    void Awake()
    {
        if (mSingleton)
        {
            Destroy(gameObject);
        }
        else
        {
            mSingleton = this;
            DontDestroyOnLoad(gameObject);
        }
    }


    public IEnumerator SearchFurniture(string _name, ReceiveStringData getInfo)
    {
        WWWForm form = new WWWForm();
        form.AddField("q", _name);

        WWW w = new WWW(Tags.SearchUrl, form);
        yield return w;
        Debug.Log("return: " + w.text);
        if (!string.IsNullOrEmpty(w.error))
            Debug.Log("Get Game Info error: " + w.error);
        else
            getInfo(w.text);
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
            Debug.Log("text: " + w.text);
            //returnData = (MessageData)JsonUtility.FromJson(w.text, typeof(MessageData));
            getInfo(w.text);
        }
        //else if (w.text.StartsWith("ok"))
        //{
        //    getInfo(w.text.Substring(3));
        //}
        //else
        //    Debug.Log("GetGameInfo error:" + w.text);
    }

    public IEnumerator UserLogin(string emailStr, string pw, ReceiveData getInfo)
    {
        WWWForm form = new WWWForm();
        form.AddField("email", emailStr);
        form.AddField("password", pw);
        Debug.Log(emailStr + " ; " + pw);
        WWW w = new WWW(Tags.UserLoginUrl, form);
        yield return w;
        MessageData returnData = JsonConvert.DeserializeObject<MessageData>(w.text);
        getInfo(returnData);
    }

    public IEnumerator DownloadAssetBundle(int id, string url, RecObjectsData getInfo)
    {
        WWW www = new WWW(url);
        yield return www;

        if(www.assetBundle != null)
        {
            Object[] objs = www.assetBundle.LoadAllAssets();
            getInfo(id, objs);
        }
        else
        {
            Debug.Log("cannot download asset bundle from " + url);
        }
    }

    public IEnumerator DownloadBinary(int id, string url, ReceiveBinary getInfo)
    {
        WWW www = new WWW(url);
        yield return www;

        if (www.error == null)
        {
            getInfo(id, www.bytes);
        }
        else
            Debug.Log("Cannot download image from: " + url);
    }

    public IEnumerator SaveScene(string key, string data, ReceiveStringData getInfo)
    {
        WWWForm form = new WWWForm();
        form.AddField("key", key);
        form.AddField("raw_design", data);
        Debug.Log(key + " ; " + data);
        WWW w = new WWW(Tags.UploadSceneUrl, form);
        yield return w;
        if (w.error == null)
            getInfo("ok");
        else
            getInfo("error" + w.error);
    }

    public IEnumerator LoadScene(string key, string id, ReceiveStringData getInfo)
    {
        WWWForm form = new WWWForm();
        form.AddField("key", key);
        form.AddField("id", id);
        Debug.Log(key + " ; " + id);

        WWW w = new WWW(Tags.UploadSceneUrl, form);
        yield return w;
        if (w.error == null)
            getInfo("ok");
        else
            getInfo("error" + w.error);
        yield return null;
    }

    public IEnumerator GetSceneList(string key, ReceiveStringData getInfo)
    {
        WWWForm form = new WWWForm();
        form.AddField("key", key);
        Debug.Log(key + " ; " );

        WWW w = new WWW(Tags.GetScenesUrl, form);
        yield return w;
        Debug.Log("text: " + w.text);
        if (w.error == null)
            getInfo(w.text);
        else
            getInfo("error" + w.error);
        yield return null;
    }

}
