using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IMAV.Service;
using IMAV.Model;
using IMAV.Effect;

namespace IMAV.Controller
{
    public class ProductController : MonoBehaviour
    {
        TouchService touchService;
        public TouchService TouchService
        {
            get { return touchService; }
        }

        SelectState selected = SelectState.None;
		public SelectState Selected
        {
            get { return selected; }
            set { selected = value; }
        }

        protected Product product;
        public Product ProductModel
		{
			get { return product; }
			set { product = value; }
		}

        Vector3 localTop = Vector3.one;
        bool isChoosed = false;
        public bool Choosed
        {
            get { return isChoosed; }
            set { 
                isChoosed = value; 
                EnableOutline(isChoosed);
            }
        }

        List<Outline> outlines = new List<Outline>();

        private void Start()
        {
            DataUtility.SetObjectLayer(gameObject, SceneController.ProductLayer);
        }

        void Update()
        {
            if (selected == SelectState.Actived)
                touchService.Update();
        }

		public void Delete()
		{
            if (Choosed)
                SceneController.Singleton.ChooseProduct(false);
			Destroy(gameObject);
		}

        public void SetTouchService(TouchService service)
        {
            touchService = service;
            touchService.Start();
        }

        void SetOutline()
        {
			MeshRenderer[] childRenders = GetComponentsInChildren<MeshRenderer>();
            float top = float.MinValue;
			if (childRenders != null)
			{
				foreach (MeshRenderer mr in childRenders)
				{
                    if (top < mr.bounds.max.y)
                        top = mr.bounds.max.y;
                    if (mr.tag != "static")
                    {
                        Outline ot = mr.gameObject.AddComponent<Outline>();
                        ot.enabled = false;
                        outlines.Add(ot);
                    }
				}
			}
            localTop = new Vector3(0, top - transform.position.y, 0);
        }

        public Vector3 GetTopPosition()
        {
            return transform.position + localTop;
        }

        public void EnableOutline(bool flag)
        {
            if(outlines.Count == 0)
            {
                SetOutline();
            }
            for (int i = 0; i < outlines.Count; i++)
            {
                outlines[i].enabled = flag;
            }
        }
    }
}
