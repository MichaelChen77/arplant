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
            }
        }

        void LateUpdate()
        {
            if (isActiveAndEnabled)
                refresh();
        }
    }
}
