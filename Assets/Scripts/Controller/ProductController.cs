using UnityEngine;
using IMAV.Service;
using IMAV.Model;

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

		Vector3 localTop = Vector3.zero;
		bool isChoosed = false;
		public bool Choosed
		{
			get { return isChoosed; }
			set { 
				isChoosed = value; 
				//EnableOutline(isChoosed);
			}
		}

		//List<Outline> outlines = new List<Outline>();

		private void Start()
		{
			DataUtility.SetObjectLayer(gameObject, SceneController.ProductLayer);
			SetOutline();
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
				}
			}
			localTop = new Vector3(0, top - transform.position.y, 0);
		}

		public Vector3 GetTopPosition()
		{
			return transform.position + localTop;
		}
	}
}
