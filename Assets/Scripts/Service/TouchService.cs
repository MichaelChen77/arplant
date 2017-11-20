using IMAV.Controller;
using UnityEngine;

namespace IMAV.Service
{
    public abstract class TouchService
    {
        protected ProductController controller;
        protected Vector2 offset;
        protected float touchTime = 0;
        protected bool beginTouch = false;

        protected abstract void MoveTouch(Vector3 pos);
        public abstract void Start();

        public TouchService(ProductController ctrl)
        {
            controller = ctrl;
        }

        protected virtual void processRotate(Touch fing1, Touch fing2)
        {
            if (fing1.phase == TouchPhase.Moved && fing2.phase == TouchPhase.Moved)
            {
                float deltaRot = fing1.deltaPosition.x * DataUtility.ProductRotateSpeed * Mathf.Deg2Rad * Time.deltaTime;
                controller.transform.Rotate(0, -deltaRot, 0);
            }
        }

        protected virtual void StationaryTouch()
        {
            SceneController.Singleton.ChooseProduct(!controller.Choosed);
        }

        public virtual void Update()
        {
            if (Input.touchCount == 1)
            {
                var touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began)
                {
                    beginTouch = true;
                    touchTime = 0;
                    Vector3 pos = Camera.main.WorldToScreenPoint(controller.transform.position);
                    offset = new Vector2(pos.x - touch.position.x, pos.y - touch.position.y);
                }
                if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved)
                {
                    MoveTouch(touch.position + offset);
                }
                else if (beginTouch && touch.phase == TouchPhase.Stationary)
                {
                    touchTime += touch.deltaTime;
                    if (touchTime > DataUtility.ProductStationaryTouchTimeThreshold)
                    {
                        StationaryTouch();
                        beginTouch = false;
                    }
                }
            }
            else if (Input.touchCount == 2)
            {
                processRotate(Input.GetTouch(0), Input.GetTouch(1));
            }
        }
    }
}
