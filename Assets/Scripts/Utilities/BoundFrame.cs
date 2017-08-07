using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IMAV.UI
{
    public class BoundFrame : MonoBehaviour
    {
        public Transform topFrontLeft;
        public Transform topFrontRight;
        public Transform topBackLeft;
        public Transform topBackRight;
        public Transform bottomFrontLeft;
        public Transform bottomFrontRight;
        public Transform bottomBackLeft;
        public Transform bottomBackRight;
        public Transform frontTop;
        public Transform frontBottom;
        public Transform frontLeft;
        public Transform frontRight;
        public Transform midTopLeft;
        public Transform midBottomLeft;
        public Transform midTopRight;
        public Transform midBottomRight;
        public Transform backTop;
        public Transform backBottom;
        public Transform backLeft;
        public Transform backRight;
        //public Transform bottomPlane;

        private Vector3 boundDiff;
        private Vector3 boundExtents;
        private Quaternion quat;
        private float originalSize;
        GameObject target;
        private Renderer[] renderers;

        public void CloseShadowCast()
        {
            MeshRenderer[] renders = GetComponentsInChildren<MeshRenderer>();
            if (renders != null)
            {
                foreach (MeshRenderer mr in renders)
                {
                    if (mr.name != "Plane")
                    {
                        mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                        mr.receiveShadows = false;
                    }
                }
            }
        }

        public void SetObject(ARModel obj)
        {
            if (obj != null)
            {
                gameObject.SetActive(true);
                target = obj.gameObject;
                originalSize = obj.transform.localScale.x;
                transform.position = target.transform.position;
                transform.rotation = target.transform.rotation;
                renderers = obj.GetComponentsInChildren<Renderer>();
                bool flag = calculateBounds(target);
                target.transform.localPosition = new Vector3(target.transform.localPosition.x, -backBottom.localPosition.z + 1, target.transform.localPosition.z);
                refresh();
                obj.SaveTransform();
            }
            else
            {
                target = null;
                gameObject.SetActive(false);
            }
        }

        bool calculateBounds(GameObject obj)
        {
            BoxCollider box = obj.GetComponent<BoxCollider>();
            if (box != null)
            {
                GameObject co = new GameObject("dummy");
                co.transform.position = obj.transform.position;
                co.transform.localScale = obj.transform.lossyScale;
                BoxCollider cobc = co.AddComponent<BoxCollider>();
                quat = obj.transform.rotation;
                cobc.center = box.center;
                cobc.size = box.size;
                Bounds bound = cobc.bounds;
                Destroy(co);
                boundDiff = bound.center - transform.position;
                boundExtents = bound.extents;
                setPoints();
                return true;
            }
            return false;
        }

        void setPoints()
        { 
            Vector3 bc = transform.position + quat * boundDiff;
            topFrontRight.position = bc + quat * Vector3.Scale(boundExtents, new Vector3(1, 1, 1));
            topFrontLeft.position = bc + quat * Vector3.Scale(boundExtents, new Vector3(-1, 1, 1));
            topBackLeft.position = bc + quat * Vector3.Scale(boundExtents, new Vector3(-1, 1, -1));
            topBackRight.position = bc + quat * Vector3.Scale(boundExtents, new Vector3(1, 1, -1));
            bottomFrontRight.position = bc + quat * Vector3.Scale(boundExtents, new Vector3(1, -1, 1));
            bottomFrontLeft.position = bc + quat * Vector3.Scale(boundExtents, new Vector3(-1, -1, 1));
            bottomBackLeft.position = bc + quat * Vector3.Scale(boundExtents, new Vector3(-1, -1, -1));
            bottomBackRight.position = bc + quat * Vector3.Scale(boundExtents, new Vector3(1, -1, -1));

            frontTop.position = bc + quat * Vector3.Scale(boundExtents, new Vector3(0, 1, 1));
            frontBottom.position = bc + quat * Vector3.Scale(boundExtents, new Vector3(0, -1, 1));
            frontLeft.position = bc + quat * Vector3.Scale(boundExtents, new Vector3(-1, 0, 1));
            frontRight.position = bc + quat * Vector3.Scale(boundExtents, new Vector3(1, 0, 1));
            midTopLeft.position = bc + quat * Vector3.Scale(boundExtents, new Vector3(-1, 1, 0));
            midBottomLeft.position = bc + quat * Vector3.Scale(boundExtents, new Vector3(-1, -1, 0));
            midTopRight.position = bc + quat * Vector3.Scale(boundExtents, new Vector3(1, 1, 0));
            midBottomRight.position = bc + quat * Vector3.Scale(boundExtents, new Vector3(1, -1, 0));
            backTop.position = bc + quat * Vector3.Scale(boundExtents, new Vector3(0, 1, -1));
            backBottom.position = bc + quat * Vector3.Scale(boundExtents, new Vector3(0, -1, -1));
            backLeft.position = bc + quat * Vector3.Scale(boundExtents, new Vector3(-1, 0, -1));
            backRight.position = bc + quat * Vector3.Scale(boundExtents, new Vector3(1, 0, -1));
            //bottomPlane.position = bc + quat * Vector3.Scale(boundExtents, new Vector3(0, 0, -1));
            //Debug.Log("bottomPlane: " + bottomPlane.localPosition);

            scalelines();
        }

        void scaleline(Transform tran, float _x1, float _x2)
        {
            float f = _x1 - _x2 - 13;
            if (f > 0)
            {
                tran.gameObject.SetActive(true);
                tran.localScale = new Vector3(1, f * 0.5f, 1);
            }
            else
                tran.gameObject.SetActive(false);
        }

        void scalelines()
        {
            scaleline(frontTop, topFrontRight.localPosition.x, topFrontLeft.localPosition.x);
            scaleline(frontBottom, bottomFrontRight.localPosition.x, bottomFrontLeft.localPosition.x);
            scaleline(frontLeft, topFrontLeft.localPosition.y, bottomFrontLeft.localPosition.y);
            scaleline(frontRight, topFrontRight.localPosition.y, bottomFrontRight.localPosition.y);
            scaleline(backTop, topBackRight.localPosition.x, topBackLeft.localPosition.x);
            scaleline(backBottom, bottomBackRight.localPosition.x, bottomBackLeft.localPosition.x);
            scaleline(backLeft, topBackLeft.localPosition.y, bottomBackLeft.localPosition.y);
            scaleline(backRight, topBackRight.localPosition.y, bottomBackRight.localPosition.y);
            scaleline(midTopLeft, topFrontLeft.localPosition.z, topBackLeft.localPosition.z);
            scaleline(midTopRight, topFrontRight.localPosition.z, topBackRight.localPosition.z);
            scaleline(midBottomLeft, bottomFrontLeft.localPosition.z, bottomBackRight.localPosition.z);
            scaleline(midBottomRight, bottomFrontRight.localPosition.z, bottomBackRight.localPosition.z);

            //float _x = topBackRight.localPosition.x - topBackLeft.localPosition.x;
            //float _y = topBackLeft.localPosition.y - bottomBackLeft.localPosition.y;
            //bottomPlane.localScale = new Vector3(_x * 0.1f, 1, _y * 0.1f);
        }

        void refresh()
        {
            transform.position = target.transform.position;
            transform.rotation = target.transform.rotation;
            if (target.transform.localScale.x != originalSize)
            {
                float rate = target.transform.localScale.x / originalSize;
                originalSize = target.transform.localScale.x;
                topFrontRight.localPosition = topFrontRight.localPosition * rate;
                topFrontLeft.localPosition = topFrontLeft.localPosition * rate;
                topBackLeft.localPosition = topBackLeft.localPosition * rate;
                topBackRight.localPosition = topBackRight.localPosition * rate;
                bottomFrontRight.localPosition = bottomFrontRight.localPosition * rate;
                bottomFrontLeft.localPosition = bottomFrontLeft.localPosition * rate;
                bottomBackLeft.localPosition = bottomBackLeft.localPosition * rate;
                bottomBackRight.localPosition = bottomBackRight.localPosition * rate;

                frontTop.localPosition = frontTop.localPosition * rate;
                frontBottom.localPosition = frontBottom.localPosition * rate;
                frontLeft.localPosition = frontLeft.localPosition * rate;
                frontRight.localPosition = frontRight.localPosition * rate;
                midTopLeft.localPosition = midTopLeft.localPosition * rate;
                midTopRight.localPosition = midTopRight.localPosition * rate;
                midBottomLeft.localPosition = midBottomLeft.localPosition * rate;
                midBottomRight.localPosition = midBottomRight.localPosition * rate;
                backTop.localPosition = backTop.localPosition * rate;
                backBottom.localPosition = backBottom.localPosition * rate;
                backLeft.localPosition = backLeft.localPosition * rate;
                backRight.localPosition = backRight.localPosition * rate;
                //bottomPlane.localPosition = bottomPlane.localPosition * rate;

                scalelines();
                target.transform.localPosition = new Vector3(target.transform.localPosition.x, -backBottom.localPosition.z + 1, target.transform.localPosition.z);
            }
        }

        void LateUpdate()
        {
            if (target != null)
                refresh();
        }
    }
}
