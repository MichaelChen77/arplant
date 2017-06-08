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
