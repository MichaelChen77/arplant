using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MaterialForm : MonoBehaviour {

    public MeshRenderer render1;
    public MeshRenderer render2;
    public MeshRenderer render3;
    public Text nameText1;
    public Text nameText2;
    public Text nameText3;
    public GToggleButton showOutlineBtn;
    public Button NextButton;
    public Button PrevButton;

    ARObject currentObj;
    Material originMt;
    int currentMtID = 0;

    void Start()
    {
        render1.material = MaterialManager.Singleton.materails[currentMtID];
        render2.material = MaterialManager.Singleton.materails[currentMtID + 1];
        render3.material = MaterialManager.Singleton.materails[currentMtID + 2];
    }

    public void Open(ARObject obj)
    {
        if (obj != null)
        {
            gameObject.SetActive(true);
            showOutlineBtn.setTrigger(true);
            currentObj = obj;
            originMt = obj.GetMaterial();
            UpdateMoveBtnState();
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
        if (currentMtID < MaterialManager.Singleton.MaterialCount - 3)
        {
            if (currentMtID < MaterialManager.Singleton.MaterialCount - 5)
                currentMtID += 3;
            else if (currentMtID == MaterialManager.Singleton.MaterialCount - 5)
                currentMtID += 2;
            else
                currentMtID += 1;
            AssignMaterials();
        }
        UpdateMoveBtnState();
    }

    void AssignMaterials()
    {
        render1.material = MaterialManager.Singleton.materails[currentMtID];
        render2.material = MaterialManager.Singleton.materails[currentMtID + 1];
        render3.material = MaterialManager.Singleton.materails[currentMtID + 2];
        nameText1.text = render1.material.name;
        nameText2.text = render3.material.name;
        nameText3.text = render3.material.name;
    }

    void UpdateMoveBtnState()
    {
        if (currentMtID < MaterialManager.Singleton.MaterialCount - 3)
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
            if (currentMtID > 2)
                currentMtID -= 3;
            else if (currentMtID == 2)
                currentMtID -= 2;
            else
                currentMtID--;
            AssignMaterials();
        }
        UpdateMoveBtnState();
    }

    public void SetOutlineShow()
    {
        showOutlineBtn.setTrigger();
        currentObj.ShowOutline(showOutlineBtn.TriggerFlag);
    }

    public void SetRotateState(bool flag)
    {
        SetRotate(render1, flag);
        SetRotate(render2, flag);
        SetRotate(render3, flag);
    }

    public void ResetMaterial()
    {
        currentObj.SetMaterial(originMt);
    }

    public void LockObject(GToggleButton btn)
    {
        btn.setTrigger();
        UIManager.Singleton.IsLock = btn.TriggerFlag;
    }

    void SetRotate(MeshRenderer rend, bool flag)
    {
        Rotate rot1 = rend.GetComponent<Rotate>();
        rot1.rotated = flag;
    }

    public void SelectMaterial(int index)
    {
        Material mt = render1.material;
        switch(index)
        {
            case 2: mt = render2.material; break;
            case 3: mt = render3.material; break;
        }
        if (currentObj != null)
            currentObj.SetMaterial(mt);
    }
}
