using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Collections;
using Newtonsoft.Json;
using IMAV.Model;
using IMAV.Controller;

namespace IMAV.Service
{
    public class MagentoService : ICollectorService
    {
//        public const string baseUrl = "http://j-clef-web-01.japaneast.cloudapp.azure.com:9000";
//		public const string baseUrl = "http://localhost/arkittest";
		public const string baseUrl = "http://172.21.149.162/arpreview/www/arkittest";

        public const string webPageUrl = "http://j-clef-web-01.japaneast.cloudapp.azure.com/index.php/";
        bool useCache;

		//test
		private int maxProductsCountInCache = 25;
		ArrayList productkeys = new ArrayList();
		List<Product> productList = new List<Product>();
		List<Product> newProductList = new List<Product>();

        public MagentoService(bool caching)
        {
            useCache = caching;
        }

        public IEnumerator TopCategories(Action<string> callback)
        {
			WWW cases = new WWW(baseUrl + "/magento/products.json");
            yield return cases;
            string json = cases.text;
			Debug.Log ("magento json file = " + json);
            if (useCache)
            {
                if (!string.IsNullOrEmpty(cases.error))//cases read failed
                {
                    if (File.Exists(DataUtility.GetCategoryFile()))
                    {
                        json = File.ReadAllText(DataUtility.GetCategoryFile());
                    }
                }
                else
                {
                    File.WriteAllText(DataUtility.GetCategoryFile(), json);
                }
            }

            callback(json);
        }

        public IEnumerator GetProductsInCategory(long categoryId, Action<string> callback)
        {
            string json = "";
            string path = DataUtility.GetCategoryPath() + categoryId + ".json";
            if (useCache && File.Exists(path))
            {
                json = File.ReadAllText(path);
            }
            else
            {
//                WWW cases = new WWW(baseUrl + "/magento/categories/" + categoryId + "/products");
				WWW cases = new WWW(baseUrl + "/magento/Json/mc/" + categoryId + ".json");	
                yield return cases;
                json = cases.text;
                if (useCache)
                    File.WriteAllText(path, json);
            }
			Debug.Log ("GetProductsInCategory");
			Debug.Log ("GetProductsInCategory json = " + json);
            callback(json);
        }


		public IEnumerator GetProductsInSubCategory(string subCategoryName, Action<string> callback)
		{
			string json = "";
			string path = DataUtility.GetSubCategoryPath() + subCategoryName + ".json";
			if (useCache && File.Exists(path))
			{
				json = File.ReadAllText(path);
			}
			else
			{
				//WWW cases = new WWW(baseUrl + "/magento/categories/" + categoryId + "/products");
				WWW cases = new WWW(baseUrl + "/magento/Json/sc/" + subCategoryName + ".json");	
				yield return cases;
				json = cases.text;
				if (useCache)
					File.WriteAllText(path, json);
			}
			Debug.Log ("GetProductsInSubCategory");
			Debug.Log ("GetProductsInSubCategory json = " + json);
			callback(json);
		}




        public IEnumerator GetProductDetail(string sku, ProductDownloadCallback callback)
        {
            WWW cases = new WWW(baseUrl + "/magento/Json/Products/" + EncodeUriComponent(sku) + ".json");
            yield return cases;

            string json = cases.text;

            Product result = JsonConvert.DeserializeObject<Product>(json);
            callback(result);
        }

		//test !!!!need to rewrite in the future
		public IEnumerator GetSubCategoryProductDetail(long subCategoryId, string sku, ProductDownloadCallback callback)
		{
			WWW cases = new WWW(baseUrl + "/magento/Json/Products/" + EncodeUriComponent(sku) + ".json");
			yield return cases;

			string json = cases.text;

			Product result = JsonConvert.DeserializeObject<Product>(json);
			callback(result);
		}

		//test !!!!need to rewrite in the future
		public IEnumerator GetSubCategoryDetail(long subCategoryId, string subCategoryName, SubCategoryDownloadCallback callback)
		{
			long subCategoryParentID = subCategoryId / 100;
			WWW cases = new WWW(baseUrl + "/magento/Json/" + subCategoryParentID + "/sc/" + EncodeUriComponent(subCategoryName) + ".json");
			yield return cases;

			string json = cases.text;

			SubCategory result = JsonConvert.DeserializeObject<SubCategory>(json);
			callback(result);
		}

