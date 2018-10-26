using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using IMAV.Model;
using IMAV.Controller;

namespace IMAV.UI
{
    public class UICatalogPanel : UIControl
    {
//        public float furRectPos = 100f;
		//test
//		public float subCatRecPos = 100f;
		public float subCatRecPos = 100f;
		public float furRectPos = 320f;

        public float moveTime = 0.3f;
        public GameObject iconPrefab;
        public Text titleText;
        public GToggleButton backButton;
        public RectTransform contentRect;
        public HorizontalLayoutGroup catGroup;
		public HorizontalLayoutGroup subCatGroup;
        public HorizontalLayoutGroup furGroup;
        public SpinUI loadingUI;
        bool replaced = false;
        RectTransform catRect;
		RectTransform subCatRect;
        RectTransform furRect;
        float originPosY;

		int currentLayer = 0;//0->main category, 1->subcat+product or subcat or product, 2->products
		string parentCategoryName;

        private void Awake()
        {
			parentCategoryName = "Catalogue";
			titleText.text = "Catalogue";
            catRect = catGroup.GetComponent<RectTransform>();
			subCatRect = subCatGroup.GetComponent<RectTransform> ();//test
            furRect = furGroup.GetComponent<RectTransform>();
            originPosY = contentRect.anchoredPosition.y;
        }

        public override void Open()
		{
			titleText.text = "Catalogue";
			currentLayer = 0;
            gameObject.SetActive(true);
            backButton.setTriggerWithoutAnimation(true);
            LoadObjects();
        }

        public void Open(bool isSwitch)
        {
            replaced = isSwitch;
            Open();
        }

        void LoadObjects()
        {
            if (catRect.childCount != DataController.Singleton.Categories.Count)
            {
                Clear(catRect);
				Debug.Log ("how many main catalogue? " + DataController.Singleton.Categories.Count); //->4
                foreach (Category cat in DataController.Singleton.Categories)
                {
                    AddCategoryItem(cat);
                }
                StartCoroutine(DelayRefresh());
            }
			catRect.anchoredPosition = new Vector2(0, catRect.anchoredPosition.y);
        }

        void AddCategoryItem(Category cat)
        {
			GameObject obj = Instantiate(iconPrefab, catGroup.transform);
//			//test 
//			GameObject obj;
//			Debug.Log ("AddCategoryItem currentLayer " + currentLayer);
//			if (currentLayer == 0) {
//				obj = Instantiate (iconPrefab, catGroup.transform);
//			}
//			else {//else if (currentLayer == 1)
//				obj = Instantiate (iconPrefab, subCatGroup.transform);
//			}
				
            obj.transform.localScale = Vector3.one;
            ResObjectItem item = obj.GetComponent<ResObjectItem>();
			//attach the action "ShowCategory" for the catalogue. once the catalogue's icon was click,
			//it will call the function ShowCategory
//            item.Init(cat.name, cat.id, cat.icon, ShowCategory);
			//ShowSubCategory -> show what's inside the first category, 2nd layer
			Debug.Log("AddCategoryItem cat.name = " + cat.name);
			Debug.Log("AddCategoryItem cat.id = " + cat.id);
			if (cat.icon != null) {
				Debug.Log ("AddCategoryItem cat.icon = " + cat.icon);
			}
			item.Init(cat.name, cat.id, cat.icon, ShowSecondLayer);
        }

		//test -> The second layer
		void ShowSecondLayer(string _name, System.Object cat)
		{
			Debug.Log ("ShowSubCategory!");
			currentLayer = 1;
			parentCategoryName = "Catalogue";
			LeanTween.moveY(contentRect, subCatRecPos, moveTime);
			//			Category tempC = (Category)cat;
			//			parentCategoryName = tempC.parent_name;

			backButton.setTrigger(false);
			Category fcat = DataController.Singleton.GetCategory((long)cat);
			if (fcat != null)
			{
				parentCategoryName = fcat.name;
				//				Clear(furRect);
				//				//test
				//				if (currentLayer == 0) {
				//					Clear(subCatRect);
				//				}
				//				else if (currentLayer == 1) {
				//					Clear(furRect);
				//				}
				Clear(subCatRect);
				//				StartCoroutine(LoadSubCategory(fcat));
				Debug.Log("begin LoadSecondLayer!");
				StartCoroutine(LoadSecondLayer(fcat));
			}
			else
				Debug.Log("fcat is null");
		}



