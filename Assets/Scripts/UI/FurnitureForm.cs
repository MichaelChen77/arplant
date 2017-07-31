using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace IMAV.UI
{
    public class FurnitureForm : MonoBehaviour
    {
        Animator anim;
        public GameObject furPrefab;
        public Text titleText;
        public VerticalLayoutGroup group;
        public RectTransform content;
        public Scrollbar contentBar;
        public float itemHeight = 100f;

        private void Awake()
        {
            anim = GetComponent<Animator>();
        }

        public void Open()
        {
            gameObject.SetActive(true);
            anim.SetBool("Show", true);
            LoadObjects();
        }

        void LoadObjects()
        {
            Clear();
            contentBar.value = 1;
            foreach (FurCategory cat in ResourceManager.Singleton.FurCategories)
            {
                AddCategory(cat);
            }
            UpdateHeight();
        }

        void AddCategory(FurCategory cat)
        {
            GameObject obj = Instantiate(furPrefab);
            obj.transform.SetParent(content);
            obj.transform.localScale = Vector3.one;
            ResObjectItem item = obj.GetComponent<ResObjectItem>();
            //item.Init(cat.name, string.Empty, cat.thumbnail, ShowCategory);
        }

        void UpdateHeight()
        {
            float y = content.childCount * (itemHeight + group.spacing) + group.padding.top + group.padding.bottom;
            content.sizeDelta = new Vector2(content.sizeDelta.x, y);
        }

        void AddResObj(ResObject res)
        {
            GameObject obj = Instantiate(furPrefab);
            obj.transform.SetParent(content);
            obj.transform.localScale = Vector3.one;
            obj.name = res.name;
            ResObjectItem item = obj.GetComponent<ResObjectItem>();
            //item.Init(res.Category, res.name, res.thumbnail, LoadObject);
        }

        public void MovetoTop()
        {
            contentBar.value = 1;
        }

        public void MovetoBottom()
        {
            contentBar.value = 0;
        }

        void ShowCategory(string cat, string name)
        {
            FurCategory fcat = ResourceManager.Singleton.GetCategory(cat);
            if (fcat != null)
            {
                Clear();
                foreach (ResObject obj in fcat.Furnitures)
                {
                    AddResObj(obj);
                }
                UpdateHeight();
                titleText.text = fcat.name;
                contentBar.value = 1;
            }
        }

        void LoadObject(string cat, string _name)
        {
            ResourceManager.Singleton.LoadGameObject(cat, _name);
            Close();
        }

        void Clear()
        {
            foreach(Transform tran in content)
            {
                Destroy(tran.gameObject);
            }
            content.DetachChildren();
        }

        public void Close()
        {
            anim.SetBool("Show", false);
        }
    }
}
