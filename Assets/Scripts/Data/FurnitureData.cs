using UnityEngine;
using System.IO;

namespace IMAV
{
    public class FurnitureData
    {
        public int id;
        public string name;
        public string category_name;
        public string brand_name;
        public string manufacturer_name;
        public string production_site;
        public string production_date;
        public float price;
        public string thumbnail_path;
        public string object_path;

        public ModelData modeldata;
        protected Sprite thumbnail;
        public Sprite Thumbnail
        {
            get { return thumbnail; }
            set { thumbnail = value; }
        }
        protected GameObject model;
        public GameObject Model
        {
            get { return model; }
            set { model = value; }
        }

        public FurnitureData(int _id, string _name)
        {
            id = _id;
            name = _name;
        }

        public bool CheckModelExist()
        {
            return File.Exists(DataUtility.GetLocalModelFile(this));
        }
    }
}
