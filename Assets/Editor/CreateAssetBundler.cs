using UnityEditor;
using UnityEngine;

public class CreateAssetBundler{

    [UnityEditor.MenuItem("AssetsBundle/Build AssetBundles - Win")]
    static void BuildStandaloneAssetBundles()
    {
        BuildPipeline.BuildAssetBundles("Assets/AssetBundles/Win", BuildAssetBundleOptions.DeterministicAssetBundle, BuildTarget.StandaloneWindows64);
    }

    [UnityEditor.MenuItem("AssetsBundle/Build AssetBundles -IOS")]
    static void BuildIOSBundles()
    {
        BuildPipeline.BuildAssetBundles("Assets/AssetBundles/IOS", BuildAssetBundleOptions.DeterministicAssetBundle, BuildTarget.iOS);
    }

    [UnityEditor.MenuItem("AssetsBundle/Build AssetBundles - Android")]
    static void BuildAndroidBundles()
    {
        BuildPipeline.BuildAssetBundles("Assets/AssetBundles/Android", BuildAssetBundleOptions.DeterministicAssetBundle, BuildTarget.Android);
    }

    [UnityEditor.MenuItem("AssetsBundle/Get AssetBundle names")]
    static void GetNames()
    {
        var names = AssetDatabase.GetAllAssetBundleNames();
        foreach (var name in names)
            Debug.Log("AssetBundle: " + name);
    }

    [UnityEditor.MenuItem("AssetsBundle/Clear Cache")]
    static void ClearCache()
    {
        Caching.ClearCache();
    }
} 
