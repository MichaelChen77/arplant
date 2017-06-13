using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace IMAV.UI
{
    public class InsertForm : MonoBehaviour
    {

        public Text formText;
        public GameObject backBtn;
        public RectTransform objListContent;
        public GameObject objItemPrefab;
        public float ObjectItemHeight;
        public float spacing;
        public Animator anim;
        public ToggleButton markerBtn;
        public FurARUI appUI;
        string formStr;


        List<FurnitureObjectItem> objItems = new List<FurnitureObjectItem>();

        void Start()
        {
            formStr = formText.text;
        }

        public void ShowObjects(string str)
        {
            int i = 0;
            //foreach (ResObject obj in ResourceManager.Singleton.PresetObjects)
            //{
            //    if (obj.Category == str)
            //    {
            //        if (objItems.Count > i)
            //            objItems[i].Init(obj, Close);
            //        else
            //            AddObjectItems(obj, -objListContent.sizeDelta.y);
            //        i++;
            //    }
            //}
            //if (objItems.Count > i)
            //{
            //    for (int t = objItems.Count - 1; t >= i; t--)
            //    {
            //        Destroy(objItems[t].gameObject);
            //        objItems.RemoveAt(t);
            //        AddContentHeight(-ObjectItemHeight - spacing);
            //    }
            //}
            //anim.SetBool("Show", true);
            //backBtn.SetActive(true);
            //formText.text = str;
        }

        void AddObjectItems(ResObject res, float height)
        {
            GameObject obj = Instantiate(objItemPrefab);
            FurnitureObjectItem _item = obj.GetComponent<FurnitureObjectItem>();
            _item.Init(res, Close);
            RectTransform rect = obj.GetComponent<RectTransform>();
            rect.SetParent(objListContent);
            rect.localScale = Vector3.one;
            objItems.Add(_item);
            rect.offsetMin = new Vector2(0, height - ObjectItemHeight);
            rect.offsetMax = new Vector2(0, height);
            AddContentHeight(ObjectItemHeight + spacing);
        }

        void AddContentHeight(float size)
        {
            objListContent.sizeDelta = new Vector2(objListContent.sizeDelta.x, objListContent.sizeDelta.y + size);
        }

        public void HideObjectList()
        {
            anim.SetBool("Show", false);
            backBtn.SetActive(false);
            formText.text = formStr;
        }

        public void Open()
        {
            gameObject.SetActive(true);
            //appUI.Init(markerBtn);
        }

        public void Close()
        {
            gameObject.SetActive(false);
            backBtn.SetActive(false);
        }
    }
}