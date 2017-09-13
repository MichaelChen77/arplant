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
        public float furRectPos = 100f;
        public float moveTime = 0.3f;
        public GameObject furPrefab;
        public Text titleText;
        public GToggleButton backButton;
        public RectTransform contentRect;
        public HorizontalLayoutGroup catGroup;
        public HorizontalLayoutGroup furGroup;
        public SpinUI loadingUI;
        bool replaced = false;
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
            GameObject obj = Instantiate(furPrefab, catGroup.transform);
            obj.transform.localScale = Vector3.one;
            ResObjectItem item = obj.GetComponent<ResObjectItem>();
            item.Init(cat.name, cat.id, cat.icon, ShowCategory);
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
            DataController.Singleton.GetProduct(res.sku, AddProductItem);
        }

        void AddProductItem(Product p)
        {
            GameObject obj = Instantiate(furPrefab, furGroup.transform);
            obj.transform.localScale = Vector3.one;
            obj.name = p.sku;
            ResObjectItem item = obj.GetComponent<ResObjectItem>();
            item.Init(p.name, p.sku, p.icon, LoadObject);
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

        void ShowCategory(string _name, System.Object cat)
        {
            LeanTween.moveY(contentRect, furRectPos, moveTime);

            backButton.setTrigger(false);
            Category fcat = DataController.Singleton.GetCategory((long)cat);
            if (fcat != null)
            {
                Clear(furRect);
                StartCoroutine(LoadCategory(fcat));
            }
            else
                Debug.Log("fcat is null");
        }

        int loadingProductCount = 0;
        int loadedProductCount = 0;
        IEnumerator LoadCategory(Category c)
        {
            loadingUI.Show();
            if (!c.IsLoaded)
                c.LoadProducts();
            yield return new WaitUntil(()=>c.IsLoaded);
            loadingProductCount = c.Products.Count;
            loadedProductCount = 0;
            foreach (CategoryProduct obj in c.Products)
            {
                AddResObj(obj);
            }
            titleText.text = c.name;
        }

        public void GotoCatalogue(SceneUIController ctl)
        {
            if (backButton.TriggerFlag)
            {
                ctl.CloseUI(false);
            }
            else
            {
                backButton.setTrigger(true);
                LeanTween.moveY(contentRect, originPos, moveTime);
            }
        }

        public void Search(string str)
        {
            Debug.Log("Search " + str);
        }

        void LoadObject(string cat, System.Object sku)
        {
            if (replaced)
                SceneController.Singleton.DeleteCurrentProduct();
            DataController.Singleton.LoadModelData((string)sku);
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
