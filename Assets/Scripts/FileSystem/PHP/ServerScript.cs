using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Xml;

public class ServerScript : MonoBehaviour
{
    public string baseUrl = "http://155.69.150.59/IMAV/file_util.php";
    public string postUrl = "http://155.69.147.43/maze/upload_file.php";

    private Queue<PhpRequest> requests = new Queue<PhpRequest>();

    //private const float SyncDelay = 5.0f;
    //private float elapsed = 0.0f;
    static ServerScript mSingleton = null;
    public static ServerScript Singleton
    {
        get
        {
            if (mSingleton == null)
            {
                mSingleton = new GameObject("Server Script").AddComponent<ServerScript>();
            }
            return mSingleton;
        }
    }

    void Awake()
    {
        if (mSingleton != null)
        {
            Destroy(this);
            UnityEngine.Debug.LogError("Cannot create ServerScript twice");
        }
        else
        {
            DontDestroyOnLoad(this);
        }
    }
    void Start()
    {
        StartCoroutine(RequestDispatcher());
    }

    //void Update()
    //{
    //    elapsed += Time.deltaTime;
    //    if (elapsed >= SyncDelay)
    //    {
    //        ((UnityWebFileSystem)(MLFileSystem.IO)).Sync();
    //        elapsed = 0.0f;
    //    }
    //}

    public void setURL(string base_url, string post_url)
    {
        baseUrl = base_url;
        postUrl = post_url;
    }

    public void AppendRequest(PhpRequest request)
    {
        requests.Enqueue(request);
    }

    public void StartPHPRequests()
    {
        StartCoroutine(RequestDispatcher());
    }

    /// <summary>
    /// A TaskDispatcher to execute all the Php Request one at a time. It is always running.
    /// </summary>
    /// <returns></returns>
    IEnumerator RequestDispatcher()
    {
        while (true)
        {
            if (requests.Count > 0)
            {
                PhpRequest request = requests.Dequeue();
                yield return StartCoroutine(DoRequest(request));
            }
            yield return null;
        }
    }
    public static bool ServerError = false;

    IEnumerator DoRequest(PhpRequest request)
    {
        WWW www = null;
        string myURL = null;

        if (request is PhpDownloadRequest)
        {
            myURL = request.ToString();
            www = new WWW(myURL);
            yield return www;
            if (www.error == null)
            {
                string path = request.GetAttribute(PhpAttributeType.path);
                //if (MLFileSystem.GetFileType(path) == VirtualFile.FileType.Binary)
                if(request.ReturnFileType == VirtualFile.FileType.Binary)
                {
                    ((PhpDownloadRequest)request).InvokeOnDownloadedBinary(path, www.bytes);
                }
                else
                {
                    ((PhpDownloadRequest)request).InvokeOnDownloaded(path, www.text);
                }
            }
            else
            {
                Debug.Log("www error: "+www.error);
                ServerError = true;
            }
        }
        else if (request is PhpUploadRequest)
        {
            WWWForm form = ((PhpUploadRequest)request).Form;
            www = new WWW(postUrl, form);
            yield return www;
            ((PhpUploadRequest)request).InvokeOnUploaded(request.GetAttribute(PhpAttributeType.path));
        }
        else if (request is PhpDeleteRequest)
        {
            myURL = request.ToString();
            www = new WWW(myURL);
            yield return www;
            ((PhpDeleteRequest)request).InvokeOnDeleted(request.GetAttribute(PhpAttributeType.path));
        }
        else
        {
            throw new Exception("Requests for other types of php requests have not been implemented.");
        }
    }
}
