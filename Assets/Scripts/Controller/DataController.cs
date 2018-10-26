using System.Collections.Generic;
using System.Collections;
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

//        Stack<string> productkeys = new Stack<string>();
		ArrayList productkeys1 = new ArrayList();

		//test
		Dictionary<long, SubCategory> subCategoies = new Dictionary<long, SubCategory>();
		public Dictionary<long, SubCategory> SubCategoies
		{
			get { return subCategoies; }
		}
		Stack<long> subCategorykeys = new Stack<long>();

		//test
		private int maxCount = 5;

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
			Debug.Log (" CheckForUpdates()");
            DataCollector.Singleton.TopCategories(json =>
            {
				Debug.Log (" CheckForUpdates() before json = " + json);
                loadInitData(json);
				Debug.Log (" CheckForUpdates() after json = " + json);
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

		//test
		public void GetSubCatImage(SubCategory cat)
		{
			DataCollector.Singleton.GetCategoryImage(cat.sub_id, (sp)=>
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

		//test
		public SubCategory GetSubCategory(long id)
		{
			long parent_id = id / 100;
			Debug.Log ("GetSubCategory subcat id = " + id);
			for (int i = 0; i < categories.Count; i++)
			{
				if (categories [i].id == parent_id) {
//					Debug.Log ("current subcat parent_id = " + categories[i].id);
					for (int j = 0; j < categories [i].SubCategorys.Count; j++) {
//						Debug.Log ("current subcat id = " + categories [i].SubCategorys[j].sub_id);
						if (id == categories [i].SubCategorys [j].sub_id) {
//							Debug.Log ("return subcat");
							return categories [i].SubCategorys [j];
						}
					}
				}
			}
			return null;
		}

		//useless
        public void GetCatProduct(Category cat)
        {
            DataCollector.Singleton.GetProductsInCategory(cat.id, (json) =>
            {
                cat.Products = JsonConvert.DeserializeObject<List<CategoryProduct>>(json);
                cat.IsLoaded = true;
            });
        }

		//test
		public void GetSubCatAndProduct(Category cat)
		{
			DataCollector.Singleton.GetProductsInCategory(cat.id, (json) =>
				{
//					test 
//					cat.SubCategorys = JsonConvert.DeserializeObject<List<SubCategory>>(json);
//					Debug.Log("cat.id = " + cat.id + " SubCat num = " + cat.SubCategorys.Count);
//					foreach (SubCategory subcat in cat.SubCategorys) {
//						Debug.Log("subcat.id = " + subcat.sub_id);
//						Debug.Log("subcat.parent_id = " + subcat.parent_id);
//						Debug.Log("subcat.name = " + subcat.sub_name);
//					}

//					//test 
////					cat.CCategorys = JsonConvert.DeserializeObject<List<CategoryCategory>>(json);
//					Debug.Log("cat.id = " + cat.id + " CatCat num = " + cat.CCategorys.Count);
//					foreach (CategoryCategory ccat in cat.CCategorys) {
//						Debug.Log("ccat.id = " + ccat.subcatid);
//						Debug.Log("ccat.parent_id = " + ccat.parcatid);
//						Debug.Log("ccat.name = " + ccat.subcatname);
//					}

					//test
//					cat.SubCategorys = JsonConvert.DeserializeObject<List<SubCategory>>(json);
//					cat.Products = JsonConvert.DeserializeObject<List<CategoryProduct>>(json);

					Debug.Log("cat.id = " + cat.id + " cat.name = " + cat.name);

//					//test
//					CollectorServiceType collectorType = CollectorServiceType.Magento;
//					if (collectorType == CollectorServiceType.Local)
//					{
//						cat.Products = JsonConvert.DeserializeObject<List<CategoryProduct>>(json);
//					}
//					cat.Products = JsonConvert.DeserializeObject<List<CategoryProduct>>(json);
					Debug.Log("cat.id = " + cat.id + " cp num = " + cat.Products.Count);
					foreach (CategoryProduct cp in cat.Products) {
						Debug.Log("cp.sku = " + cp.sku);
					}

					cat.IsLoaded = true;
				});
		}

		//test
		public void GetSubCatProduct(SubCategory subCat)
		{
//			Debug.Log("scat.id = " + subCat.sub_id + " scat.name = " + subCat.sub_name);
//			Debug.Log("scat.parentid = " + subCat.parent_id + " scat.parent_name = " + subCat.parent_name);
//			Debug.Log("scat.products num = " + subCat.Products.Count);
			DataCollector.Singleton.GetProductsInSubCategory(subCat.sub_name, (json) =>
				{
//					subCat.Products = JsonConvert.DeserializeObject<List<CategoryProduct>>(json);
					Debug.Log("scat.id = " + subCat.sub_id + " scat.name = " + subCat.sub_name);
					Debug.Log("scat.parentid = " + subCat.parent_id + " scat.parent_name = " + subCat.parent_name);
					Debug.Log("scat.products num = " + subCat.Products.Count);
//					foreach (CategoryProduct scp in subCat.Products) {
//						Debug.Log("scp.sku = " + scp.sku);
//					}
					//test
//					subCat.Products = JsonConvert.DeserializeObject<List<CategoryProduct>>(json);
					subCat.IsLoaded = true;
				});
		}

        public void GetProduct(string sku, long categoryID, Action<Product> callback)
        {
			if (products.ContainsKey (sku)) {
//				findflag = -1;//find the product
//				productkeys1.Remove (sku);
//				productkeys1.Insert(0, sku);
				callback (products [sku]);
			}
            else
            {
                DataCollector.Singleton.GetProductDetail(sku, (p) =>
                {
//						if (p.sku != ""){
//							findflag = -1;//find the product
//						}else {
//							findflag = -2;//didn't find the product
//						}
//                    products[p.sku] = p;
////                    productkeys.Push(p.sku);
////                    if (products.Count > 30)
////                    {
////                        string str = productkeys.Pop();
////                        products[str].Delete();
////                        products.Remove(str);
////                    }
//						productkeys1.Insert(0, p.sku);
//
//						if (products.Count > maxCount)
//						{
//							Debug.Log("productkeys1 = " + productkeys1[maxCount - 1].ToString());
//							string str = productkeys1[maxCount - 1].ToString();
//							products[str].Delete();
//							products.Remove(str);
//						}
                    GetProductImage(p, callback);
                });
            }
        }

		//        public void GetProduct(string sku, long categoryID, Action<Product> callback)
		public void CheckProductExist(string sku, ref bool isProductExist, ref bool isInSubCategory, 
			ref long subCategoryID, ref CategoryProduct cp){
			Debug.Log ("CheckProductExist sku = " + sku);
			Debug.Log ("CheckProductExist categories.Count = " + categories.Count);
			for (int i = 0; i < categories.Count; i++)
			{
				foreach (CategoryProduct mp in categories[i].Products) 
				{
					//product exists, in main category
					if (mp.sku == sku) {
						Debug.Log ("CheckProductExist match! mp");
						isInSubCategory = false;
						isProductExist = true;
						cp = mp;
						return;
					}
				}

				//product exists, in subcategory
				Debug.Log ("CheckProductExist categories[" + i + 
					"].SubCategorys.Count = " + categories[i].SubCategorys.Count);
				foreach (SubCategory sc in categories[i].SubCategorys) {
					foreach (CategoryProduct sp in sc.Products) {
						if (sp.sku == sku) {
							Debug.Log ("CheckProductExist match! sp");
							isInSubCategory = true;
							isProductExist = true;
							subCategoryID = sc.sub_id;
							cp = sp;
							return;
						}
					}
				}
			}
			cp = null;
			return;
		}
		//test
		//public void GetSubProduct(string sku, Action<Product> callback)



		public void GetSubCategoryProduct(long subCatID, string sku, Action<Product> callback)
		{
			if (products.ContainsKey(sku))
				callback(products[sku]);
			else
			{
				DataCollector.Singleton.GetSubCategoryProductDetail(subCatID, sku, (p) =>
					{
//						products[p.sku] = p;
////						productkeys.Push(p.sku);
////						if (products.Count > 30)
////						{
////							string str = productkeys.Pop();
////							products[str].Delete();
////							products.Remove(str);
////						}
//						productkeys1.Insert(0, p.sku);
//
//						if (products.Count > maxCount)
//						{
//							Debug.Log("productkeys1 = " + productkeys1[maxCount - 1].ToString());
//							string str = productkeys1[maxCount - 1].ToString();
//							products[str].Delete();
//							products.Remove(str);
//						}
						GetSubCategoryProductImage(subCatID, p, callback);
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

		public void GetSubCategoryProductImage(long SubCatID, Product p, Action<Product> callback)
		{
			DataCollector.Singleton.GetSubCategoryProductImage(SubCatID, p.sku, (sp)=>
				{
					p.icon = sp;
					callback(p);
				});
		}

		//test 
		public void GetSubcategory (long subCatID, string subCatName, Action<SubCategory> callback){
			if (subCategoies.ContainsKey(subCatID))
				callback(subCategoies[subCatID]);
			else
			{
				DataCollector.Singleton.GetSubCategoryDetail(subCatID, subCatName, (sc) =>
					{
						subCategoies[subCatID] = sc;
						subCategorykeys.Push(subCatID);
						if (subCategoies.Count > 30)
						{
							long subid = subCategorykeys.Pop();
							subCategoies[subid].Delete();
							subCategoies.Remove(subid);
						}
						GetSubCategoryImage(sc, callback);
					});
			}
		}

		public void GetSubCategoryImage(SubCategory sc, Action<SubCategory> callback)
		{
			DataCollector.Singleton.GetSubCategoryImage(sc.sub_id, sc.sub_name, (sp)=>
				{
					sc.icon = sp;
					callback(sc);
				});
		}
        

        public void LoadModelData(string sku)
        {
            GetProduct(sku, 0, GetProductModel);
        }

		public void LoadSubModelData(long subCatID, string sku)
		{
			GetSubCategoryProduct(subCatID, sku, GetProductModel);
		}

        void GetProductModel(Product p)
        {
			//test
			Debug.Log ("GetProductModel products.count= " + productkeys1.Count);
//			DataUtility.DeleteProductModelFile ("Vase1");
			foreach (string pp in productkeys1)
			{
				Debug.Log ("before pp.sku = " + pp);
			}

			if (products.ContainsKey (p.sku)) {
				productkeys1.Remove (p.sku);
				productkeys1.Insert(0, p.sku);
			}
			else
			{
				products[p.sku] = p;
				productkeys1.Insert(0, p.sku);
				if (products.Count > maxCount)
				{
					Debug.Log("productkeys1 = " + productkeys1[maxCount].ToString());
					string str = productkeys1[maxCount].ToString();
					products[str].Delete();
					products.Remove(str);
				}
			}

			foreach (string pp in productkeys1)
			{
				Debug.Log ("after pp.sku = " + pp);
			}

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

        public void RefreshData()
        {
            
        }
    }
}