		void AddSubCategoryItem(SubCategory scat)
		{
			GameObject obj = Instantiate(iconPrefab, subCatGroup.transform);
			obj.transform.localScale = Vector3.one;
			ResObjectItem item = obj.GetComponent<ResObjectItem>();
			item.Init(scat.sub_name, scat.sub_id, scat.icon, ShowThirdLayer);
		}




        IEnumerator DelayRefresh()
        {
            yield return new WaitForEndOfFrame();
            Refresh();
        }

        public override void Refresh()
        {
			//test
			subCatRect.anchoredPosition = new Vector2 (0, subCatRect.anchoredPosition.y);
            furRect.anchoredPosition = new Vector2(0, furRect.anchoredPosition.y);
            catRect.sizeDelta = new Vector2(catGroup.preferredWidth+20, catRect.sizeDelta.y);
			subCatRect.sizeDelta = new Vector2(subCatGroup.preferredWidth+20, subCatRect.sizeDelta.y);
            furRect.sizeDelta = new Vector2(furGroup.preferredWidth+20, furRect.sizeDelta.y);
        }

		void AddResSubCat (SubCategory res)
		{
			DataController.Singleton.GetSubcategory(res.sub_id, res.sub_name, AddSubCatItem);
		}

		void AddSubCatItem(SubCategory sc)
		{
			//			GameObject obj = Instantiate(iconPrefab, furGroup.transform);
			//test
			GameObject obj = Instantiate (iconPrefab, subCatGroup.transform);

			obj.transform.localScale = Vector3.one;
			obj.name = sc.sub_name;
			ResObjectItem item = obj.GetComponent<ResObjectItem>();
			item.Init(sc.sub_name, sc.sub_id, sc.icon, ShowThirdLayer);
			loadingProductCount--;
			loadedProductCount++;
			if (loadedProductCount > 8)
				loadingUI.Hide();
			if (loadingProductCount == 0)
			{
				loadingUI.Hide();
				StartCoroutine(DelayRefresh());
			}
		}

		//test search
//		void checkSearchProductResult(string ProductName)
//		{
//			long flag = -2;
//			DataController.Singleton.GetProduct(ProductName, flag, ShowSearchProductResultItem);
//		}

        void AddResObj(CategoryProduct res)
        {
			DataController.Singleton.GetProduct(res.sku, res.category_id, AddProductItem);
        }
			
		void AddSubResObj(long subID, CategoryProduct res)
		{
			DataController.Singleton.GetSubCategoryProduct(subID, res.sku, AddProductItem);
		}

//		//test search
//		void ShowSearchProductResultItem(Product p)
//		{
//			if (p.sku == "")
//				return;
//			GameObject obj;
//			obj = Instantiate (iconPrefab, subCatGroup.transform);
//			obj.transform.localScale = Vector3.one;
//			obj.name = p.sku;
//			ResObjectItem item = obj.GetComponent<ResObjectItem>();
//			item.Init(p.name, p.sku, p.icon, LoadObject);
//		}

        void AddProductItem(Product p)
        {
//			GameObject obj = Instantiate(iconPrefab, furGroup.transform);
			//test
			GameObject obj;
			Debug.Log ("AddProductItem currentLayer " + currentLayer);
			if (currentLayer == 1) {
				obj = Instantiate (iconPrefab, subCatGroup.transform);
			}
			else {//else if (currentLayer == 2)
				obj = Instantiate (iconPrefab, furGroup.transform);
			}

            obj.transform.localScale = Vector3.one;
            obj.name = p.sku;
            ResObjectItem item = obj.GetComponent<ResObjectItem>();
//            item.Init(p.name, p.sku, p.icon, LoadObject);
			//test 
			if (currentLayer == 1) {
				item.Init (p.name, p.sku, p.icon, LoadObject);
			} else {
				item.Init (p.category_id, p.sku, p.icon, LoadSubObject);
			}
			//else item.Init(p.name, p.sku, p.icon, LoadSubObject);

            loadingProductCount--;
            loadedProductCount++;
            if (loadedProductCount > 8)
                loadingUI.Hide();
            if (loadingProductCount == 0)
            {
                loadingUI.Hide();
                StartCoroutine(DelayRefresh());
            }
        }

