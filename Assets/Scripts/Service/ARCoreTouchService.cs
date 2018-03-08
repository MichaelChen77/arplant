using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IMAV.Controller;
using GoogleARCore;

namespace IMAV.Service
{
    public class ARCoreTouchService : TouchService
    {
        private TrackedPlane m_AttachedPlane;
        private float m_planeYOffset;

        public ARCoreTouchService(ProductController ctrl): base(ctrl)
        {

        }

        public override void Start()
        {
            MoveTouch(new Vector3(Screen.width*0.5f, Screen.height*0.5f, 0));
            controller.transform.LookAt(Camera.main.transform);
            controller.transform.rotation = Quaternion.Euler(0.0f, controller.transform.rotation.eulerAngles.y, controller.transform.rotation.z);
        }

        protected override void MoveTouch(Vector3 pos)
        {
            TrackableHit hit;
            TrackableHitFlag raycastFilter = TrackableHitFlag.PlaneWithinPolygon;
                //| TrackableHitFlag.PlaneWithinInfinity;

            if (Session.Raycast(Camera.main.ScreenPointToRay(pos), raycastFilter, out hit))
            {
                Debug.Log("Hit: " + hit.Point);

                // Create an anchor to allow ARCore to track the hitpoint as understanding of the physical
                // world evolves.
                var anchor = Session.CreateAnchor(hit.Point, Quaternion.identity);

                controller.transform.position = hit.Point;
                controller.transform.parent = anchor.transform;
                Attach(hit.Plane);
            }
        }

        public override void Update()
        {
            while (m_AttachedPlane.SubsumedBy != null)
            {
                m_AttachedPlane = m_AttachedPlane.SubsumedBy;
            }
            base.Update();
            controller.transform.position = new Vector3(controller.transform.position.x, m_AttachedPlane.Position.y + m_planeYOffset, controller.transform.position.z);
        }

        public void Attach(TrackedPlane plane)
        {
            m_AttachedPlane = plane;
            m_planeYOffset = controller.transform.position.y - plane.Position.y;
        }

        public override string ToString()
        {
            return controller.name + " " + m_AttachedPlane.Position.y + " , " + m_AttachedPlane.IsUpdated;
        }
    }
}
