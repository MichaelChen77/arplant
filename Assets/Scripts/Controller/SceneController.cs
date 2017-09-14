using System;
using System.Collections.Generic;
using UnityEngine;
using IMAV.Model;
using UnityEngine.XR.iOS;

namespace IMAV.Controller
{
    public abstract class SceneController : MonoBehaviour
    {
        protected ProductController currentObject;
        public ProductController CurrentObject
		{
			get { return currentObject; }
		}

        protected List<ProductController> objlist = new List<ProductController>();
		public List<ProductController> ObjList
		{
			get { return objlist; }
		}

		protected static SceneController mSingleton;
		public static SceneController Singleton
		{
			get
			{
				return mSingleton;
			}
		}

        public event Action<bool> ProductChoosedEvent;

		public bool ExistObject
		{
			get { return objlist.Count != 0; }
		}

        public static int ProductLayer;

		protected virtual void Awake()
		{
            ProductLayer = LayerMask.NameToLayer("Product");
		}

        public abstract void AddProductToScene(Product p);

        public virtual void AddProductToScene(string sku)
        {
            DataController.Singleton.LoadModelData(sku);
        }

		public void DeleteCurrentProduct()
		{
            if (currentObject != null)
			{
                currentObject.Delete();
                objlist.Remove(currentObject);
                currentObject = null;
			}
		}

        public void SetCurrentProduct(ProductController obj)
        {
            if (currentObject != null)
            {
                currentObject.Selected = SelectState.None;
                if (currentObject != obj && currentObject.Choosed)
                    ChooseProduct(false);
            }
            currentObject = obj;
            if (currentObject != null)
                currentObject.Selected = SelectState.Actived;
        }

        public virtual void SetCurrentObject(GameObject obj)
        {
            ProductController ctrl = obj.GetComponent<ProductController>();
            if (ctrl == null)
                ctrl = obj.GetComponentInParent<ProductController>();
            SetCurrentProduct(ctrl);
        }

		public virtual void SetCurrentObjectState(SelectState st)
		{
            if (currentObject != null)
				currentObject.Selected = st;
            if (st == SelectState.None)
                currentObject = null;
		}

		public virtual void Clear()
		{
			try
			{
                SetCurrentProduct(null);
                for (int i = objlist.Count - 1; i > -1; i--)
                {
                    objlist[i].Delete();
                }
				objlist.Clear();
			}
			catch (System.Exception ex)
			{
                Debug.Log("Unity: Clear error: " + ex.Message);
			}
		}

        public virtual void Reset()
        {
            Clear();
            ResetARSession();
		}

        public abstract void ResetARSession();

        public abstract void Pause();

        public abstract void Resume();

        public void OnApplicationPause(bool pause)
        {
            if (pause)
                Pause();
            else
                Resume();
        }

        public void ChooseProduct(bool choose)
        {
            currentObject.Choosed = choose;
            if (ProductChoosedEvent != null)
                ProductChoosedEvent(choose);
        }

        protected virtual void showInform(string str, bool autoHide = false)
        {
            MediaController.Singleton.textInform.ShowInform(str, autoHide, 0.5f);
        }
    }
}
