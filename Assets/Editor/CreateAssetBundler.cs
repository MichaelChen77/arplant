using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using IMAV.Model;

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

    [UnityEditor.MenuItem("AssetsBundle/Create local data")]
    static void CreateLocalData()
    {
        string[] paths = Directory.GetDirectories(Application.dataPath + "/Resources/Products/");
        int pIndex = 1;
        List<Category> clist = new List<Category>();
        for (int i = 0; i< paths.Length; i++)
        {
            DirectoryInfo dir = new DirectoryInfo(paths[i]);
            Category c = new Category();
            c.id = i;
            c.name = dir.Name;
            c.Products = new List<CategoryProduct>();
            FileInfo[] files = dir.GetFiles();
            foreach(FileInfo f in files)
            {
                if (!string.Equals(f.Extension, ".meta"))
                {
                    CategoryProduct cp = new CategoryProduct();
                    cp.category_id = c.id;
                    cp.position = 1;
                    cp.sku = f.Name.Substring(0, f.Name.Length - f.Extension.Length);
                    c.Products.Add(cp);
                }
            }
            clist.Add(c);
        }
        string str = JsonConvert.SerializeObject(clist);
        Debug.Log("out: " + str);
        File.WriteAllText(Application.dataPath + "/Resources/Products/products.json", str);
    }

    [UnityEditor.MenuItem("AssetsBundle/Clear Cache")]
    static void ClearCache()
    {
        Caching.ClearCache();
    }
} 
