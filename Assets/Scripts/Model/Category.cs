using UnityEngine;
using System.Collections.Generic;
using IMAV.Controller;

namespace IMAV.Model
{
    public class Category
    {
        public long id { get; set; }
        public long parent_id { get; set; }
        public string name { get; set; }
        public bool is_active { get; set; }
        public int position { get; set; }
        public int level { get; set; }
        public int product_count { get; set; }
        public List<Category> children_data { get; set; }

        public Sprite icon;
        bool isloaded = false;
        public bool IsLoaded
        {
            get { return isloaded; }
            set { isloaded = value; }
        }
        List<CategoryProduct> products = new List<CategoryProduct>();
        public List<CategoryProduct> Products
        {
            get { return products; }
            set { products = value; }
        }

        public void LoadIcon()
        {
            DataController.Singleton.GetCatImage(this);
        }

        public void LoadProducts()
        {
            DataController.Singleton.GetCatProduct(this);
        }
    }
}