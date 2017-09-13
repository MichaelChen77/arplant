using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using IMAV.Model;

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
                Destroy(gameObject);
            }
            else
            {
                mSingleton = this;
            }
        }

        void Start()
        {
            DataUtility.SetDirectory(DataUtility.GetCategoryPath());
            DataUtility.SetDirectory(DataUtility.GetProductPath());
            DataUtility.SetDirectory(DataUtility.GetProductIconPath());
            DataUtility.SetDirectory(DataUtility.GetProductModelPath());
            CheckForUpdates();
        }

        public void CheckForUpdates()
        {
            StartCoroutine(MagentoService.Instance.TopCategories(json =>
            {
                File.WriteAllText(DataUtility.GetCategoryFile(), json);
                loadInitData(json);
            }));
        }

        void loadInitData()
        {
            if (File.Exists(DataUtility.GetCategoryFile()))
            {
                string json = File.ReadAllText(DataUtility.GetCategoryFile());
                loadInitData(json);
            }
            else
            {
                CheckForUpdates();
            }
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
            string path = DataUtility.GetCategoryPath() + cat.id;
            if (File.Exists(path))
            {
                byte[] bytes = File.ReadAllBytes(path);
                cat.icon = DataUtility.CreateSprite(bytes);
            }
            else
            {
                StartCoroutine(MagentoService.Instance.GetCategoryImage(cat.id, (bytes) =>
                {
                    File.WriteAllBytes(path, bytes);
                    cat.icon = DataUtility.CreateSprite(bytes);
                }));
            }
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
            string path = DataUtility.GetCategoryPath() + cat.id + ".json";
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                cat.Products = JsonConvert.DeserializeObject<List<CategoryProduct>>(json);
                cat.IsLoaded = true;
            }
            else
            {
                StartCoroutine(MagentoService.Instance.GetProductsInCategory(cat.id, (json) =>
                {
                    File.WriteAllText(path, json);
                    cat.Products = JsonConvert.DeserializeObject<List<CategoryProduct>>(json);
                    cat.IsLoaded = true;
                }));
            }
        }

        public void GetProduct(string sku, Action<Product> callback)
        {
            if (products.ContainsKey(sku))
                callback(products[sku]);
            else
            {
                StartCoroutine(MagentoService.Instance.GetProductDetail(sku, (p) =>
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
                }));
            }
        }

        public void GetProductImage(Product p, Action<Product> callback)
        {
            string path = DataUtility.GetProductIconPath() + p.sku;
            if (File.Exists(path))
            {
                byte[] bytes = File.ReadAllBytes(path);
                p.icon = DataUtility.CreateSprite(bytes);
                callback(p);
            }
            else
            {
                StartCoroutine(MagentoService.Instance.GetProductImage(p.sku, (bytes) =>
                {
                    File.WriteAllBytes(path, bytes);
                    p.icon = DataUtility.CreateSprite(bytes);
                    callback(p);
                }));
            }
        }

        public void LoadModelData(string sku)
        {
            GetProduct(sku, GetProductModel);
        }

        void GetProductModel(Product p)
        {
            if (p.model == null)
            {
                StartCoroutine(MagentoService.Instance.DownloadAssetBundle(p.sku, (psku, objs) =>
                {
                    p.model = (GameObject)objs[0];
                    LoadModelToScene(p);
                }));
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
