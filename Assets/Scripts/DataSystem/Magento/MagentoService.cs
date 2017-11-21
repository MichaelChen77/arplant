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
        public const string baseUrl = "http://j-clef-web-01.japaneast.cloudapp.azure.com:9000";
        public const string webPageUrl = "http://j-clef-web-01.japaneast.cloudapp.azure.com/index.php/";
        bool useCache;

        public MagentoService(bool caching)
        {
            useCache = caching;
        }

        public IEnumerator TopCategories(Action<string> callback)
        {
            WWW cases = new WWW(baseUrl + "/magento/categories");
            yield return cases;
            string json = cases.text;
            if (useCache)
            {
                if (!string.IsNullOrEmpty(cases.error))
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
                WWW cases = new WWW(baseUrl + "//magento/categories/" + categoryId + "/products");
                yield return cases;
                json = cases.text;
                if (useCache)
                    File.WriteAllText(path, json);
            }

            callback(json);
        }

        public IEnumerator GetProductDetail(string sku, ProductDownloadCallback callback)
        {
            WWW cases = new WWW(baseUrl + "/magento/products/" + EncodeUriComponent(sku));
            yield return cases;

            string json = cases.text;

            Product result = JsonConvert.DeserializeObject<Product>(json);
            callback(result);
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
                WWW cases = new WWW(baseUrl + "/product-img/" + EncodeUriComponent(sku));
                yield return cases;
                bytes = cases.bytes;
                File.WriteAllBytes(path, bytes);
            }
            
            callback(bytes);
        }

        public IEnumerator GetProductTexture(string sku, TextureDownloadCallback callback)
        {
            WWW cases = new WWW(baseUrl + "/product-img/" + EncodeUriComponent(sku));
            yield return cases;

            callback(cases.texture);
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
                WWW cases = new WWW(baseUrl + "/magento/categories/" + categoryId + "/icon");
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
            string path = DataUtility.GetProductModelPath() + sku;
            string url = "";
            bool existed = File.Exists(path);
            if (useCache && existed)
                url = "file://" + path;
            else
            {
#if UNITY_ANDROID
                url = Tags.AndroidEcModelUrl + sku;
#elif UNITY_IOS
            url = Tags.AndroidEcModelUrl + sku;
#endif
            }
            WWW www = new WWW(url);
            yield return www;

            try
            {
                if (www.assetBundle != null)
                {
                    System.Object[] objs = www.assetBundle.LoadAllAssets();
                    if (useCache)
                    {
                        File.WriteAllBytes(path, www.bytes);
                    }
                    callback(sku, objs);
                }
                else
                {
                    System.Object[] objs = new System.Object[] { DataController.Singleton.defaultModel };
                    callback(sku, objs);
                }
            }
            catch (Exception ex)
            {
                Debug.Log("load asset error: " + ex.Message);
            }
        }

        public string EncodeUriComponent(string component)
        {
            return WWW.EscapeURL(component).Replace("+", "%20");
        }
    }
}