		//test !!!!need to rewrite in the future
		public IEnumerator GetSubCategoryProductImage(long subCategoryId, string sku, ImageDownloadCallback callback)
		{
			byte[] bytes;
			string path = DataUtility.GetProductIconPath() + sku;
			if (useCache && File.Exists(path))
			{
				bytes = File.ReadAllBytes(path);
			}
			else
			{
				WWW cases = new WWW(baseUrl + "/magento/products_img/" + EncodeUriComponent(sku) + ".png");
				yield return cases;
				bytes = cases.bytes;
				File.WriteAllBytes(path, bytes);
			}

			callback(bytes);
		}


        public IEnumerator GetProductImage(string sku, ImageDownloadCallback callback)
        {
            byte[] bytes;
            string path = DataUtility.GetProductIconPath() + sku;
            if (useCache && File.Exists(path))
            {
                bytes = File.ReadAllBytes(path);
            }
            else
            {
				WWW cases = new WWW(baseUrl + "/magento/products_img/" + EncodeUriComponent(sku) + ".png");
                yield return cases;
                bytes = cases.bytes;
                File.WriteAllBytes(path, bytes);
            }
            callback(bytes);
        }

		//test !!!!need to rewrite in the future
		public IEnumerator GetSubCategoryTexture(long subCategoryId, string subCategoryName, TextureDownloadCallback callback)
		{
			string sku = "abc";
			WWW cases = new WWW(baseUrl + "/product-img/" + EncodeUriComponent(sku));
			yield return cases;

			callback(cases.texture);
		}

		//test !!!!need to rewrite in the future
		public IEnumerator GetSubCategoryProductTexture(long subCategoryId, string sku, TextureDownloadCallback callback)
		{
			WWW cases = new WWW(baseUrl + "/product-img/" + EncodeUriComponent(sku));
			yield return cases;

			callback(cases.texture);
		}

        public IEnumerator GetProductTexture(string sku, TextureDownloadCallback callback)
        {
            WWW cases = new WWW(baseUrl + "/product-img/" + EncodeUriComponent(sku));
            yield return cases;

            callback(cases.texture);
        }

		//test !!!!need to rewrite in the future
		public IEnumerator GetSubCategoryImage(long subCategoryId, string subCategoryName, ImageDownloadCallback callback){
			long categoryId = subCategoryId/100;
			byte[] bytes;
			string path = DataUtility.GetSubCategoryPath() + subCategoryName + ".png";
			if (useCache && File.Exists(path))
			{
				bytes = File.ReadAllBytes(path);
			}
			else
			{
				WWW cases = new WWW(baseUrl + "/magento/categories/" + categoryId 
					+ "/" + subCategoryName + "/" + subCategoryName + ".png");
				yield return cases;
				bytes = cases.bytes;
				File.WriteAllBytes(path, bytes);
			}
			callback(bytes);
		}


        public IEnumerator GetCategoryImage(long categoryId, ImageDownloadCallback callback)
        {
            byte[] bytes;
            string path = DataUtility.GetCategoryPath() + categoryId;
            if (useCache && File.Exists(path))
            {
                bytes = File.ReadAllBytes(path);
            }
            else
            {
				//icon is the file not the folder!
                WWW cases = new WWW(baseUrl + "/magento/categories/" + categoryId + "/icon.png");
                yield return cases;
                bytes = cases.bytes;
                File.WriteAllBytes(path, bytes);
            }
            callback(bytes);
        }

        public IEnumerator GetCategoryTexture(long categoryId, TextureDownloadCallback callback)
        {
            WWW cases = new WWW(baseUrl + "/magento/categories/" + categoryId + "/icon");
            yield return cases;

            callback(cases.texture);
        }

