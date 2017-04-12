using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace IMAV.UI
{
    public class MaterialForm : MonoBehaviour
    {

        public MeshRenderer render1;
        public MeshRenderer render2;
        public MeshRenderer render3;
        public MeshRenderer render4;
        public Text nameText1;
        public Text nameText2;
        public Text nameText3;
        public Text nameText4;
        public Button NextButton;
        public Button PrevButton;

        SceneObject currentObj;
        int currentMtID = 0;
        bool rotated = true;

        void Start()
        {
            render1.material = MaterialManager.Singleton.materails[currentMtID];
            render2.material = MaterialManager.Singleton.materails[currentMtID + 1];
            render3.material = MaterialManager.Singleton.materails[currentMtID + 2];
            render4.material = MaterialManager.Singleton.materails[currentMtID + 3];
        }

        public void Open()
        {
            if (UIManager.Singleton.SelectedObj != null)
            {
                gameObject.SetActive(true);
                currentObj = UIManager.Singleton.SelectedObj;
                UpdateMoveBtnState();
                SetRotateState(rotated);
                GoNext();
            }
            else
                Close();
        }

        public void Close()
        {
            gameObject.SetActive(false);
        }

        public void GoNext()
        {
            if (currentMtID < MaterialManager.Singleton.MaterialCount-4)
            {
                if (currentMtID < MaterialManager.Singleton.MaterialCount - 8)
                    currentMtID += 4;
                else
                    currentMtID += MaterialManager.Singleton.MaterialCount - currentMtID - 4;
                AssignMaterials();
            }
            UpdateMoveBtnState();
        }

        void AssignMaterials()
        {
            render1.material = MaterialManager.Singleton.materails[currentMtID];
            render2.material = MaterialManager.Singleton.materails[currentMtID + 1];
            render3.material = MaterialManager.Singleton.materails[currentMtID + 2];
            render4.material = MaterialManager.Singleton.materails[currentMtID + 3];
            nameText1.text = render1.material.name;
            nameText2.text = render2.material.name;
            nameText3.text = render3.material.name;
            nameText4.text = render4.material.name;
        }

        void UpdateMoveBtnState()
        {
            if (currentMtID < MaterialManager.Singleton.MaterialCount - 4)
                NextButton.interactable = true;
            else
                NextButton.interactable = false;
            if (currentMtID > 0)
                PrevButton.interactable = true;
            else
                PrevButton.interactable = false;
        }

        public void GoPrev()
        {
            if (currentMtID > 0)
            {
                if (currentMtID > 3)
                    currentMtID -= 4;
                else
                    currentMtID = 0;
                AssignMaterials();
            }
            UpdateMoveBtnState();
        }

        public void SetRotateState()
        {
            rotated = !rotated;
            SetRotateState(rotated);
        }

        void SetRotateState(bool flag)
        {
            SetRotate(render1, flag);
            SetRotate(render2, flag);
            SetRotate(render3, flag);
            SetRotate(render4, flag);
        }

        public void ResetMaterial()
        {
            currentObj.ResumeMaterial();
        }

        void SetRotate(MeshRenderer rend, bool flag)
        {
            Rotate rot1 = rend.GetComponent<Rotate>();
            rot1.rotated = flag;
        }

        public void SelectMaterial(int index)
        {
            int id = currentMtID;
            Material mt = render1.material;
            switch (index)
            {
                case 2: mt = render2.material; id = currentMtID + 1; break;
                case 3: mt = render3.material; id = currentMtID + 2; break;
                case 4: mt = render4.material; id = currentMtID + 3; break;
            }
            if (currentObj != null)
                currentObj.SetMaterial(id, mt);
        }
    }
}
