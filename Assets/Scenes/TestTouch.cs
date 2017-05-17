using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using IMAV;

public class TestTouch : MonoBehaviour {
	public float rotSpeed = 4f;
	public float moveSpeed = 8f;
	public float zoomSpeed = 8f;
	public bool touchMove = true;
	public int constraint = 0;

	Quaternion originalRot;

	ObjectTouchControl currentTc;
	// Use this for initialization
	void Start () {
		originalRot = transform.localRotation;
	}
	
	// Update is called once per frame
	void Update () {
		CheckTouch ();
//		TouchModeUpdate ();
//		if (Input.touchCount == 1) {
//			//Store input
//			Touch fing = Input.GetTouch (0);
//			Drag (fing, constraint);
//		}
//		processZoomAndRotate ();
	}

	void CheckTouch()
	{
		if (Input.touchCount > 0) {
			foreach (Touch touch in Input.touches) {
				int id = touch.fingerId;
				if (EventSystem.current.IsPointerOverGameObject (id)) {
					if (currentTc != null)
						currentTc.Selected = SelectState.Pause;
					return;
				}
			}

			if (Input.GetTouch (0).phase == TouchPhase.Began) {
				Ray ray = Camera.main.ScreenPointToRay (Input.GetTouch (0).position);
				RaycastHit hit;
				if (Physics.Raycast (ray, out hit)) {
					GameObject touchedObject = hit.transform.gameObject;
					if (touchedObject != null) {
						currentTc = touchedObject.GetComponent<ObjectTouchControl> ();
						currentTc.Selected = SelectState.Actived;
					}
					Debug.Log ("Touched " + touchedObject.transform.name);
				}
			}
		}
	}

	void click(Touch fing)
	{
		Vector3 _pos = new Vector3 (fing.position.x, fing.position.y, 250);
		Vector3 pos1 = Camera.main.ScreenToWorldPoint (_pos);
		pos1.z = transform.position.z;
		transform.position = pos1;
	}

	void Drag(Touch fing, int constrainted)
	{
		if (fing.phase == TouchPhase.Moved && fing.deltaPosition.sqrMagnitude > 1f) {
			Vector2 fingMove = fing.deltaPosition;
			Vector3 deltMove = Vector3.zero;

			switch (constrainted) {
			case 0:
				deltMove.x = fingMove.x * moveSpeed * Time.deltaTime;
				deltMove.y = fingMove.y * moveSpeed * Time.deltaTime;
				deltMove.z = fingMove.y * moveSpeed * Time.deltaTime;
				break;
			case 1:
				deltMove.x = 0;
				deltMove.y = 0;
				deltMove.z = fingMove.y * moveSpeed * Time.deltaTime;
				break;
			case 2:
				deltMove.x = fingMove.x * moveSpeed * Time.deltaTime;
				deltMove.y = 0;
				deltMove.z = 0;
				break;
			case 3:
				deltMove.x = 0;
				deltMove.y = fingMove.y * moveSpeed * Time.deltaTime;
				deltMove.z = 0;
				break;
			}
			//deltMove = Quaternion.Euler (0, -rotation.y, 0) * deltMove;

			//Create appropriate vector, y is height in local the position
			//When scale larger, move larger. Scale smaller, move smaller
			float positionChangeX = this.transform.localPosition.x + deltMove.x*transform.localScale.z*0.01f;
			float positionChangeY = this.transform.localPosition.y + deltMove.y*transform.localScale.z*0.01f;
			float positionChangeZ = this.transform.position.z + deltMove.z * transform.position.z * 0.01f;

			//this.transform.localPosition = new Vector3 (positionChangeX, positionChangeY, positionChangeZ);
			Vector3 _pos = new Vector3 (fing.position.x, fing.position.y, positionChangeZ);
			Vector3 pos1 = Camera.main.ScreenToWorldPoint (_pos);
			//Vector3 pos1 = new Vector3(fing.position.x - Screen.width*0.5f, fing.position.y - Screen.height*0.5f, transform.position.z);
			pos1.z = positionChangeZ;
			//		pos1.x = pos1.x * transform.localScale.x*0.5f;
			//		pos1.y = pos1.y * transform.localScale.y*0.5f;
			transform.position = pos1;

			Debug.Log ("pos: " + transform.localPosition + " ; " + fing.position + " ; " + fing.rawPosition + " ; " + Camera.main.ScreenToWorldPoint (_pos) + " ; " + Camera.main.ScreenToViewportPoint (_pos));

		}
	}

	void processZoomAndRotate ()
	{
		if (Input.touchCount == 2) {
			//Store inputs
			Touch fing1 = Input.GetTouch (0);
			Touch fing2 = Input.GetTouch (1);
			if (fing1.phase == TouchPhase.Moved && fing2.phase == TouchPhase.Moved) {
				float delta1 = fing1.deltaPosition.sqrMagnitude;
				float delta2 = fing2.deltaPosition.sqrMagnitude;
				Debug.Log ("delta: " + delta1 + " ; " + delta2 +" ; "+fing1.deltaPosition+" ; "+fing2.deltaPosition);
				if (delta1 > 1f && delta2 > 1f) {	//If either finger has moved since the last frame
					//Get previous positions
					Vector2 fing1Prev = fing1.position - fing1.deltaPosition;
					Vector2 fing2Prev = fing2.position - fing2.deltaPosition;

					//Find vector magnitude between touches in each frame
					float prevTouchDeltaMag = (fing1Prev - fing2Prev).magnitude;		
					float touchDeltaMag = (fing1.position - fing2.position).magnitude;

					//Find difference in distances
					float deltaDistance = prevTouchDeltaMag - touchDeltaMag;

					//Create appropriate vector
					float scaleChange = this.transform.localScale.x - this.transform.localScale.x * deltaDistance * zoomSpeed / 1000;

					//To avoid the scale being negative
					if (scaleChange < 1) {
						scaleChange = 1;
					}
					this.transform.localScale = new Vector3 (scaleChange, scaleChange, scaleChange);
				} else {
					if (delta1 > 1f)
						TouchRotate (fing1);
					else
						TouchRotate (fing2);
				}
			} else if (fing1.phase == TouchPhase.Moved || fing2.phase == TouchPhase.Moved) {
				if (fing1.phase != TouchPhase.Moved)
					TouchRotate (fing2);
				else
					TouchRotate (fing1);
			}
		}
	}

	void TouchRotate(Touch fing)
	{
		float deltaRot = fing.deltaPosition.sqrMagnitude * rotSpeed * Mathf.Deg2Rad * Time.deltaTime;
		transform.Rotate (0, -deltaRot, 0);
	}
}
