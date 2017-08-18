using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IMAV
{
	public enum SelectState
	{
		Actived, Pause, None
	}

	public class ARProduct : MonoBehaviour {
		ObjectTouchControl touchCtrl;
        Vector3 originalSize;
        Quaternion originalRot;
        Vector3 originalPos;

        /// <summary>
        /// Whether the object is selected
        /// </summary>
        SelectState selected = SelectState.Actived;
		public SelectState Selected {
			get{ return selected; }
			set { 
				selected = value;
                if (touchCtrl == null)
                    Init();
                if (selected == SelectState.Actived)
                    touchCtrl.enabled = true;
                else if (selected == SelectState.None)
                    touchCtrl.enabled = false;
            }
		}

        protected string sku;
        public string SKU
        {
            get { return sku; }
            set { sku = value; }
        }

        bool init = false;


        void Start()
        {
            Init();

            //---Stripped down version required---0818
            InitTransform();
        }

        void Init () {
            if (touchCtrl == null)
            {
                touchCtrl = gameObject.AddComponent<ObjectTouchControl>();
                touchCtrl.Init(this);
            }
        }

        public void CloseShadowCast()
        {
            MeshRenderer[] renders = GetComponentsInChildren<MeshRenderer>();
            if (renders != null)
            {
                foreach (MeshRenderer mr in renders)
                {
                    mr.receiveShadows = false;
                }
            }
        }

        public void InitTransform(float y)
        {
            if (!init)
            {
                transform.localPosition = new Vector3(transform.localPosition.x, y, transform.localPosition.z);
                originalSize = transform.localScale;
                originalRot = transform.localRotation;
                originalPos = transform.localPosition;
                init = true;
            }
        }

        public void InitTransform()
        {
            if (!init)
            {
                originalSize = transform.localScale;
                originalRot = transform.localRotation;
                originalPos = transform.localPosition;
                init = true;
            }
        }

        public void ResetTransform()
        {
            transform.localScale = originalSize;
            transform.localPosition = originalPos;
            transform.localRotation = originalRot;
        }

        public void Delete()
        {
            Destroy(gameObject);
        }

        public void SetMaterial()
        {

        }
    }
}