		//show the third layer
//        void ShowCategory(string _name, System.Object cat)
//        {
////            LeanTween.moveY(contentRect, furRectPos, moveTime);
//			//move test:
//			Debug.Log ("ShowCategory currentLayer " + currentLayer);
//			if (currentLayer == 0) {
//				parentCategoryName = "Catalogue";
//				LeanTween.moveY (contentRect, subCatRecPos, moveTime);
//			} else if (currentLayer == 1) {
//				Category tempC = (Category)cat;
//				parentCategoryName = tempC.parent_name;
//				LeanTween.moveY (contentRect, furRectPos, moveTime);
//			}
//				
//            backButton.setTrigger(false);
//            Category fcat = DataController.Singleton.GetCategory((long)cat);
//            if (fcat != null)
//            {
////                Clear(furRect);
//				//test
//				if (currentLayer == 0) {
//					Clear(subCatRect);
//				}
//				else if (currentLayer == 1) {
//					Clear(furRect);
//				}
//
////                StartCoroutine(LoadCategory(fcat));
//				//test-> 3 layers
//				StartCoroutine(LoadCategoryFor3layers(fcat));
//            }
//            else
//                Debug.Log("fcat is null");
//			currentLayer++;
//        }

		//test -> The third layer
		void ShowThirdLayer(string _name, System.Object cat)
		{
			currentLayer = 2;
			Debug.Log ("showfinalcategory");
			LeanTween.moveY (contentRect, furRectPos, moveTime);
			backButton.setTrigger(false);
			SubCategory scat = DataController.Singleton.GetSubCategory((long)cat);
			Debug.Log ("showfinalcategory scat.id = " + scat.sub_id);
			if (scat != null) {
				Clear(furRect);
				Debug.Log ("before loadfinalcategory scat.subid = " + scat.sub_id);
				StartCoroutine(LoadThirdLayer(scat));
			}
		}


        int loadingProductCount = 0;
        int loadedProductCount = 0;
//		int loadingSubCatagoryCount = 0;
//		int loadedSubCatagoryCount = 0;

		//this function is useless
        IEnumerator LoadCategory(Category c)
        {
            loadingUI.Show();
            if (!c.IsLoaded)
                c.LoadProducts();
            yield return new WaitUntil(()=>c.IsLoaded);
            loadingProductCount = c.Products.Count;
            loadedProductCount = 0;
//			Debug.Log ("How many proudcts in this catalogue? " + c.Products.Count);
            foreach (CategoryProduct obj in c.Products)
            {
                AddResObj(obj);
            }
            titleText.text = c.name;
        }

		IEnumerator LoadThirdLayer(SubCategory sc)
		{
			Debug.Log ("LoadFinalCategory begin ");
			loadingUI.Show();
			if (!sc.IsLoaded)
				sc.LoadProducts();
			yield return new WaitUntil(()=>sc.IsLoaded);
			loadingProductCount = sc.Products.Count;
			loadedProductCount = 0;

			Debug.Log ("How many proudcts in this sub catalogue? " + sc.Products.Count);
			foreach (CategoryProduct obj in sc.Products)
			{
				AddSubResObj(sc.sub_id, obj);
			}
			titleText.text = sc.sub_name;
		}

		//test
		IEnumerator LoadSearchLayer(CategoryProduct cp, long subCategoryID)
		{
			Debug.Log ("LoadSearchLayer subCategoryID = " + subCategoryID);
			loadingUI.Show();
			yield return null;
			if (subCategoryID >= 0) {
				AddSubResObj (subCategoryID, cp);
			} else {
				AddResObj (cp);
			}
		}

		//test
		IEnumerator LoadSearchLayer(string productName)
		{
			Debug.Log ("LoadSearchLayer productName = " + productName);
			loadingUI.Show();
			yield return null;
			foreach (Category cat in DataController.Singleton.Categories) {
				foreach (CategoryProduct mcp in cat.Products) {
					if (mcp.sku.Contains (productName) == true) {
						AddResObj (mcp);
					}
				}
				foreach (SubCategory scat in cat.SubCategorys) {
					foreach (CategoryProduct scp in scat.Products) {
						if (scp.sku.Contains (productName) == true) {
							AddSubResObj (scp.category_id, scp);
						}
					}
				}
			}


		}


		//test
		IEnumerator LoadSecondLayer(Category c)
		{	
			loadingUI.Show();
			if (!c.IsLoaded) {
				c.LoadSubCatagoryAndProducts ();
			}
			yield return new WaitUntil(()=>c.IsLoaded);

			//			//test
			//			loadingSubCatagoryCount = c.SubCategory.Count;
			//			loadedSubCatagoryCount = 0;
			Debug.Log ("How many subcat in this catalogue? " + c.SubCategorys.Count);
			foreach (SubCategory subObj in c.SubCategorys)
			{
				Debug.Log ("subcatName " + subObj.sub_name);
//				AddSubCategoryItem (subObj);
				AddResSubCat (subObj);
			}

			loadingProductCount = c.Products.Count;
			loadedProductCount = 0;
			Debug.Log ("How many proudcts in this catalogue? " + c.Products.Count);
			foreach (CategoryProduct obj in c.Products)
			{
				Debug.Log ("CategoryProduct " + obj.sku);
				AddResObj(obj);
			}

			titleText.text = c.name;
		}

