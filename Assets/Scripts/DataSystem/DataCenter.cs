using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

namespace IMAV
{
    public class DataCenter : MonoBehaviour
    {
        public GameObject defaultModel;
        List<CategoryData> categories = new List<CategoryData>();

        public List<CategoryData> Categories
        {
            get { return categories; }
        }

        Dictionary<string, ProductData> products = new Dictionary<string, ProductData>();
        public Dictionary<string, ProductData> Products
        {
            get { return products; }
        }

        private static DataCenter mSingleton;
        public static DataCenter Singleton
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
            if(File.Exists(DataUtility.GetCategoryFile()))
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
            List<Category> cats = JsonConvert.DeserializeObject<List<Category>>(json);
            if (cats != null)
            {
                foreach (Category c in cats)
                {
                    CategoryData d = new CategoryData(c);
                    d.LoadIcon();
                    categories.Add(d);
                }
            }
        }

        public void GetCatImage(CategoryData cat)
        {
            string path = DataUtility.GetCategoryPath() + cat.Cat.id;
            if (File.Exists(path))
            {
                byte[] bytes = File.ReadAllBytes(path);
                cat.icon = DataUtility.CreateSprite(bytes);
            }
            else
            {
                StartCoroutine(MagentoService.Instance.GetCategoryImage(cat.Cat.id, (bytes) =>
                {
                    File.WriteAllBytes(path, bytes);
                    cat.icon = DataUtility.CreateSprite(bytes);
                }));
            }
        }

        public CategoryData GetCategory(long id)
        {
            for(int i=0; i<categories.Count; i++)
            {
                if (categories[i].Cat.id == id)
                    return categories[i];
            }
            return null;
        }

        public void GetCatProduct(CategoryData cat)
        {
            string path = DataUtility.GetCategoryPath() + cat.Cat.id + ".json";
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                cat.Products = JsonConvert.DeserializeObject<List<CategoryProduct>>(json);
                cat.SetLoaded(true);
            }
            else
            {
                StartCoroutine(MagentoService.Instance.GetProductsInCategory(cat.Cat.id, (json) =>
                {
                    File.WriteAllText(path, json);
                    cat.Products = JsonConvert.DeserializeObject<List<CategoryProduct>>(json);
                    cat.SetLoaded(true);
                }));
            }
        }

        public void GetProduct(string sku, Action<ProductData> callback)
        {
            if (products.ContainsKey(sku))
                callback(products[sku]);
            else
            {
                StartCoroutine(MagentoService.Instance.GetProductDetail(sku, (p) =>
                {
                    ProductData pd = new ProductData(p);
                    products[pd.ProductInfo.sku] = pd;
                    GetProductImage(pd, callback);
                }));
            }
        }

        public void GetProductImage(ProductData p, Action<ProductData> callback)
        {
            string path = DataUtility.GetProductIconPath() + p.ProductInfo.sku;
            if (File.Exists(path))
            {
                byte[] bytes = File.ReadAllBytes(path);
                p.icon = DataUtility.CreateSprite(bytes);
                callback(p);
            }
            else
            {
                StartCoroutine(MagentoService.Instance.GetProductImage(p.ProductInfo.sku, (bytes) =>
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

        void GetProductModel(ProductData p)
        {
            if (p.model == null)
            {
                StartCoroutine(MagentoService.Instance.DownloadAssetBundle(p.ProductInfo.sku, (psku, objs) =>
                {
                    p.model = (GameObject)objs[0];
                    LoadModelToScene(p);
                }));
            }
            else
                LoadModelToScene(p);
        }

        void LoadModelToScene(ProductData p)
        {
            ResourceManager.Singleton.AddARObject(p.ProductInfo.sku, p.model, true);
        }
    }
}
