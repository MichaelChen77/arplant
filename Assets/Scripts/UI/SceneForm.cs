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
            foreach(SceneData d in DataManager.Singleton.Scenes)
            {
                AddItem(d);
            }
        }

        void AddItem(SceneData _data)
        {
            GameObject obj = Instantiate(scenePrefab);
            SceneItem _item = obj.GetComponent<SceneItem>();
            _item.SetValue(_data);
        }

        public void Clear()
        {
            foreach(Transform tran in content)
            {
                Destroy(tran.gameObject);
            }
            content.DetachChildren();
        }
    }
}
