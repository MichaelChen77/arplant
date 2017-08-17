using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace IMAV.UI
{
    public class UIObjectMenu : UIControl
    {
        public RectTransform headerRect;
        public RectTransform bodyRect;
        public HorizontalLayoutGroup contentGroup;
        public UIProductInforDialog infoDialog;
        public float moveTime = 0.3f;
        public GameObject relativeButton;
        public GameObject productPrefab;
        public SpinUI loadingUI;

        bool menushowed = true;

        ARProduct currentObject;
        ProductData product;
        int loadingProductCount = 0;
        RectTransform contentRect;

        private void Awake()
        {
            contentRect = contentGroup.GetComponent<RectTransform>();
        }

        public override void Open()
        {
            currentObject = ResourceManager.Singleton.CurrentObject;
            if (currentObject != null)
            {
                DataCenter.Singleton.GetProduct(currentObject.SKU, InitProduct);
                base.Open();
            }
        }

        public void Open(float size)
        {
            Open();
            SetExtend(size);
        }

        public void SetExtend(float size)
        {
            if (gameObject.activeSelf)
            {
                RectTransform productRect = GetComponent<RectTransform>();
                productRect.offsetMax = new Vector2(size, productRect.offsetMax.y);
            }
        }

        void InitProduct(ProductData data)
        {
            product = data;
            if (product.ProductInfo.product_links == null || product.ProductInfo.product_links.Count == 0)
                relativeButton.SetActive(false);
            else
                relativeButton.SetActive(true);
        }

        void ShowMenu(bool flag)
        {
            if (menushowed != flag)
            {
                menushowed = flag;
                if (flag)
                {
                    LeanTween.moveY(headerRect, 0, moveTime);
                    LeanTween.moveY(bodyRect, -bodyRect.sizeDelta.y, moveTime);
                }
                else
                {
                    LeanTween.moveY(headerRect, -headerRect.sizeDelta.y, moveTime);
                    LeanTween.moveY(bodyRect, 0, moveTime);
                }
            }
        }

        public void ShowInformation()
        {
            infoDialog.Open(product);
        }

        public void OpenRelativeMenu()
        {
            Clear();
            ShowMenu(false);
            loadingUI.Show();
            loadingProductCount = product.ProductInfo.product_links.Count;
            for (int i = 0; i < product.ProductInfo.product_links.Count; i++)
            {
                DataCenter.Singleton.GetProduct(product.ProductInfo.product_links[i], AddProductItem);
            }
        }

        void AddProductItem(ProductData data)
        {
            GameObject obj = Instantiate(productPrefab, contentGroup.transform);
            obj.transform.localScale = Vector3.one;
            obj.name = data.ProductInfo.sku;
            ResObjectItem item = obj.GetComponent<ResObjectItem>();
            item.Init(data.ProductInfo.name, data.ProductInfo.sku, data.icon, LoadObject);
            loadingProductCount--;
            if (loadingProductCount == 0)
            {
                loadingUI.Hide();
                StartCoroutine(DelayRefresh());
            }
        }

        void LoadObject(string cat, System.Object sku)
        {
            DataCenter.Singleton.LoadModelData((string)sku);
            BacktoMenu();
        }

        public void OpenMaterialMenu()
        {
            ShowMenu(false);
        }

        public void ResetObject()
        {
            currentObject.ResetTransform();
        }

        public void DuplicateObject()
        {
            GameObject obj = Instantiate(currentObject.gameObject, currentObject.transform.parent);
            ARProduct m = obj.GetComponent<ARProduct>();
            m.SKU = currentObject.SKU;
            obj.transform.position += new Vector3(0, 0, 10);
            ResourceManager.Singleton.SetAsARObject(obj);
        }

        public void DeleteObject()
        {
            ResourceManager.Singleton.DeleteCurrentObject();
            Close();
        }

        public void BacktoMenu()
        {
            ShowMenu(true);
        }

        IEnumerator DelayRefresh()
        {
            yield return new WaitForEndOfFrame();
            Refresh();
        }

        public override void Refresh()
        {
            contentRect.anchoredPosition = new Vector2(0, contentRect.anchoredPosition.y);
            contentRect.sizeDelta = new Vector2(contentGroup.preferredWidth + 10, contentRect.sizeDelta.y);
        }

        public void Clear()
        {
            foreach (Transform tran in contentGroup.transform)
            {
                Destroy(tran.gameObject);
            }
            contentGroup.transform.DetachChildren();
        }

        public override void Close()
        {
            headerRect.anchoredPosition = new Vector2(headerRect.anchoredPosition.x, 0);
            bodyRect.anchoredPosition = new Vector2(bodyRect.anchoredPosition.x, -bodyRect.rect.height);
            base.Close();
        }

        public void Share()
        {
            MediaCenter.Singleton.ShareString(MagentoService.webPageUrl + product.ProductInfo.name + ".html");
        }
    }
}