        public IEnumerator DownloadAssetBundle(string sku, AssetBundleDownloadCallback callback)
        {
			if (useCache) {
				updateProductQueue (sku);
			}

            string path = DataUtility.GetProductModelPath() + sku;
            string url = "";
            bool existed = File.Exists(path);
            if (useCache && existed)
                url = "file://" + path;
//				url = path;
            else
            {
//#if UNITY_ANDROID
//                url = Tags.AndroidEcModelUrl + sku;
//#elif UNITY_IOS
//            url = Tags.AndroidEcModelUrl + sku;
//#endif
				//test
				url = baseUrl + "/magento/model/" + EncodeUriComponent(sku);
				Debug.Log ("url = " + url);
            }

            WWW www = new WWW(url);
            yield return www;

            try
            {
                if (www.assetBundle != null)
                {
					Debug.Log("www.assetBundle != null");
                    System.Object[] objs = www.assetBundle.LoadAllAssets();
                    if (useCache)
                    {
                        File.WriteAllBytes(path, www.bytes);
                    }
                    callback(sku, objs);
                }
                else
                {
					Debug.Log("www.assetBundle == null");
                    System.Object[] objs = new System.Object[] { DataController.Singleton.defaultModel };
                    callback(sku, objs);
                }
            }
            catch (Exception ex)
            {
                Debug.Log("load asset error: " + ex.Message);
            }
        }

		//test delete model
		public void DeleteProductModelFile(string ProductName)
		{
			string path = DataUtility.GetProductModelPath() + ProductName;
//			Debug.Log ("model file path = " + path);
			FileAttributes attr = File.GetAttributes(path);
			if (File.Exists (path)) {
//				Debug.Log ("delete model name = " + ProductName);
				if (attr == FileAttributes.Directory) {
					Directory.Delete (path, true);
				} else {
					File.Delete (path);
				}
//				Debug.Log ("delete successful");
			} else {
				Debug.Log ("model file path not exit ");
			}
		}

		//test read product queue
		public void updateProductQueue(string productName){
			string path = DataUtility.GetProductPath() + "ProductQueue.json";
			int count = 0;
			if (productkeys.Count == 0) {
				if (File.Exists (path)) {
					//get the queue of products from json file
					Debug.Log("File existed");
					string json = File.ReadAllText(path);
					productList = JsonConvert.DeserializeObject<List<Product>>(json);
					foreach (Product p in productList) {
						if (count < maxProductsCountInCache) {
							productkeys.Add (p.name);
							count++;
						}
						else {
							DeleteProductModelFile (p.name);
						}
					}
				} else {
					Debug.Log("File not existed");
					//get the queue of products from Testsample
					count = 0;
					string productModelFolder = DataUtility.GetProductModelPath();
					DirectoryInfo dir = new DirectoryInfo(productModelFolder);
					Debug.Log("dir = " + dir.Name);
					productkeys.Clear();
					FileInfo[] files = dir.GetFiles();
					foreach (FileInfo f in files) {
						string modelName = f.Name.Substring (0, 
							f.Name.Length - f.Extension.Length);
						Debug.Log("modelName = " + modelName);
						if (count < maxProductsCountInCache) {
							//productkeys.Insert(0, modelName);
							Debug.Log("productkeys.Add " + modelName);
							productkeys.Add(modelName);
							count++;
						}
						else {
							DeleteProductModelFile (modelName);
						}
					}

				}
			}

			Debug.Log("productkeys." + productkeys.Count);
			if (productkeys.IndexOf (productName) != -1) {
				Debug.Log("b productkeys.IndexOf (productName) != -1");
				productkeys.Remove (productName);
			} else if (productkeys.Count == maxProductsCountInCache) {
				string pname = (string)productkeys[maxProductsCountInCache - 1];
				Debug.Log("productkeys.Count == maxProductsCountInCache pname = " + pname);
				DeleteProductModelFile (pname);
				productkeys.RemoveAt (maxProductsCountInCache - 1);
			}
			productkeys.Insert(0, productName);


			newProductList.Clear();
			int newCount = 0;
			Debug.Log("string pk in productkeys.");
			foreach (string pk in productkeys) {
				Product newProduct = new Product ();
				newProduct.name = pk;
				newProduct.queue = newCount;//put it at the begin
				newProductList.Add(newProduct);
				newCount++;
			}

			Debug.Log("WriteAllText(path, str)");
			string str = JsonConvert.SerializeObject(newProductList);
			Debug.Log("out: " + str);
			File.WriteAllText(path, str);
		}

        public string EncodeUriComponent(string component)
        {
            return WWW.EscapeURL(component).Replace("+", "%20");
        }
    }
}
