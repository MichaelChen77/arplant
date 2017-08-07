using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace IMAV.UI
{
    public class UICatalogPanel : UIControl
    {
        public float furRectPos = 100f;
        public float moveTime = 0.3f;
        public GameObject furPrefab;
        public Text titleText;
        public Button backButton;
        public RectTransform contentRect;
        public HorizontalLayoutGroup catGroup;
        public HorizontalLayoutGroup furGroup;
        public SpinUI loadingUI;
        RectTransform catRect;
        RectTransform furRect;
        float originPos;

        private void Awake()
        {
            catRect = catGroup.GetComponent<RectTransform>();
            furRect = furGroup.GetComponent<RectTransform>();
            originPos = contentRect.anchoredPosition.y;
        }

        public override void Open()
        {
            gameObject.SetActive(true);
            LoadObjects();
        }

        void LoadObjects()
        {
            if (catRect.childCount != DataCenter.Singleton.Categories.Count)
            {
                Clear(catRect);
                foreach (CategoryData cat in DataCenter.Singleton.Categories)
                {
                    AddCategoryItem(cat);
                }
                StartCoroutine(DelayRefresh());
            }
            catRect.anchoredPosition = new Vector2(0, catRect.anchoredPosition.y);
        }

        void AddCategoryItem(CategoryData cat)
        {
            GameObject obj = Instantiate(furPrefab, catGroup.transform);
            obj.transform.localScale = Vector3.one;
            ResObjectItem item = obj.GetComponent<ResObjectItem>();
            item.Init(cat.Cat.name, cat.Cat.id, cat.icon, ShowCategory);
        }

        IEnumerator DelayRefresh()
        {
            yield return new WaitForEndOfFrame();
            Refresh();
        }

        public override void Refresh()
        {
            furRect.anchoredPosition = new Vector2(0, furRect.anchoredPosition.y);
            catRect.sizeDelta = new Vector2(catGroup.preferredWidth+20, catRect.sizeDelta.y);
            furRect.sizeDelta = new Vector2(furGroup.preferredWidth+20, furRect.sizeDelta.y);
        }

        void AddResObj(CategoryProduct res)
        {
            DataCenter.Singleton.GetProduct(res.sku, AddProductItem);
        }

        void AddProductItem(ProductData p)
        {
            GameObject obj = Instantiate(furPrefab, furGroup.transform);
            obj.transform.localScale = Vector3.one;
            obj.name = p.ProductInfo.sku;
            ResObjectItem item = obj.GetComponent<ResObjectItem>();
            item.Init(p.ProductInfo.name, p.ProductInfo.sku, p.icon, LoadObject);
            loadingProductCount--;
            if (loadingProductCount == 0)
            {
                loadingUI.Hide();
                StartCoroutine(DelayRefresh());
            }
        }

        void ShowCategory(string name, System.Object cat)
        {
            LeanTween.moveY(contentRect, furRectPos, moveTime);
            backButton.interactable = true;
            CategoryData fcat = DataCenter.Singleton.GetCategory((long)cat);
            if (fcat != null)
            {
                Clear(furRect);
                StartCoroutine(LoadCategory(fcat));
            }
            else
                Debug.Log("fcat is null");
        }

        int loadingProductCount = 0;
        IEnumerator LoadCategory(CategoryData c)
        {
            loadingUI.Show();
            if (!c.IsLoaded())
                c.LoadProducts();
            yield return new WaitUntil(c.IsLoaded);
            loadingProductCount = c.Products.Count;
            foreach (CategoryProduct obj in c.Products)
            {
                AddResObj(obj);
            }
            titleText.text = c.Cat.name;
        }

        public void GotoCatalogue()
        {
            backButton.interactable = false;
            LeanTween.moveY(contentRect, originPos, moveTime);
        }

        public void Search(string str)
        {
            Debug.Log("Search " + str);
        }

        void LoadObject(string cat, System.Object sku)
        {
            DataCenter.Singleton.LoadModelData((string)sku);
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

        public override void Close()
        {
            contentRect.anchoredPosition = new Vector2(contentRect.anchoredPosition.x, originPos);
            base.Close();
        }
    }
}
