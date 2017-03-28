using UnityEngine;
using System.Collections;

public class TestTouch : MonoBehaviour {
	public float rotSpeed = 4f;
	public float moveSpeed = 8f;
	public bool touchMove = true;
	public int constraint = 0;

	int prevTrouchConstraint = 0;
	float orignalSize = 1f;
	Quaternion originalRot;
	// Use this for initialization
	void Start () {
		originalRot = transform.localRotation;
		orignalSize = this.transform.localScale.x;
	}
	
	// Update is called once per frame
	void Update () {
		TouchModeUpdate ();
		if (Input.touchCount == 1) {
			//Store input
			Touch fing = Input.GetTouch (0);
			if (touchMove)
				Drag (fing, constraint);
			else
				Rotate (fing, constraint);
		}
		processZoom ();
	}

	void Drag(Touch fing, int constrainted)
	{
		if (fing.phase == TouchPhase.Moved) {
			Vector2 fingMove = fing.deltaPosition;
			Vector3 deltMove = Vector3.zero;

			switch (constrainted) {
			case 0:
				deltMove.x = fingMove.x * moveSpeed * Time.deltaTime;
				deltMove.y = fingMove.y * moveSpeed * Time.deltaTime;
				deltMove.z = 0;
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
			float positionChangeX = this.transform.localPosition.x + deltMove.x*transform.position.z*0.01f;
			float positionChangeY = this.transform.localPosition.y + deltMove.y*transform.position.z*0.01f;
			float positionChangeZ = this.transform.localPosition.z + deltMove.z*transform.position.z*0.01f;

			this.transform.localPosition = new Vector3 (positionChangeX, positionChangeY, positionChangeZ);
		}
	}

	void Rotate(Touch fing, int constrainted)
	{
		if (fing.phase == TouchPhase.Moved) {
			Vector2 deltaRot = fing.deltaPosition * rotSpeed * Mathf.Deg2Rad;
			switch (constrainted) {
			case 0: 
				deltaRot = deltaRot * Time.deltaTime;
				transform.RotateAround (Vector3.up, -deltaRot.x);
				transform.RotateAround (Vector3.right, deltaRot.y);
				break;
			case 1:
				if (resetRot) {
					transform.localRotation = originalRot;
					//transform.localEulerAngles = new Vector3 (-90, transform.localEulerAngles.y, transform.localEulerAngles.z);
					resetRot = false;
				} else
					transform.Rotate(0, -deltaRot.x, 0);
				transform.Rotate (0, 0, -deltaRot.x);
				break;
			case 2:
				transform.Rotate (0, 0, -deltaRot.y);
				transform.Rotate(0, -deltaRot.y, 0);
				//transform.Rotate (deltaRot.y, 0, 0);
				break;
			case 3:
				transform.Rotate (deltaRot.y, 0, 0);
				//transform.Rotate(0, -deltaRot.x, 0);
				break;
			}
			//Debug.Log ("rotation: " + transform.rotation.eulerAngles+" delta: "+deltaRot+" final: "+deltaRot*Time.deltaTime);
		}
	}

	void processZoom ()
	{
		if (Input.touchCount == 2)
		{
			//Store inputs
			Touch fing1 = Input.GetTouch (0);
			Touch fing2 = Input.GetTouch (1);

			if (fing1.phase == TouchPhase.Moved || fing2.phase == TouchPhase.Moved)	//If either finger has moved since the last frame
			{
				//Get previous positions
				Vector2 fing1Prev = fing1.position - fing1.deltaPosition;
				Vector2 fing2Prev = fing2.position - fing2.deltaPosition;

				//Find vector magnitude between touches in each frame
				float prevTouchDeltaMag = (fing1Prev - fing2Prev).magnitude;		
				float touchDeltaMag = (fing1.position - fing2.position).magnitude;

				//Find difference in distances
				float deltaDistance = prevTouchDeltaMag - touchDeltaMag;

				//Create appropriate vector
				float scaleChange = this.transform.localScale.x - this.transform.localScale.x * deltaDistance * 0.5f / 500;

				//To avoid the scale being negative
				if(scaleChange < 0){
					scaleChange = 0;
				}

				//Scale object
				this.transform.localScale = new Vector3 (scaleChange, scaleChange, scaleChange);
			}
		}
	}

	bool resetRot = false;
	void TouchModeUpdate()
	{
		if (!touchMove && constraint != prevTrouchConstraint) {
			prevTrouchConstraint = constraint;
			if (prevTrouchConstraint == 1) {
				resetRot = true;
			}
		}
	}
}
