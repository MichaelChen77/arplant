using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using IMAV.Model;
using IMAV.Service;

namespace IMAV.Controller
{
    public class DataController : MonoBehaviour
    {
        public GameObject defaultModel;
        List<Category> categories;
        public List<Category> Categories
        {
            get { return categories; }
        }

        Dictionary<string, Product> products = new Dictionary<string, Product>();
        public Dictionary<string, Product> Products
        {
            get { return products; }
        }

        Stack<string> productkeys = new Stack<string>();

        private static DataController mSingleton;
        public static DataController Singleton
        {
            get
            {
                return mSingleton;
            }
        }

        void Awake()
        {
            if (mSingleton != null)
            {
                Destroy(this);
            }
            else
            {
                mSingleton = this;
            }
        }

        void Start()
        {
            CheckForUpdates();
        }

        public void CheckForUpdates()
        {
            DataCollector.Singleton.TopCategories(json =>
            {
                loadInitData(json);
            });
        }

        void loadInitData(string json)
        {
            categories = JsonConvert.DeserializeObject<List<Category>>(json);
            if (categories != null)
            {
                foreach (Category c in categories)
                {
                    c.LoadIcon();
                }
            }
            else
                categories = new List<Category>();
        }

        public void GetCatImage(Category cat)
        {
            DataCollector.Singleton.GetCategoryImage(cat.id, (sp)=>
            {
                cat.icon = sp;
            });
        }

        public Category GetCategory(long id)
        {
            for (int i = 0; i < categories.Count; i++)
            {
                if (categories[i].id == id)
                    return categories[i];
            }
            return null;
        }

        public void GetCatProduct(Category cat)
        {
            DataCollector.Singleton.GetProductsInCategory(cat.id, (json) =>
            {
                cat.Products = JsonConvert.DeserializeObject<List<CategoryProduct>>(json);
                cat.IsLoaded = true;
            });
        }

        public void GetProduct(string sku, Action<Product> callback)
        {
            if (products.ContainsKey(sku))
                callback(products[sku]);
            else
            {
                DataCollector.Singleton.GetProductDetail(sku, (p) =>
                {
                    products[p.sku] = p;
                    productkeys.Push(p.sku);
                    if (products.Count > 30)
                    {
                        string str = productkeys.Pop();
                        products[str].Delete();
                        products.Remove(str);
                    }
                    GetProductImage(p, callback);
                });
            }
        }

        public void GetProductImage(Product p, Action<Product> callback)
        {
            DataCollector.Singleton.GetProductImage(p.sku, (sp)=>
            {
                p.icon = sp;
                callback(p);
            });
        }

        public void LoadModelData(string sku)
        {
            GetProduct(sku, GetProductModel);
        }

        void GetProductModel(Product p)
        {
            if (p.model == null)
            {
                DataCollector.Singleton.DownloadAssetBundle(p.sku, (psku, objs) =>
                {
                    p.model = (GameObject)objs[0];
                    LoadModelToScene(p);
                });
            }
            else
                LoadModelToScene(p);
        }

        void LoadModelToScene(Product p)
        {
            SceneController.Singleton.AddProductToScene(p);
        }
    }
}
