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
			Debug.Log("LoadResource() json = " + json);
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

		public IEnumerator GetProductsInSubCategory(string subCategoryName, Action<string> callback)
		{
			Debug.Log ("subCategoryName = " + subCategoryName);
			yield return null;
			foreach(Category cat in catList)
			{
				foreach(SubCategory scat in cat.SubCategorys)
				{
					Debug.Log ("scat.sub_name = " + scat.sub_name);
					if(scat.sub_name == subCategoryName)
					{
//						SubCategory sc = new SubCategory();
//						sc = scat;
						callback(JsonConvert.SerializeObject(scat.Products));
						break;
					}
				}
			}
		}

		public IEnumerator GetSubCategoryDetail(long sub_id, string sub_name, SubCategoryDownloadCallback callback)
		{
			bool toEnd = false;
			yield return null;
			foreach (Category cat in catList)
			{
				foreach(SubCategory scat in cat.SubCategorys)
				{
					if(scat.sub_id == sub_id)
					{
						SubCategory sc = new SubCategory();
						sc = scat;
						callback(sc);
						toEnd = true;
						break;
					}
				}
				if (toEnd)
					break;
			}
		}

		public IEnumerator GetSubCategoryImage(long subCategoryId, string subCategoryName, ImageDownloadCallback callback)
		{
			bool toEnd = false;
			yield return null;
			foreach (Category cat in catList)
			{
				foreach (SubCategory sc in cat.SubCategorys)
				{
					if (sc.sub_id == subCategoryId)
					{
						WWW www = new WWW(Application.streamingAssetsPath+ "/Products/" + cat.name + "/" + sc.sub_name +".png");
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
						p.category_id = cp.category_id;
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

		//test
		public IEnumerator GetSubCategoryProductDetail(long subCategoryId, string sku, ProductDownloadCallback callback)
		{
			bool toEnd = false;
			yield return null;

			int parentID = (int)(subCategoryId / 100);
			SubCategory scat = catList[parentID].SubCategorys[(int)(subCategoryId - parentID * 100)];
			Debug.Log ("GetSubCategoryProductDetail scat.Products.Count = " + scat.Products.Count);
			Debug.Log ("sku = " + sku);
			foreach(CategoryProduct cp in scat.Products)
			{
				Debug.Log ("cp.sku = " + cp.sku);
				if(cp.sku == sku)
				{
					Debug.Log ("GetSubCategoryProductDetail return");
					Product p = new Product();
					p.category_id = cp.category_id;
					p.sku = sku;
					p.name = cp.sku;
					callback(p);
					toEnd = true;
					break;
				}
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
			
		//test
		public IEnumerator GetSubCategoryProductImage(long subCategoryId, string sku, ImageDownloadCallback callback)
		{
			bool toEnd = false;
			yield return null;
			foreach (Category cat in catList)
			{
				foreach (SubCategory scat in cat.SubCategorys) 
				{
					foreach (CategoryProduct cp in scat.Products)
					{
						if (cp.sku == sku)
						{
							WWW www = new WWW(Application.streamingAssetsPath+ "/Products/" + scat.parent_name + "/" + scat.sub_name + "/" + cp.sku+".png");
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
		}


		public IEnumerator GetSubCategoryProductTexture(long subCategoryId, string sku, TextureDownloadCallback callback)
		{
			Debug.Log ("Icons GetSubCategoryProductTexture");
			bool toEnd = false;
			yield return null;

			int parentID = (int)(subCategoryId / 100);
			SubCategory scat = catList[parentID].SubCategorys[(int)(subCategoryId - parentID * 100)];
			Debug.Log ("GetSubCategoryProductTexture scat.Products.Count = " + scat.Products.Count);
			Debug.Log ("sku = " + sku);

			Texture2D ta = Resources.Load("ProductIcons/" + scat.parent_name + "/" + scat.sub_name + "/" + sku, typeof(Texture2D)) as Texture2D;
			callback(ta);
		}
        public IEnumerator GetProductTexture(string sku, TextureDownloadCallback callback)
        {
			Debug.Log ("Icons GetProductTexture");
            bool toEnd = false;
            yield return null;
            foreach (Category cat in catList)
            {
                foreach (CategoryProduct cp in cat.Products)
                {
                    if (cp.sku == sku)
                    {
                        Texture2D ta = Resources.Load("ProductIcons/" + cat.name + "/" + cp.sku, typeof(Texture2D)) as Texture2D;
                        callback(ta);
                        toEnd = true;
                        break;
                    }
                }
                if (toEnd)
                    break;
            }
        }

		public IEnumerator GetSubCategoryTexture(long subCategoryId, string subCategoryName, TextureDownloadCallback callback)
		{
			Debug.Log ("Icons GetSubCategoryTexture");
			bool toEnd = false;
			yield return null;

			foreach (Category cat in catList)
			{
				foreach (SubCategory sc in cat.SubCategorys)
				{
					if (sc.sub_id == subCategoryId)
					{
						Debug.Log ("ProductIcons/" + cat.name + "/" + sc.sub_name);
						Texture2D ta = Resources.Load("ProductIcons/" + cat.name + "/" + sc.sub_name, typeof(Texture2D)) as Texture2D;
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
			Debug.Log ("Icons GetCategoryTexture");
            yield return null;
            foreach (Category cat in catList)
            {
                if (cat.id == categoryId)
				{
                    Texture2D ta = Resources.Load("ProductIcons/" + cat.name, typeof(Texture2D)) as Texture2D;
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

				//test
				foreach (SubCategory scat in cat.SubCategorys)
				{
					foreach (CategoryProduct cp in scat.Products)
					{
						if (cp.sku == sku)
						{
							System.Object ta = Resources.Load("Products/" + scat.parent_name + "/" + scat.sub_name + "/" + cp.sku);
							callback(sku, new System.Object[] { ta });
							toEnd = true;
							break;
						}
					}
				}


                if (toEnd)
                    break;
            }
        }
    }
}
