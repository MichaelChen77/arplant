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
            if (catRect.childCount != ResourceManager.Singleton.FurCategories.Length)
            {
                Clear(catRect);
                foreach (FurCategory cat in ResourceManager.Singleton.FurCategories)
                {
                    AddCategory(cat);
                }
                StartCoroutine(DelayRefresh());
            }
            catRect.anchoredPosition = new Vector2(0, catRect.anchoredPosition.y);
        }

        void AddCategory(FurCategory cat)
        {
            GameObject obj = Instantiate(furPrefab, catGroup.transform);
            obj.transform.localScale = Vector3.one;
            ResObjectItem item = obj.GetComponent<ResObjectItem>();
            item.Init(cat.name, string.Empty, cat.thumbnail, ShowCategory);
        }

        IEnumerator DelayRefresh()
        {
            yield return new WaitForEndOfFrame();
            Refresh();
        }

        public override void Refresh()
        {
            catRect.sizeDelta = new Vector2(catGroup.preferredWidth, catRect.sizeDelta.y);
            furRect.sizeDelta = new Vector2(furGroup.preferredWidth, furRect.sizeDelta.y);
        }

        void AddResObj(ResObject res)
        {
            GameObject obj = Instantiate(furPrefab, furGroup.transform);
            obj.transform.localScale = Vector3.one;
            obj.name = res.name;
            ResObjectItem item = obj.GetComponent<ResObjectItem>();
            item.Init(res.Category, res.name, res.thumbnail, LoadObject);
        }

        void ShowCategory(string cat, string name)
        {
            LeanTween.moveY(contentRect, furRectPos, moveTime);
            backButton.interactable = true;;
            FurCategory fcat = ResourceManager.Singleton.GetCategory(cat);
            if (fcat != null)
            {
                Clear(furRect);
                foreach (ResObject obj in fcat.Furnitures)
                {
                    AddResObj(obj);
                }
                titleText.text = fcat.name;
            }
            StartCoroutine(DelayRefresh());
        }

        public void GotoCatalogue()
        {
            backButton.interactable = false;
            LeanTween.moveY(contentRect, originPos, moveTime);
        }

        public void Search(string str)
        {
            //DataManager.Singleton.Search(str);
            Debug.Log("Search " + str);
        }

        void LoadObject(string cat, string _name)
        {
            ResourceManager.Singleton.LoadGameObject(cat, _name);
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
