using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace IMAV.UI
{
    public class SearchForm : MonoBehaviour
    {
        public InputField searchInput;
        public Dropdown categoryDropdown;
        public GameObject furniturePrefab;
        public RectTransform content;
        public float itemHeight = 90;
        public float spacing = 0;
        List<FurnitureItem> items = new List<FurnitureItem>();
        List<FurnitureData> searcheResult = new List<FurnitureData>();
        public List<FurnitureData> SearchResult
        {
            get { return searcheResult; }
        }

        // Use this for initialization
        void Start()
        {
            categoryDropdown.options.Clear();
            for (int i = 0; i < Tags.FurnitureCategory.Length; i++)
            {
                categoryDropdown.options.Add(new Dropdown.OptionData(Tags.FurnitureCategory[i]));
            }
            categoryDropdown.value = 0;
            categoryDropdown.captionText.text = categoryDropdown.options[0].text;
        }

        public void Close()
        { 
            gameObject.SetActive(false);
        }

        public void Open()
        {
            gameObject.SetActive(true);
            searchInput.ActivateInputField();
        }

        public void Search(string str)
        {
            gameObject.SetActive(true);
            searcheResult.Clear();
            //string _cat = categoryDropdown.options[categoryDropdown.value].text;
            DataManager.Singleton.Search(str);
        }

        public void Refresh()
        {
            int i = 0;
            foreach (FurnitureData _data in searcheResult)
            {
                if (items.Count > i)
                    items[i].LoadData(_data);
                else
                    AddItem(_data, -content.sizeDelta.y);
                i++;
            }
            if (items.Count > i)
            {
                for (int j = items.Count - 1; j > i; j--)
                {
                    RemoveItem(j);
                }
                RemoveItem(i);
            }
        }

        void RemoveItem(int _index)
        {
            FurnitureItem _temp = items[_index];
            items.RemoveAt(_index);
            Destroy(_temp.gameObject);
            AddContentHeight(-itemHeight - spacing);
        }

        void AddItem(FurnitureData _data, float height)
        {
            GameObject obj = Instantiate(furniturePrefab);
            FurnitureItem _item = obj.GetComponent<FurnitureItem>();
            _item.LoadData(_data);
            RectTransform rect = obj.GetComponent<RectTransform>();
            rect.SetParent(content);
            rect.localScale = Vector3.one;
            items.Add(_item);
            rect.offsetMin = new Vector2(0, height - itemHeight);
            rect.offsetMax = new Vector2(0, height);
            AddContentHeight(itemHeight + spacing);
        }

        void AddContentHeight(float size)
        {
            content.sizeDelta = new Vector2(content.sizeDelta.x, content.sizeDelta.y + size);
        }
    }
}
