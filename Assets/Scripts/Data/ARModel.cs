using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IMAV
{
	public enum SelectState
	{
		Actived, Pause, None
	}

	public class ARModel : MonoBehaviour {

		ObjectTouchControl touchCtrl;

		/// <summary>
		/// Whether the object is selected
		/// </summary>
		SelectState selected = SelectState.None;
		public SelectState Selected {
			get{ return selected; }
			set { 
				selected = value;
				if (touchCtrl == null)
					Init ();
				if (selected == SelectState.Actived)
					touchCtrl.SetActive (true);
				else if (selected == SelectState.None)
					touchCtrl.SetActive (false);
			}
		}

		void Start()
		{
			Init ();
            //BoxCollider box = GetComponent<BoxCollider>();
            //if(box != null)
            //{
            //    GameObject plane = Instantiate(Resources.Load("ShadowPlane", typeof(GameObject))) as GameObject;
            //    if (plane != null)
            //    {
            //        plane.transform.parent = transform;
            //        Vector3 boundDiff = box.bounds.center - transform.position;
            //        plane.transform.localPosition = transform.rotation * boundDiff + transform.rotation * Vector3.Scale(box.bounds.extents, new Vector3(0, 0, -1));
            //        plane.transform.localScale = new Vector3(box.bounds.extents.x * 0.2f, 1, box.bounds.extents.y * 0.1f);
            //    }
            //}
		}

		void Init () {
			touchCtrl = GetComponent<ObjectTouchControl> ();
			if (touchCtrl == null)
				touchCtrl = gameObject.AddComponent<ObjectTouchControl> ();
			touchCtrl.Init (this);
            CloseShadowCast();
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

		public void Delete()
		{
			touchCtrl.Delete ();
			Destroy (gameObject);
		}
	}
}
