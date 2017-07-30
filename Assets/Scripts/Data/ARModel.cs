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
		SelectState selected = SelectState.Actived;
		public SelectState Selected {
			get{ return selected; }
			set { 
				selected = value;
				if (touchCtrl == null)
					Init ();
				if (selected == SelectState.Actived)
					touchCtrl.enabled = true;
				else if (selected == SelectState.None)
					touchCtrl.enabled = false;
			}
		}


        void Start()
        {
            Init();
        }

        void Init () {
            if (touchCtrl == null)
            {
                touchCtrl = gameObject.AddComponent<ObjectTouchControl>();
                touchCtrl.Init(this);
                CloseShadowCast();
            }
		}

        //float calculateBounds(GameObject obj)
        //{
        //    BoxCollider box = obj.GetComponent<BoxCollider>();
        //    if (box != null)
        //    {
        //        GameObject co = new GameObject("dummy");
        //        co.transform.position = obj.transform.position;
        //        co.transform.localScale = obj.transform.lossyScale;
        //        BoxCollider cobc = co.AddComponent<BoxCollider>();
        //        Quaternion quat = obj.transform.rotation;
        //        cobc.center = box.center;
        //        cobc.size = box.size;
        //        Bounds bound = cobc.bounds;
        //        Destroy(co);
        //        Vector3 boundDiff = bound.center - transform.position;
        //        Vector3 boundExtents = bound.extents;

        //        Vector3 delta = quat * boundDiff + quat * Vector3.Scale(boundExtents, new Vector3(0, 0, -1));
        //        ResourceManager.Singleton.DebugString("delta: " + delta);
        //        return -delta.y;
        //    }
        //    return 0;
        //}

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
			touchCtrl.Delete();
			Destroy (gameObject);
		}
	}
}
