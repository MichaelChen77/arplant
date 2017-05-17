using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

#if UNITY_STANDALONE_WIN || UNITY_ANDROID || UNITY_IOS ||UNITY_STANDALONE_OSX
using System.IO;
#endif

public class MLFileSystem
{
    public static bool dummy = false;
	#if UNITY_STANDALONE_WIN || UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE_OSX
    IMLFileSystem filesystem = new UnityWinFileSystem();
    #elif UNITY_WEBPLAYER
    IMLFileSystem filesystem = new UnityWebFileSystem();
    #endif
        
    private static MLFileSystem mSingleton;
    private static MLFileSystem Singleton{
        get{
            if(mSingleton == null){
                mSingleton = new MLFileSystem();
            }
            return mSingleton;
        }
    }
    public static void Initialise()
    {
        IMLFileSystem test = MLFileSystem.IO;
    }

    public static VirtualFile.FileType GetFileType(string path)
    {
        if (path == null)
        {
            return VirtualFile.FileType.String;
        }
        string extension = System.IO.Path.GetExtension(path);
        if (extension.Equals(".xml") || extension.Equals(".txt"))
        {
            return VirtualFile.FileType.String;
        } else {
            return VirtualFile.FileType.Binary;
        }
    }
    public static void RegisterNewFileSystem(IMLFileSystem newSystem)
    {
        Singleton.filesystem = newSystem;
    }

    public static IMLFileSystem IO
    {
        get
        {
            return Singleton.filesystem;
        }
    }

    public static event RefreshHandler RefreshEvent = delegate { };
    public static void RefreshFileSystem()
    {
        RefreshEvent();
    }
    public delegate void RefreshHandler();

    public static void RegisterRefresh(RefreshHandler refreshMethod)
    {
        MLFileSystem.Initialise();
        RefreshEvent += refreshMethod;
    }
    public static void UnRegisterRefresh(RefreshHandler refreshMethod)
    {
        RefreshEvent -= refreshMethod;
    }
}