using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using IMAV.Model;
using IMAV.Controller;

namespace IMAV.Service
{
    public class LocalDataService : ICollectorService
    {
        List<Category> catList;
        string json = "";

        public LocalDataService()
        {
            LoadResource();
        }

        void LoadResource()
        {
            TextAsset str = Resources.Load("Products/products") as TextAsset;
            json = str.text;
            catList = JsonConvert.DeserializeObject<List<Category>>(json);
        }

        public IEnumerator TopCategories(Action<string> callback)
        {
            yield return null;
            callback(json);
        }

        public IEnumerator GetProductsInCategory(long categoryId, Action<string> callback)
        {
            yield return null;
            foreach(Category cat in catList)
            {
                if(cat.id == categoryId)
                {
                    callback(JsonConvert.SerializeObject(cat.Products));
                    break;
                }
            }
        }

        public IEnumerator GetProductDetail(string sku, ProductDownloadCallback callback)
        {
            bool toEnd = false;
            yield return null;
            foreach (Category cat in catList)
            {
                foreach(CategoryProduct cp in cat.Products)
                {
                    if(cp.sku == sku)
                    {
                        Product p = new Product();
                        p.sku = sku;
                        p.name = cp.sku;
                        callback(p);
                        toEnd = true;
                        break;
                    }
                }
                if (toEnd)
                    break;
            }
        }

        public IEnumerator GetProductImage(string sku, ImageDownloadCallback callback)
        {
            bool toEnd = false;
            yield return null;
            foreach (Category cat in catList)
            {
                foreach (CategoryProduct cp in cat.Products)
                {
                    if (cp.sku == sku)
                    {
                        WWW www = new WWW(Application.streamingAssetsPath+ "/Products/" + cat.name + "/" + cp.sku+".png");
                        yield return www;
                        callback(www.bytes);
                        toEnd = true;
                        break;
                    }
                }
                if (toEnd)
                    break;
            }
        }

        public IEnumerator GetProductTexture(string sku, TextureDownloadCallback callback)
        {
            bool toEnd = false;
            yield return null;
            foreach (Category cat in catList)
            {
                foreach (CategoryProduct cp in cat.Products)
                {
                    if (cp.sku == sku)
                    {
                        Texture2D ta = Resources.Load("Products/" + cat.name + "/Icons/" + cp.sku) as Texture2D;
                        callback(ta);
                        toEnd = true;
                        break;
                    }
                }
                if (toEnd)
                    break;
            }
        }

        public IEnumerator GetCategoryImage(long categoryId, ImageDownloadCallback callback)
        {
            yield return null;
            foreach (Category cat in catList)
            {
                if (cat.id == categoryId)
                {
                    Debug.Log("path: " + Application.streamingAssetsPath + "/Products/" + cat.name + ".png");
                    WWW www = new WWW("file:///"+Application.streamingAssetsPath + "/Products/" + cat.name + ".png");

                    yield return www;
                    callback(www.bytes);
                    break;
                }
            }
        }

        public IEnumerator GetCategoryTexture(long categoryId, TextureDownloadCallback callback)
        {
            yield return null;
            foreach (Category cat in catList)
            {
                if (cat.id == categoryId)
                {
                    Texture2D ta = Resources.Load("Products/" + cat.name + "/" + cat.name) as Texture2D;
                    callback(ta);
                    break;
                }
            }
        }

        public IEnumerator DownloadAssetBundle(string sku, AssetBundleDownloadCallback callback)
        {
            bool toEnd = false;
            yield return null;
            foreach (Category cat in catList)
            {
                foreach (CategoryProduct cp in cat.Products)
                {
                    if (cp.sku == sku)
                    {
                        System.Object ta = Resources.Load("Products/" + cat.name + "/" + cp.sku);
                        callback(sku, new System.Object[] { ta });
                        toEnd = true;
                        break;
                    }
                }
                if (toEnd)
                    break;
            }
        }
    }
}
