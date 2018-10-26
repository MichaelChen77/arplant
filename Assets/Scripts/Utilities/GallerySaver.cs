using System.Collections;
using System.IO;

using UnityEngine;

public static class GallerySaver
{
#if !UNITY_EDITOR && UNITY_ANDROID
    private static AndroidJavaClass m_ajc = null;
    private static AndroidJavaClass AJC
    {
        get
        {
            if( m_ajc == null )
                m_ajc = new AndroidJavaClass( "com.simsoft.sshelper.SSHelper" );
 
            return m_ajc;
        }
    }
#endif

    public static void SavePictureToGallery(byte[] bytes, string filename)
    {
        string path = Path.Combine(GetPicturesFolderPath(), filename);

        File.WriteAllBytes(path, bytes);

#if !UNITY_EDITOR && UNITY_ANDROID
        AndroidJavaObject context;
        using( AndroidJavaClass unityClass = new AndroidJavaClass( "com.unity3d.player.UnityPlayer" ) )
        {
            context = unityClass.GetStatic<AndroidJavaObject>( "currentActivity" );
        }
 
        AJC.CallStatic( "MediaScanFile", context, path );
#endif
    }

    public static string GetPicturesFolderPath()
    {
#if UNITY_EDITOR
        return System.Environment.GetFolderPath(System.Environment.SpecialFolder.DesktopDirectory);
#elif UNITY_ANDROID
        return AJC.CallStatic<string>( "GetPicturesFolderPath", Application.productName );
#else
        return Application.persistentDataPath;
#endif
    }

    public static void CopyToGallery(string source, string filename)
    {
        string path = Path.Combine(GetPicturesFolderPath(), filename);
        File.Copy(source, path);
    }
}