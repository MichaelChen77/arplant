using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

namespace IMAV
{
    public class UIManager : MonoBehaviour {

        public MainCamCtrl camCtrl;
        public Transform room;
        public Text lightText;
        public ObjectGroup lights;

        private static UIManager mSingleton;
        public static UIManager Singleton
        {
            get
            {
                return mSingleton;
            }
        }

        SceneObject selectedObj;
        public SceneObject SelectedObj
        {
            get { return selectedObj; }
        }
		GameObject roomObj;
        int lightIndex = 0;

        void Awake()
        {
            if (mSingleton)
			{
                Destroy(gameObject);
            }
            else
            {
                mSingleton = this;
            }
        }

        void Start()
        {
			if (DataUtility.CurrentObject != null) {
				roomObj = Instantiate (DataUtility.CurrentObject);
				DataUtility.CurrentObject.SetActive (false);
			}
			if (roomObj != null)
            {
				roomObj.transform.parent = room;
				roomObj.SetActive(true);
				ARModel model = roomObj.GetComponent<ARModel> ();
				StartCoroutine(SetObjectState (model, SelectState.None));
				selectedObj = roomObj.GetComponent<SceneObject>();
				SceneObject s = DataUtility.CurrentObject.GetComponent<SceneObject> ();
				selectedObj.SetTransform(s.OriginalScale, s.OriginalRotation);
				PutObjectOnFloor (roomObj.transform);
				SetCamCtrl ();
            }
            GameObject lobj = lights.GetCurrentObject();
            if (lobj != null)
                lightText.text = lobj.name;
        }

		IEnumerator SetObjectState(ARModel mo, SelectState _state)
		{
			yield return new WaitForEndOfFrame ();
			mo.Selected = _state;
		}

		public void SetCamCtrl()
		{
			if (camCtrl.orbitPivot == null) {
				camCtrl.orbitPivot = roomObj.transform;
				camCtrl.SetorbitDistance ();
			} else {
				camCtrl.orbitPivot = null;
			}
		}

		public void SetCamLight()
		{
			Light l = camCtrl.GetComponentInChildren<Light> ();
			if (l != null)
				l.enabled = !l.enabled;
		}

		public void PutObjectOnFloor(Transform tran)
		{
			RaycastHit hit;
			float dist = 0;
			if (Physics.Raycast(tran.position, Vector3.down, out hit, LayerMask.NameToLayer("Room"))){
				dist = hit.distance;
				tran.localPosition = new Vector3 (0, dist, 0);
			}
			else
				tran.localPosition = new Vector3(0, 1.4f, 0);
		}

        public void SetNextLight()
        {
            GameObject obj = lights.ActiveNextObject();
            if (obj != null)
                lightText.text = obj.name;
        }

        public void SetPrevLight()
        {
            GameObject obj = lights.ActivePrevObject();
            if (obj != null)
                lightText.text = obj.name;
        }

        public void GoToMainScene()
        {
            if(selectedObj.materialID != -1)
            {
                DataUtility.CurrentObject.SetActive(true);
                SceneObject obj = DataUtility.CurrentObject.GetComponent<SceneObject>();
                if(obj != null)
                {
                    obj.SetMaterial(selectedObj.materialID, MaterialManager.Singleton.materails[selectedObj.materialID]);
                }
            }
            SceneManager.LoadScene("ARScene");
        }

    }
}
