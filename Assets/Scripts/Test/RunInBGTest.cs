using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RunInBGTest : MonoBehaviour {

    public DebugView debug;
    public InputField input;

	// Use this for initialization
	void Start () {
        Application.targetFrameRate = 30;
	}

    private void OnEnable()
    {
        debug.Log("Unity Enable");
    }

    private void OnDisable()
    {
        debug.Log("Unity disable");
    }

    private void OnApplicationFocus(bool focus)
    {
        debug.Log("Unity OnApplicationFocus");
    }

    private void OnApplicationPause(bool pause)
    {
        debug.Log("Unity OnApplicationPause");
    }

    private void OnApplicationQuit()
    {
        debug.Log("Unity OnApplicationQuit");
    }

    private void OnDestroy()
    {
        debug.Log("Unity OnDestroy");
    }

    private void OnPreRender()
    {
        debug.Log("Unity OnPreRender");
    }

    public void Share()
    {
        NativeShare.Share("Whiz Home product: Testing.....", string.Empty, string.Empty, FileType.Text);
    }

    public void GoBack()
    {
        using (AndroidJavaClass cls = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            using (AndroidJavaObject jo = cls.GetStatic<AndroidJavaObject>("currentActivity"))
            {
                jo.Call("quitActivity", input.text);
            }
        }
    }
}
