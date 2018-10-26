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
		Debug.Log("paths.Length = " + paths.Length);
        for (int i = 0; i< paths.Length; i++)
        {
            DirectoryInfo dir = new DirectoryInfo(paths[i]);
			Debug.Log("dir = " + dir);
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
					Debug.Log("cp.sku = " + cp.sku);
                    c.Products.Add(cp);
                }
            }
            clist.Add(c);

        }
        string str = JsonConvert.SerializeObject(clist);
        Debug.Log("out: " + str);
        File.WriteAllText(Application.dataPath + "/Resources/Products/products.json", str);
    }

	[UnityEditor.MenuItem("AssetsBundle/New Create local data")]
	static void NewCreateLocalData()
	{
		string[] paths = Directory.GetDirectories(Application.dataPath + "/Resources/Products/");
		int pIndex = 1;
		List<Category> clist = new List<Category>();
		Debug.Log("paths.Length = " + paths.Length);
		for (int i = 0; i< paths.Length; i++)
		{
			DirectoryInfo dir = new DirectoryInfo(paths[i]);
			Debug.Log("dir = " + dir);
			Category mc = new Category();
			mc.id = i;
			mc.name = dir.Name;
			Debug.Log("mc.name = " + mc.name);
			mc.SubCategorys = new List<SubCategory>();
			mc.Products = new List<CategoryProduct>();

			//the sub path (second layer)
			List<SubCategory> subclist = new List<SubCategory>();
			string[] subPaths = Directory.GetDirectories(paths[i]);
			Debug.Log("subPaths.Length = " + subPaths.Length);
			for (int j = 0; j< subPaths.Length; j++){
				DirectoryInfo subDir = new DirectoryInfo(subPaths[j]);
				Debug.Log("subDir = " + subDir);

				SubCategory sc = new SubCategory();
				sc.sub_id = mc.id * 100 + j;//the subCatagory id is made up from parent_id and its own current id
				sc.sub_name = subDir.Name;
				sc.parent_id = mc.id;
				sc.parent_name = mc.name;

				FileInfo[] subFiles = subDir.GetFiles ();
				foreach (FileInfo f in subFiles) {
					if (string.Equals (f.Extension, ".DS_Store")) {
						continue;
					}

					if (string.Equals (f.Extension, ".prefab")) {
						CategoryProduct cp = new CategoryProduct ();
						cp.category_id = sc.sub_id;
						cp.sku = f.Name.Substring (0, f.Name.Length - f.Extension.Length);
						Debug.Log ("cp.sku = " + cp.sku);
						sc.Products.Add (cp);
					}
				}
//				subclist.Add(sc);
				mc.SubCategorys.Add(sc);
			}
//			mc.SubCategorys = subclist;



			//the products inside the current path, not including the folder 
			FileInfo[] files = dir.GetFiles();
			foreach(FileInfo f in files)
			{
				Debug.Log ("ffffffffff = " + f.ToString());

				if (string.Equals (f.Extension, ".DS_Store")) {
					continue;
				}

				if (string.Equals (f.Extension, ".prefab")) {
					CategoryProduct cp = new CategoryProduct ();
					cp.category_id = mc.id;
					cp.sku = f.Name.Substring (0, f.Name.Length - f.Extension.Length);
					Debug.Log ("cp.sku = " + cp.sku);
					mc.Products.Add (cp);
				}
					
//				if (!string.Equals(f.Extension, ".meta"))
//				{
//					CategoryProduct cp = new CategoryProduct();
//					cp.category_id = c.id;
//					cp.position = 1;
//					cp.sku = f.Name.Substring(0, f.Name.Length - f.Extension.Length);
//					Debug.Log("cp.sku = " + cp.sku);
//					c.Products.Add(cp);
//				}
			}

			string mcstr = JsonConvert.SerializeObject(mc);
			string filePath = Application.dataPath + "/Resources/Json/mc/";
			if (!File.Exists (filePath)) {
				Directory.CreateDirectory (filePath);
			}
			Debug.Log("mcout: " + mcstr);
			File.WriteAllText(Application.dataPath + "/Resources/Json/mc/" + mc.id + ".json", mcstr);

			clist.Add(mc);

		}
		string str = JsonConvert.SerializeObject(clist);
		Debug.Log("out: " + str);
		File.WriteAllText(Application.dataPath + "/Resources/Products/products.json", str);
	}


	[UnityEditor.MenuItem("AssetsBundle/Create Json File")]
	static void CreateJsonFile()
	{
		int productQueue = 0;
		string[] paths = Directory.GetDirectories(Application.dataPath + "/Resources/Products/");
		int pIndex = 1;
		List<Category> clist = new List<Category>();
		for (int i = 0; i< paths.Length; i++)
		{
			DirectoryInfo dir = new DirectoryInfo(paths[i]);
			Debug.Log("dir = " + dir);
			Category mc = new Category();
			mc.id = i;
			mc.name = dir.Name;
			Debug.Log("mc.name = " + mc.name);
			mc.SubCategorys = new List<SubCategory>();
			mc.Products = new List<CategoryProduct>();

			//the sub path (second layer)
			string[] subPaths = Directory.GetDirectories(paths[i]);
			Debug.Log("subPaths.Length = " + subPaths.Length);
			for (int j = 0; j< subPaths.Length; j++){
				DirectoryInfo subDir = new DirectoryInfo(subPaths[j]);
				Debug.Log("subDir = " + subDir);

				SubCategory sc = new SubCategory();
				sc.sub_id = mc.id * 100 + j;//the subCatagory id is made up from parent_id and its own current id
				sc.sub_name = subDir.Name;
				sc.parent_id = mc.id;
				sc.parent_name = mc.name;

				FileInfo[] subFiles = subDir.GetFiles ();
				foreach (FileInfo f in subFiles) {
					if (string.Equals (f.Extension, ".DS_Store")) {
						continue;
					}

					if (string.Equals (f.Extension, ".prefab")) {
						CategoryProduct cp = new CategoryProduct ();
						cp.category_id = sc.sub_id;
						cp.sku = f.Name.Substring (0, f.Name.Length - f.Extension.Length);
						Debug.Log ("cp.sku = " + cp.sku);
						sc.Products.Add (cp);

						//product in sub category
						Product p = new Product ();
						p.sku = f.Name.Substring (0, f.Name.Length - f.Extension.Length);
						p.queue = productQueue++;
						p.category_id = sc.sub_id;
						p.category_name = sc.sub_name;
						string pstr = JsonConvert.SerializeObject(p);
						Debug.Log("out: " + pstr);

//						string filePath = Application.dataPath + "/Resources/Json/" + mc.id + "/sc/" + sc.sub_name;
						string filePath = Application.dataPath + "/Resources/Json/Products/";
						if (!File.Exists (filePath)) {
							Directory.CreateDirectory (filePath);
						}
//						File.WriteAllText(Application.dataPath + "/Resources/Json/" + mc.id + "/sc/" + sc.sub_name + "/" + p.sku + ".json", pstr);
						File.WriteAllText(Application.dataPath + "/Resources/Json/Products/" + p.sku + ".json", pstr);
					}
				}
				mc.SubCategorys.Add(sc);
				string scstr = JsonConvert.SerializeObject(sc);
				Debug.Log("out: " + scstr);
//				string filePath1 = Application.dataPath + "/Resources/Json/" + mc.id + "/sc";
				string filePath1 = Application.dataPath + "/Resources/Json/sc";
				if (!File.Exists (filePath1)) {
					Directory.CreateDirectory (filePath1);
				}
//				File.WriteAllText(Application.dataPath + "/Resources/Json/" + mc.id + "/sc/" + sc.sub_name + ".json", scstr);
				File.WriteAllText(Application.dataPath + "/Resources/Json/sc/" + sc.sub_name + ".json", scstr);
			}

			//the products inside the current path, not including the folder 
			FileInfo[] files = dir.GetFiles();
			foreach(FileInfo f in files)
			{
				if (string.Equals (f.Extension, ".DS_Store")) {
					continue;
				}

				if (string.Equals (f.Extension, ".prefab")) {

					//product in main category
					CategoryProduct cp = new CategoryProduct ();
					cp.category_id = mc.id;
					cp.sku = f.Name.Substring (0, f.Name.Length - f.Extension.Length);
					Debug.Log ("cp.sku = " + cp.sku);
					mc.Products.Add (cp);

					//product in main category
					Product p = new Product ();
					p.sku = f.Name.Substring (0, f.Name.Length - f.Extension.Length);
					p.queue = productQueue++;
					p.category_id = mc.id;
					p.category_name = mc.name;
					string pstr = JsonConvert.SerializeObject(p);
					Debug.Log("out: " + pstr);
//					string filePath = Application.dataPath + "/Resources/Json/" + mc.id + "/mp";
					string filePath = Application.dataPath + "/Resources/Json/Products/";
					if (!File.Exists (filePath)) {
						Directory.CreateDirectory (filePath);
					}
//					File.WriteAllText(Application.dataPath + "/Resources/Json/" + mc.id + "/mp/" + p.sku + ".json", pstr);
					File.WriteAllText(Application.dataPath + "/Resources/Json/Products/" + p.sku + ".json", pstr);
				}

			}

			//mc
			string mcstr = JsonConvert.SerializeObject(mc);
			string mcfilePath = Application.dataPath + "/Resources/Json/mc/";
			if (!File.Exists (mcfilePath)) {
				Directory.CreateDirectory (mcfilePath);
			}
			Debug.Log("mcout: " + mcstr);
			File.WriteAllText(Application.dataPath + "/Resources/Json/mc/" + mc.id + ".json", mcstr);
			clist.Add(mc);

		}
		string str = JsonConvert.SerializeObject(clist);
		Debug.Log("out: " + str);
		File.WriteAllText(Application.dataPath + "/Resources/Products/webproducts.json", str);
	}


    [UnityEditor.MenuItem("AssetsBundle/Clear Cache")]
    static void ClearCache()
    {
        Caching.ClearCache();
    }
} 