		//test search
//		IEnumerator LoadSearchLayer


//		//test->3 layers
//		IEnumerator LoadCategoryFor3layers(Category c)
//		{
//			loadingUI.Show();
//			if (!c.IsLoaded) {
////				c.LoadSubCatagory ();
//				c.LoadSubCatagoryAndProducts ();
//			}
//			yield return new WaitUntil(()=>c.IsLoaded);
//
////			//test
////			loadingSubCatagoryCount = c.SubCategory.Count;
////			loadedSubCatagoryCount = 0;
//			Debug.Log ("How many subcat in this catalogue? " + c.SubCategory.Count);
//			foreach (Category subObj in c.SubCategory)
//			{
//				Debug.Log ("subcatName " + subObj.name);
//				AddCategoryItem (subObj);
//			}
//
//			loadingProductCount = c.Products.Count;
//			loadedProductCount = 0;
//			Debug.Log ("How many proudcts in this catalogue? " + c.Products.Count);
//			foreach (CategoryProduct obj in c.Products)
//			{
//				Debug.Log ("CategoryProduct " + obj.sku);
//				AddResObj(obj);
//			}
//
//			titleText.text = c.name;
//		}

        public void GotoCatalogue(SceneUIController ctl)
        {

			//test
			Debug.Log ("GotoCatalogue currentLayer " + currentLayer);
			if (currentLayer == 2) {
				titleText.text = parentCategoryName;
				backButton.setTrigger(false);
				LeanTween.moveY (contentRect, subCatRecPos, moveTime);
			} else if (currentLayer == 1) {
				titleText.text = "Catalogue";
				backButton.setTrigger(true);
				LeanTween.moveY (contentRect, originPosY, moveTime);
			} else if (currentLayer == 0) {
				ctl.CloseUI (false);
			}
			currentLayer--;
//			titleText.text = parentCategoryName;
        }

		//ShowSearchLayer
        public void Search(string str)
        {
            Debug.Log("Search " + str);
			currentLayer = 1;//show the result in the subcategory layer
			titleText.text = str;
			LeanTween.moveY(contentRect, subCatRecPos, moveTime);
			backButton.setTrigger(false);
			Clear(subCatRect);

//			bool isProductExist = false;
//			bool isInSubCategory = false; 
//			long subCategoryID = -1;
//			CategoryProduct pFind = new CategoryProduct ();
//			DataController.Singleton.CheckProductExist(str, ref isProductExist,
//				ref isInSubCategory,ref subCategoryID, ref pFind);
//			Debug.Log ("Search isProductExist = " + isProductExist);
//			Debug.Log ("Search isInSubCategory = " + isInSubCategory);
//			Debug.Log ("Search subCategoryID = " + subCategoryID);
//			Debug.Log ("Search pFind = " + pFind.sku);
//			if (isProductExist && !isInSubCategory) {
//				Debug.Log ("isProductExist && !isInSubCategory");
//				StartCoroutine(LoadSearchLayer(pFind, subCategoryID));
//			} else if (isProductExist && isInSubCategory) {
//				Debug.Log ("isProductExist && isInSubCategory");
//				StartCoroutine(LoadSearchLayer(pFind, subCategoryID));
//			}
			StartCoroutine(LoadSearchLayer(str));

			StartCoroutine(DelayRefresh());
        }

        void LoadObject(string cat, System.Object sku)
        {
            if (replaced)
                SceneController.Singleton.DeleteCurrentProduct();
            DataController.Singleton.LoadModelData((string)sku);
            Close();
        }

		void LoadSubObject(long sub_id, System.Object sku)
		{
			if (replaced)
				SceneController.Singleton.DeleteCurrentProduct();
			DataController.Singleton.LoadSubModelData(sub_id, (string)sku);
			Close();
		}

        void Clear(Transform content)
        {
            foreach (Transform tran in content)
            {
                Destroy(tran.gameObject);
            }
            content.DetachChildren();
        }

        public void RefreshCacheData()
        {
            //DataController.Singleton.GetCategory();
        }

        public override void Close()
        {
            contentRect.anchoredPosition = new Vector2(contentRect.anchoredPosition.x, originPosY);
            base.Close();
        }
    }
}
