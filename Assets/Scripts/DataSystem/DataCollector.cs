using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IMAV.Model;

public delegate void CategoryDownloadCallback(List<Category> categories);
public delegate void CategoryProductDownloadCallback(long categoryId, List<CategoryProduct> cpList);
public delegate void ProductDownloadCallback(Product p);
public delegate void SubCategoryDownloadCallback(SubCategory sc);//test
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
        public bool useCache = true;


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
			DataUtility.SetDirectory(DataUtility.GetCategoryPath());
			DataUtility.SetDirectory(DataUtility.GetSubCategoryPath());
//			DataUtility.SetDirectory(DataUtility.GetSubCategoryIconPath());
			DataUtility.SetDirectory(DataUtility.GetProductPath());
			DataUtility.SetDirectory(DataUtility.GetProductIconPath());
			DataUtility.SetDirectory(DataUtility.GetProductModelPath());
            if(useCache)
            {
                DataUtility.SetDirectory(DataUtility.GetCategoryPath());
				DataUtility.SetDirectory(DataUtility.GetSubCategoryPath());
//				DataUtility.SetDirectory(DataUtility.GetSubCategoryIconPath());
                DataUtility.SetDirectory(DataUtility.GetProductPath());
                DataUtility.SetDirectory(DataUtility.GetProductIconPath());
                DataUtility.SetDirectory(DataUtility.GetProductModelPath());
            }
            switch(collectorType)
            {
                case CollectorServiceType.Local: collector = new LocalDataService(); break;
                case CollectorServiceType.Magento: collector = new MagentoService(useCache); break;
            }
        }

        public void ClearData()
        {
			//System.IO.DirectoryInfo di = new DirectoryInfo("YourPath");

			//foreach (FileInfo file in di.GetFiles())
			//{
			//	file.Delete();
			//}
			//foreach (DirectoryInfo dir in di.GetDirectories())
			//{
			//	dir.Delete(true);
			//}
        }

		public void SwitchLocalAndWeb()
		{
			
		}

        public void TopCategories(Action<string> callback)
        {
            StartCoroutine(collector.TopCategories(callback));
        }

        public void GetProductsInCategory(long categoryId, Action<string> callback)
        {
            StartCoroutine(collector.GetProductsInCategory(categoryId, callback));
        }

		public void GetProductsInSubCategory(string subCategoryName, Action<string> callback)
		{
			StartCoroutine(collector.GetProductsInSubCategory(subCategoryName, callback));
		}

//		//test
//		public void GetSubCatAndProductsInCategory(long categoryId, Action<string> callback)
//		{
//			StartCoroutine(collector.GetSubCatAndProductsInCategory(categoryId, callback));
//		}


        public void GetProductDetail(string sku, ProductDownloadCallback callback)
        {
            StartCoroutine(collector.GetProductDetail(sku, callback));
        }

		public void GetSubCategoryProductDetail(long sub_id, string sku, ProductDownloadCallback callback)
		{
			StartCoroutine(collector.GetSubCategoryProductDetail(sub_id, sku, callback));
		}

		public void GetSubCategoryDetail(long sub_id, string sub_name, SubCategoryDownloadCallback callback)
		{
			StartCoroutine(collector.GetSubCategoryDetail(sub_id, sub_name, callback));
		}


        public void GetProductImage(string sku, Action<Sprite> callback)
        {
            if (collectorType == CollectorServiceType.Local)
            {
                StartCoroutine(collector.GetProductTexture(sku, (tex) =>
                {
                    Sprite sp = DataUtility.CreateSprite(tex);
                    callback(sp);
                }));
            }
            else
            {
                StartCoroutine(collector.GetProductImage(sku, (bytes) =>
                {
                    Sprite sp = DataUtility.CreateSprite(bytes);
                    callback(sp);
                }));
            }
        }

		public void GetSubCategoryProductImage(long sub_id, string sku, Action<Sprite> callback)
		{
			if (collectorType == CollectorServiceType.Local)
			{
				StartCoroutine(collector.GetSubCategoryProductTexture(sub_id, sku, (tex) =>
					{
						Sprite sp = DataUtility.CreateSprite(tex);
						callback(sp);
					}));
			}
			else
			{
				StartCoroutine(collector.GetSubCategoryProductImage(sub_id, sku, (bytes) =>
					{
						Sprite sp = DataUtility.CreateSprite(bytes);
						callback(sp);
					}));
			}
		}




		public void GetSubCategoryImage(long sub_id, string sub_name, Action<Sprite> callback)
		{
			if (collectorType == CollectorServiceType.Local)
			{
				StartCoroutine(collector.GetSubCategoryTexture(sub_id, sub_name, (tex) =>
					{
						Sprite sp = DataUtility.CreateSprite(tex);
						callback(sp);
					}));
			}
			else
			{
				StartCoroutine(collector.GetSubCategoryImage(sub_id, sub_name, (bytes) =>
					{
						Sprite sp = DataUtility.CreateSprite(bytes);
						callback(sp);
					}));
			}
		}

        public void GetProductTexture(string sku, TextureDownloadCallback callback)
        {
            StartCoroutine(collector.GetProductTexture(sku, callback));
        }

        public void GetCategoryImage(long categoryId, Action<Sprite> callback)
        {
            if (collectorType == CollectorServiceType.Local)
            {
                StartCoroutine(collector.GetCategoryTexture(categoryId, (tex) =>
                {
                    Sprite sp = DataUtility.CreateSprite(tex);
                    callback(sp);
                }));
            }
            else
            {
                StartCoroutine(collector.GetCategoryImage(categoryId, (bytes) =>
                {
                    Sprite sp= DataUtility.CreateSprite(bytes);
                    callback(sp);
                }));
            }
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
