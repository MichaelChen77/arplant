using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

public delegate void CategoryDownloadCallback(List<Category> categories);
public delegate void CategoryProductDownloadCallback(long categoryId, List<CategoryProduct> cpList);
public delegate void ProductDownloadCallback(Product p);
public delegate void MagentoImageDownloadCallback(byte[] bytes);
public delegate void MagentoTextureDownloadCallback(Texture2D texture);

public class MagentoService : MonoBehaviour {

    public const string baseUrl = "http://j-clef-web-01.japaneast.cloudapp.azure.com:9000";
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
            Debug.Log("Should not reach here");
        }
    }

    public IEnumerator TopCategories(CategoryDownloadCallback callback)
    {
        WWW cases = new WWW(baseUrl + "/magento/categories");
        yield return cases;
        
        string json = cases.text;

        List<Category> result = JsonConvert.DeserializeObject<List<Category>>(json);

        callback(result);
    }

    public IEnumerator GetProductsInCategory(long categoryId, CategoryProductDownloadCallback callback)
    {
        WWW cases = new WWW(baseUrl + "//magento/categories/" + categoryId + "/products");
        yield return cases;

        string json = cases.text;

        List<CategoryProduct> result = JsonConvert.DeserializeObject<List<CategoryProduct>>(json);

        Debug.Log("products in cat: " + result.Count);

        callback(categoryId, result);
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
	

    public string EncodeUriComponent(string component)
    {
        return WWW.EscapeURL(component).Replace("+", "%20");
    }
}
