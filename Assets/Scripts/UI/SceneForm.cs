using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IMAV.UI
{
    public class SceneForm : MonoBehaviour
    {
        public GameObject scenePrefab;
        public Transform content;

        List<SceneItem> items = new List<SceneItem>();

        // Use this for initialization
        void Start()
        {

        }

        public void Open()
        {
            gameObject.SetActive(true);
        }

        public void Close()
        {
            gameObject.SetActive(false);
        }

        public void Refresh()
        {
            Clear();
            //if (DataManager.Singleton.Scenes != null)
            //{
            //    foreach (SceneData d in DataManager.Singleton.Scenes.data)
            //    {
            //        AddItem(d);
            //    }
            //}
        }

        void AddItem(SceneData _data)
        {
            GameObject obj = Instantiate(scenePrefab);
            SceneItem _item = obj.GetComponent<SceneItem>();
            _item.SetValue(_data, ClickSceneItem);
            obj.transform.SetParent(content);
        }

        public void Clear()
        {
            foreach(Transform tran in content)
            {
                Destroy(tran.gameObject);
            }
            content.DetachChildren();
            items.Clear();
        }

        public void ClickSceneItem(SceneItem _item)
        {
            //DataManager.Singleton.LoadSceneItem(_item.Data);
            Close();
        }
    }
}
