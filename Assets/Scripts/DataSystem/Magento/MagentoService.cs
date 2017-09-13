using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using IMAV;
using IMAV.Model;
using IMAV.Controller;

public delegate void CategoryDownloadCallback(List<Category> categories);
public delegate void CategoryProductDownloadCallback(long categoryId, List<CategoryProduct> cpList);
public delegate void ProductDownloadCallback(Product p);
public delegate void MagentoImageDownloadCallback(byte[] bytes);
public delegate void MagentoTextureDownloadCallback(Texture2D texture);
public delegate void MagentoAssetBundleDownloadCallback(string sku, System.Object[] objs);

public class MagentoService : MonoBehaviour {

    public const string baseUrl = "http://j-clef-web-01.japaneast.cloudapp.azure.com:9000";
    public const string webPageUrl = "http://j-clef-web-01.japaneast.cloudapp.azure.com/index.php/";
    private static MagentoService mInstance = null;

    public static MagentoService Instance
    {
        get
        {
            if (!mInstance)
            {
                mInstance = GameObject.FindObjectOfType<MagentoService>();
            }
            return mInstance;
        }
    }

    void Awake()
    {
        if (mInstance == null)
        {
            mInstance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public IEnumerator TopCategories(Action<string> callback)
    {
        WWW cases = new WWW(baseUrl + "/magento/categories");
        yield return cases;

        callback(cases.text);
    }

    public IEnumerator GetProductsInCategory(long categoryId, Action<string> callback)
    {
        WWW cases = new WWW(baseUrl + "//magento/categories/" + categoryId + "/products");
        yield return cases;

        callback(cases.text);
    }

    public IEnumerator GetProductDetail(string sku, ProductDownloadCallback callback)
    {
        WWW cases = new WWW(baseUrl + "/magento/products/" + EncodeUriComponent(sku));
        yield return cases;

        string json = cases.text;

        Product result = JsonConvert.DeserializeObject<Product>(json);
        callback(result);
    }

    public IEnumerator GetProductImage(string sku, MagentoImageDownloadCallback callback)
    {
        WWW cases = new WWW(baseUrl + "/product-img/" + EncodeUriComponent(sku));
        yield return cases;

        byte[] bytes = cases.bytes;

        callback(bytes);
    }

    public IEnumerator GetProductTexture(string sku, MagentoTextureDownloadCallback callback)
    {
        WWW cases = new WWW(baseUrl + "/product-img/" + EncodeUriComponent(sku));
        yield return cases;

        callback(cases.texture);
    }

    public IEnumerator GetCategoryImage(long categoryId, MagentoImageDownloadCallback callback)
    {
        WWW cases = new WWW(baseUrl + "/magento/categories/" + categoryId + "/icon");
        yield return cases;

        byte[] bytes = cases.bytes;

        callback(bytes);
    }

    public IEnumerator GetCategoryTexture(long categoryId, MagentoTextureDownloadCallback callback)
    {
        WWW cases = new WWW(baseUrl + "/magento/categories/" + categoryId + "/icon");
        yield return cases;

        callback(cases.texture);
    }

    public IEnumerator DownloadAssetBundle(string sku, MagentoAssetBundleDownloadCallback callback)
    {
        string path = DataUtility.GetProductModelPath() + sku;
        string url = "";
        bool existed = File.Exists(path);
        if (existed)
            url = "file://" + path;
        else
        {
#if UNITY_ANDROID
            url = Tags.AndroidEcModelUrl + sku;
#elif UNITY_IOS
            url = Tags.AndroidEcModelUrl + sku;
#endif
        }
        Debug.Log("url: "+url);
        WWW www = new WWW(url);
        yield return www;

        try
        {
            if (www.assetBundle != null)
            {
                System.Object[] objs = www.assetBundle.LoadAllAssets();
                if (!existed)
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
        catch(Exception ex)
        {
            Debug.Log("load asset error: " + ex.Message);
        }
    }


    public string EncodeUriComponent(string component)
    {
        return WWW.EscapeURL(component).Replace("+", "%20");
    }
}
