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
        public Transform bottomPlane;

        private Vector3 boundDiff;
        private Vector3 boundExtents;
        private Quaternion quat;
        private float originalSize;
        GameObject target;
        private Renderer[] renderers;

        public void SetObject(GameObject obj)
        {
            target = obj;
            if (obj != null)
            {
                transform.position = target.transform.position;
                transform.rotation = target.transform.rotation;
                renderers = obj.GetComponentsInChildren<Renderer>();
                bool flag = calculateBounds(target);
                gameObject.SetActive(flag);
            }
            else
                gameObject.SetActive(false);
        }

        bool calculateBounds(GameObject obj)
        {
            BoxCollider box = obj.GetComponent<BoxCollider>();
            if (box != null)
            {
                originalSize = obj.transform.localScale.x;
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
            bottomPlane.position = bc + quat * Vector3.Scale(boundExtents, new Vector3(0, 0, -1));

            scalelines();
        }

        void scaleline(Transform tran, float _x)
        {
            if (_x > 7)
            {
                tran.gameObject.SetActive(true);
                tran.localScale = new Vector3(1, _x - 7, 1);
            }
            else
                tran.gameObject.SetActive(false);
        }

        void scalelines()
        {
            scaleline(frontTop, topFrontRight.localPosition.x);
            scaleline(frontBottom, topFrontRight.localPosition.x);
            scaleline(frontLeft, topFrontRight.localPosition.y);
            scaleline(frontRight, topFrontRight.localPosition.y);
            scaleline(backTop, topBackRight.localPosition.x);
            scaleline(backBottom, topBackRight.localPosition.x);
            scaleline(backLeft, topBackRight.localPosition.y);
            scaleline(backRight, topBackRight.localPosition.y);
            scaleline(midTopLeft, topFrontRight.localPosition.z);
            scaleline(midTopRight, topFrontRight.localPosition.z);
            scaleline(midBottomLeft, topFrontRight.localPosition.z);
            scaleline(midBottomRight, topFrontRight.localPosition.z);

            bottomPlane.localScale = new Vector3(topBackRight.localPosition.x, 1, topBackRight.localPosition.y);
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
                bottomPlane.localPosition = bottomPlane.localPosition * rate;

                scalelines();
            }
        }

        void LateUpdate()
        {
            if (isActiveAndEnabled)
                refresh();
        }
    }
}
