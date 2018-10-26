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

		//test 
		private Touch oldTouch1;
		private Touch oldTouch2;
		private double smallestScale = 0.3;
		private double largestScale = 2.0;

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

		protected virtual void processZoomInOut(Touch fing1, Touch fing2)
		{
			if (fing1.phase == TouchPhase.Moved || fing2.phase == TouchPhase.Moved)
			{
				float oldDistance = Vector2.Distance(fing1.position, fing2.position);
				float newDistance = Vector2.Distance(fing1.position + fing1.deltaPosition, fing2.position + fing2.deltaPosition);

				//两个距离之差，为正表示放大手势， 为负表示缩小手势
				float offset = newDistance - oldDistance;

				//放大因子， 一个像素按 0.01倍来算(500可调整)
				float scaleFactor = offset / 500f;
				Vector3 localScale = controller.transform.localScale;
				Vector3 scale = new Vector3(localScale.x + scaleFactor * localScale.x,
					localScale.y + scaleFactor * localScale.y, 
					localScale.z + scaleFactor * localScale.z);

				controller.transform.localScale = scale;
//				最小缩放到 0.3 倍, 放大到 2 倍
//				if (scale.x > smallestScale && scale.y > smallestScale && scale.z > smallestScale) {
//					controller.transform.localScale = scale;
//				}
//				if (scale.x >= smallestScale * localScale.x && scale.x <= largestScale * localScale.x
//				    && scale.y >= smallestScale * localScale.y && scale.y <= largestScale * localScale.y
//				    && scale.z >= smallestScale * localScale.z && scale.z <= largestScale * localScale.z) {
//					controller.transform.localScale = scale;
//				} else if ((scale.x - smallestScale) < (largestScale - scale.x)) {
//					controller.transform.localScale = smallestScale;
//				} else {
//					controller.transform.localScale = largestScale;
//				}

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
				var newTouch1 = Input.GetTouch(0);
				var newTouch2 = Input.GetTouch(1);
//                processRotate(Input.GetTouch(0), Input.GetTouch(1));
				int caseIndex = -1;
				float value = Vector2.Dot(newTouch1.deltaPosition, newTouch2.deltaPosition);
				//value >= 0, rotate, value < 0, zoom in/out
				if (value >= 0) {
					caseIndex = 0;
					processRotate (newTouch1, newTouch2);
				} else {
					caseIndex = 1;
					processZoomInOut (newTouch1, newTouch2);
				}
//				switch (caseIndex) {
//				case 0:
//					processRotate (newTouch1, newTouch2);
//					break;
//				case 1:
//					processZoomInOut (newTouch1, newTouch2);
//					break;
//				}
            }
        }
    }
}
