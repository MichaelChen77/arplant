using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectGizmo : MonoBehaviour {

    public UITestControls uiCtrl;

    private BoxCollider b;
    public Transform tCorners;
    public Transform tBounds;
    public TextMesh tmLength;
    public TextMesh tmWidth;
    public TextMesh tmHeight;

    private LineRenderer[] lrCorners;
    private LineRenderer[] lrBounds;
    private Vector3 FTL; //Front Top Left
    private Vector3 FTR; 
    private Vector3 FBL; //Front Bottom Left
    private Vector3 FBR;
    private Vector3 BTL; //Back Top Left
    private Vector3 BTR;
    private Vector3 BBL; //Back Bottom Left
    private Vector3 BBR;

    private Transform selectedObj;
	void Awake()
    {
        lrCorners = tCorners.GetComponentsInChildren<LineRenderer>();
        lrBounds = tBounds.GetComponentsInChildren<LineRenderer>();
    }
    
    public void Select(Transform targetT)
    {
        if (selectedObj == null || selectedObj != targetT)
        {
            selectedObj = targetT;
            //Parent gizmo in selected collider object, to facilitate rotation/scaling.
            transform.SetParent(targetT);
            transform.localPosition = transform.localEulerAngles = Vector3.zero;
            transform.localScale = Vector3.one;
            b = targetT.GetComponent<BoxCollider>();

            //Find the 8 corners of box collider.
            BBR = b.center + new Vector3(b.size.x, -b.size.y, b.size.z) * 0.5f;
            FBR = b.center + new Vector3(b.size.x, -b.size.y, -b.size.z) * 0.5f;
            BTR = b.center + new Vector3(b.size.x, b.size.y, b.size.z) * 0.5f;
            FTR = b.center + new Vector3(b.size.x, b.size.y, -b.size.z) * 0.5f;
            BTL = b.center + new Vector3(-b.size.x, b.size.y, b.size.z) * 0.5f;
            FTL = b.center + new Vector3(-b.size.x, b.size.y, -b.size.z) * 0.5f;
            BBL = b.center + new Vector3(-b.size.x, -b.size.y, b.size.z) * 0.5f;
            FBL = b.center + new Vector3(-b.size.x, -b.size.y, -b.size.z) * 0.5f;

            //Corners LineRenderers.
            lrCorners[0].positionCount = 3;
            lrCorners[0].SetPosition(0, 0.25f * (FTL - BTL) + BTL);
            lrCorners[0].SetPosition(1, BTL);
            lrCorners[0].SetPosition(2, 0.25f * (BTR - BTL) + BTL);
            lrCorners[1].positionCount = 2;
            lrCorners[1].SetPosition(0, 0.25f * (BBL - BTL) + BTL);
            lrCorners[1].SetPosition(1, BTL);

            lrCorners[2].positionCount = 3;
            lrCorners[2].SetPosition(0, 0.25f * (BTL - BTR) + BTR);
            lrCorners[2].SetPosition(1, BTR);
            lrCorners[2].SetPosition(2, 0.25f * (FTR - BTR) + BTR);
            lrCorners[3].positionCount = 2;
            lrCorners[3].SetPosition(0, 0.25f * (BBR - BTR) + BTR);
            lrCorners[3].SetPosition(1, BTR);

            lrCorners[4].positionCount = 3;
            lrCorners[4].SetPosition(0, 0.25f * (BTL - FTL) + FTL);
            lrCorners[4].SetPosition(1, FTL);
            lrCorners[4].SetPosition(2, 0.25f * (FTR - FTL) + FTL);
            lrCorners[5].positionCount = 2;
            lrCorners[5].SetPosition(0, 0.25f * (FBL - FTL) + FTL);
            lrCorners[5].SetPosition(1, FTL);

            lrCorners[6].positionCount = 3;
            lrCorners[6].SetPosition(0, 0.25f * (FTL - FTR) + FTR);
            lrCorners[6].SetPosition(1, FTR);
            lrCorners[6].SetPosition(2, 0.25f * (BTR - FTR) + FTR);
            lrCorners[7].positionCount = 2;
            lrCorners[7].SetPosition(0, 0.25f * (FBR - FTR) + FTR);
            lrCorners[7].SetPosition(1, FTR);

            lrCorners[8].positionCount = 3;
            lrCorners[8].SetPosition(0, 0.25f * (FBR - FBL) + FBL);
            lrCorners[8].SetPosition(1, FBL);
            lrCorners[8].SetPosition(2, 0.25f * (BBL - FBL) + FBL);
            lrCorners[9].positionCount = 2;
            lrCorners[9].SetPosition(0, 0.25f * (FTL - FBL) + FBL);
            lrCorners[9].SetPosition(1, FBL);

            lrCorners[10].positionCount = 3;
            lrCorners[10].SetPosition(0, 0.25f * (FBL - FBR) + FBR);
            lrCorners[10].SetPosition(1, FBR);
            lrCorners[10].SetPosition(2, 0.25f * (BBR - FBR) + FBR);
            lrCorners[11].positionCount = 2;
            lrCorners[11].SetPosition(0, 0.25f * (FTR - FBR) + FBR);
            lrCorners[11].SetPosition(1, FBR);

            lrCorners[12].positionCount = 3;
            lrCorners[12].SetPosition(0, 0.25f * (FBL - BBL) + BBL);
            lrCorners[12].SetPosition(1, BBL);
            lrCorners[12].SetPosition(2, 0.25f * (BBR - BBL) + BBL);
            lrCorners[13].positionCount = 2;
            lrCorners[13].SetPosition(0, 0.25f * (BTL - BBL) + BBL);
            lrCorners[13].SetPosition(1, BBL);

            lrCorners[14].positionCount = 3;
            lrCorners[14].SetPosition(0, 0.25f * (BBL - BBR) + BBR);
            lrCorners[14].SetPosition(1, BBR);
            lrCorners[14].SetPosition(2, 0.25f * (FBR - BBR) + BBR);
            lrCorners[15].positionCount = 2;
            lrCorners[15].SetPosition(0, 0.25f * (BTR - BBR) + BBR);
            lrCorners[15].SetPosition(1, BBR);

            //Bounds LineRenderers
            lrBounds[0].positionCount = 4;
            lrBounds[0].SetPosition(0, FTR);
            lrBounds[0].SetPosition(1, FTL);
            lrBounds[0].SetPosition(2, BTL);
            lrBounds[0].SetPosition(3, BTR);

            lrBounds[1].positionCount = 4;
            lrBounds[1].SetPosition(0, FBR);
            lrBounds[1].SetPosition(1, FTR);
            lrBounds[1].SetPosition(2, BTR);
            lrBounds[1].SetPosition(3, BBR);

            lrBounds[2].positionCount = 4;
            lrBounds[2].SetPosition(0, FBL);
            lrBounds[2].SetPosition(1, FBR);
            lrBounds[2].SetPosition(2, BBR);
            lrBounds[2].SetPosition(3, BBL);

            lrBounds[3].positionCount = 4;
            lrBounds[3].SetPosition(0, FTL);
            lrBounds[3].SetPosition(1, FBL);
            lrBounds[3].SetPosition(2, BBL);
            lrBounds[3].SetPosition(3, BTL);

            //Text Pos
            tmLength.transform.localPosition = 0.5f * (FBL - FBR) + FBR;
            tmWidth.transform.localPosition = 0.5f * (FBR - BBR) + BBR;
            tmHeight.transform.localPosition = 0.5f * (BTR - BBR) + BBR;

            uiCtrl.ToggleObjectUI(true);
        }
        else
        {
            transform.localScale = Vector3.zero;
            selectedObj = null;
            uiCtrl.ToggleObjectUI(false);
        }
    }
}
