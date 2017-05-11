using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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

        //public SceneObject tempobj;
        SceneObject selectedObj;
        public SceneObject SelectedObj
        {
            get { return selectedObj; }
        }
        SceneObject dragObj;
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
            GameObject obj = null;
            if (DataUtility.CurrentObject != null)
                obj = Instantiate(DataUtility.CurrentObject);
            if (obj != null)
            {
                obj.SetActive(true);
                DestroyImmediate(obj.GetComponent<ObjectTouchControl>());
                obj.transform.parent = room;
                selectedObj = obj.GetComponent<SceneObject>();
                selectedObj.transform.localPosition = new Vector3(0, 1.4f, 0);
                selectedObj.ResumeTransform();
                camCtrl.SetorbitDistance();
            }
            GameObject lobj = lights.GetCurrentObject();
            if (lobj != null)
                lightText.text = lobj.name;
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
