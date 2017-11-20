using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IMAV.Model;

public delegate void CategoryDownloadCallback(List<Category> categories);
public delegate void CategoryProductDownloadCallback(long categoryId, List<CategoryProduct> cpList);
public delegate void ProductDownloadCallback(Product p);
public delegate void ImageDownloadCallback(byte[] bytes);
public delegate void TextureDownloadCallback(Texture2D texture);
public delegate void AssetBundleDownloadCallback(string sku, System.Object[] objs);

namespace IMAV.Service
{
    public enum CollectorServiceType
    {
        Local, Magento, Web
    }

    public class DataCollector : MonoBehaviour
    {
        ICollectorService collector;
        public CollectorServiceType collectorType;

        private static DataCollector mSingleton;
        public static DataCollector Singleton
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
            SetCollectorService();
        }

        public void SetCollectorService()
        {
            switch(collectorType)
            {
                case CollectorServiceType.Local: collector = new LocalDataService(); break;
                case CollectorServiceType.Magento: collector = new MagentoService(); break;
            }
        }

        public void TopCategories(Action<string> callback)
        {
            StartCoroutine(collector.TopCategories(callback));
        }

        public void GetProductsInCategory(long categoryId, Action<string> callback)
        {
            StartCoroutine(collector.GetProductsInCategory(categoryId, callback));
        }

        public void GetProductDetail(string sku, ProductDownloadCallback callback)
        {
            StartCoroutine(collector.GetProductDetail(sku, callback));
        }

        public void GetProductImage(string sku, ImageDownloadCallback callback)
        {
            StartCoroutine(collector.GetProductImage(sku, callback));
        }

        public void GetProductTexture(string sku, TextureDownloadCallback callback)
        {
            StartCoroutine(collector.GetProductTexture(sku, callback));
        }

        public void GetCategoryImage(long categoryId, ImageDownloadCallback callback)
        {
            StartCoroutine(collector.GetCategoryImage(categoryId, callback));
        }

        public void GetCategoryTexture(long categoryId, TextureDownloadCallback callback)
        {
            StartCoroutine(collector.GetCategoryTexture(categoryId, callback));
        }

        public void DownloadAssetBundle(string sku, AssetBundleDownloadCallback callback)
        {
            StartCoroutine(collector.DownloadAssetBundle(sku, callback));
        }
    }
}
