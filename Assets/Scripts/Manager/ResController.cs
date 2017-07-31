using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IMAV
{
    public class ResController : MonoBehaviour
    {
        public FurCategory[] FurCategories;

        public void LoadLocalResource(FurCategory[] cats)
        {
            LoadResources();
            DataManager.Singleton.FurnitureDatas.Clear();
            int k = 0;
            for (int i = 0; i < FurCategories.Length; i++)
            {
                for (int j = 0; j < FurCategories[i].Furnitures.Count; j++)
                {
                    DataManager.Singleton.Init(k, FurCategories[i].Furnitures[j]);
                    k++;
                }
            }
            cats = FurCategories;
        }

        void LoadResources()
        {
            foreach (FurCategory cat in FurCategories)
            {
                Object[] objs = Resources.LoadAll("Furnitures/" + cat.name);
                cat.Furnitures.Clear();
                if (objs != null)
                {
                    for (int i = 0; i < objs.Length; i++)
                    {
                        ResObject o = new ResObject(cat.name, objs[i].name);
                        o.resource = (GameObject)objs[i];
                        o.thumbnail = Resources.Load<Sprite>("FurImages/" + cat.name + "/" + objs[i].name);
                        cat.Furnitures.Add(o);
                    }
                }
            }
        }
    }
}
