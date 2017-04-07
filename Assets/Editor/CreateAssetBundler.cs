using UnityEditor;
using UnityEngine;

public class CreateAssetBundler{

    [UnityEditor.MenuItem("Assets/Build AssetBundles")]
    static void BuildAllAssetBundles()
    {
        BuildPipeline.BuildAssetBundles("Assets/AssetBundles", BuildAssetBundleOptions.DeterministicAssetBundle, BuildTarget.StandaloneWindows64);
    }

    [UnityEditor.MenuItem("Assets/Get AssetBundle names")]
    static void GetNames()
    {
        var names = AssetDatabase.GetAllAssetBundleNames();
        foreach (var name in names)
            Debug.Log("AssetBundle: " + name);
    }

    [UnityEditor.MenuItem("Assets/Clear Cache")]
    static void ClearCache()
    {
        Caching.CleanCache();
    }
}
