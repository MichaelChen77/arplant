
public class CommentedScripts
{
	#region ObjectTouchControl old version 1.0
	//		void Drag(Touch fing, int constrainted)
	//		{
	//			if (fing.phase == TouchPhase.Moved) {
	//				Vector2 fingMove = fing.deltaPosition;
	//				Vector3 deltMove = Vector3.zero;
	//				#if UNITY_ANDROID
	//				AndroidJavaClass kudanArClass = new AndroidJavaClass("eu.kudan.androidar.KudanAR");
	//				AndroidJavaObject m_KudanAR_Instance = kudanArClass.CallStatic<AndroidJavaObject>("getInstance");
	//				Quaternion orientation = new Quaternion();
	//				if (m_KudanAR_Instance != null)
	//				{
	//				m_KudanAR_Instance.Call("updateArbi", 200.0f);
	//
	//				AndroidJavaObject arbiPosition = m_KudanAR_Instance.Get<AndroidJavaObject>("m_ArbiPosition");
	//				AndroidJavaObject arbiOrientation = m_KudanAR_Instance.Get<AndroidJavaObject>("m_ArbiOrientation");
	//
	//				orientation.x = arbiOrientation.Get<float>("x");
	//				orientation.y = arbiOrientation.Get<float>("y");
	//				orientation.z = arbiOrientation.Get<float>("z");
	//				orientation.w = arbiOrientation.Get<float>("w");
	//				}
	//				Vector3 rotation = orientation.eulerAngles;
	//
	//				#endif
	//
	//				switch (constrainted) {
	//				case 0:
	//					deltMove.x = fingMove.x * moveSpeed * Time.deltaTime;
	//					deltMove.y = fingMove.y * moveSpeed * Time.deltaTime;
	//					deltMove.z = 0;
	//					break;
	//				case 1:
	//					deltMove.x = 0;
	//					deltMove.y = 0;
	//					deltMove.z = fingMove.y * moveSpeed * Time.deltaTime;
	//					break;
	//				case 2:
	//					deltMove.x = fingMove.x * moveSpeed * Time.deltaTime;
	//					deltMove.y = 0;
	//					deltMove.z = 0;
	//					break;
	//				case 3:
	//					deltMove.x = 0;
	//					deltMove.y = fingMove.y * moveSpeed * Time.deltaTime;
	//					deltMove.z = 0;
	//					break;
	//				}
	//				#if UNITY_ANDROID
	//				     deltMove = Quaternion.Euler(0, -rotation.y, 0) * deltMove;
	//				#endif
	//				
	//				//float posChangeX, posChangeY, posChangeZ;
	//				//			switch (Screen.orientation) {
	//				//			case ScreenOrientation.Portrait:
	//				//				posChangeX = this.transform.localPosition.x - deltMove.y;
	//				//				posChangeY = this.transform.localPosition.y + deltMove.x;
	//				//				break;
	//				//			case ScreenOrientation.LandscapeRight:
	//				//				posChangeX = this.transform.localPosition.x - deltMove.x;
	//				//				posChangeY = this.transform.localPosition.y - deltMove.y;
	//				//				break;
	//				//			case ScreenOrientation.PortraitUpsideDown:
	//				//				posChangeX = this.transform.localPosition.x + deltMove.y;
	//				//				posChangeY = this.transform.localPosition.y - deltMove.x;
	//				//				break;
	//				//			default: 
	//				//				posChangeX = this.transform.localPosition.x + deltMove.x;
	//				//				posChangeY = this.transform.localPosition.y + deltMove.y;
	//				//				break;
	//				//			}
	//				//			posChangeZ = this.transform.localPosition.z + deltMove.z;
	//				
	//				//Create appropriate vector, y is height in local the position
	//				//When scale larger, move larger. Scale smaller, move smaller
	//				float posChangeX = this.transform.localPosition.x + deltMove.x * transform.localPosition.z * 0.01f;
	//				float posChangeY = this.transform.localPosition.y + deltMove.y * transform.localPosition.z * 0.01f;
	//				float posChangeZ = this.transform.localPosition.z + deltMove.z * transform.localPosition.z * 0.01f;
	//				
	//				this.transform.localPosition = new Vector3 (posChangeX, posChangeY, posChangeZ);
	//			}
	//		}
	//
	//		void Rotate(Touch fing, int constrainted)
	//		{
	//			if (fing.phase == TouchPhase.Moved)
	//			{
	//				Vector2 deltaRot = fing.deltaPosition * rotSpeed * Mathf.Deg2Rad;
	//				//			Vector3 axisX, axisY;
	//				//			switch (Screen.orientation) {
	//				//			case ScreenOrientation.Portrait:
	//				//				axisX = Vector3.left;
	//				//				axisY = Vector3.up;
	//				//				break;
	//				//			case ScreenOrientation.LandscapeRight:
	//				//				axisX = Vector3.down;
	//				//				axisY = Vector3.left;
	//				//				break;
	//				//			case ScreenOrientation.PortraitUpsideDown:
	//				//				axisX = Vector3.right;
	//				//				axisY = Vector3.down;
	//				//				break;
	//				//			default:
	//				//				axisX = Vector3.up;
	//				//				axisY = Vector3.right;
	//				//				break;
	//				//			}
	//				Vector3 axisX = Vector3.up;
	//				Vector3 axisY = Vector3.right;
	//				switch (constrainted)
	//				{
	//				case 0:
	//					deltaRot = deltaRot * Time.deltaTime;
	//					transform.RotateAround(axisX, -deltaRot.x);
	//					transform.RotateAround(axisY, deltaRot.y);
	//					break;
	//				case 1:
	//					if (resetRot)
	//					{
	//						transform.localRotation = originalRot;
	//						//transform.localEulerAngles = new Vector3 (-90, transform.localEulerAngles.y, transform.localEulerAngles.z);
	//						resetRot = false;
	//					}
	//					else {
	//						if (inverseXY)
	//							transform.Rotate(0, 0, -deltaRot.x);
	//						else
	//							transform.Rotate(0, -deltaRot.x, 0);
	//					}
	//					break;
	//				case 2:
	//					if (inverseXY)
	//						transform.Rotate(0, -deltaRot.y, 0);
	//					else
	//						transform.Rotate(0, 0, -deltaRot.y);
	//					break;
	//				case 3:
	//					transform.Rotate(deltaRot.y, 0, 0, Space.World);
	//					break;
	//				}
	//				//Debug.Log ("#rotation: " + transform.rotation.eulerAngles+" delta: "+deltaRot+" final: "+deltaRot*Time.deltaTime+ " ; "+constrainted);
	//			}
	//		}
	//
	//		bool resetRot = false;
	//		void TouchModeUpdate()
	//		{
	//			if (!ResourceManager.Singleton.touchMove && ResourceManager.Singleton.constraintID != prevTrouchConstraint)
	//			{
	//				prevTrouchConstraint = ResourceManager.Singleton.constraintID;
	//				if (prevTrouchConstraint == 1)
	//				{
	//					resetRot = true;
	//				}
	//			}
	//		}
	//
	//		void processTap()
	//		{
	//			if (Input.touchCount == 1) //if there is a touch
	//			{
	//				touchDuration += Time.deltaTime;
	//				touch = Input.GetTouch(0);
	//
	//				if (touch.phase == TouchPhase.Ended && touchDuration < 0.2f) //making sure it only check the touch once && it was a short touch/tap and not a dragging.
	//					StartCoroutine("singleOrDouble");
	//			}
	//			else
	//				touchDuration = 0.0f;
	//		}
	//
	//		IEnumerator singleOrDouble()
	//		{
	//			yield return new WaitForSeconds(0.3f);
	//			if (touch.tapCount == 1)
	//				Debug.Log("Single");
	//			else if (touch.tapCount == 2)
	//			{
	//				//this coroutine has been called twice. We should stop the next one here otherwise we get two double tap
	//				StopCoroutine("singleOrDouble");
	//				Debug.Log("Double");
	//			}
	//		}
	#endregion
}

