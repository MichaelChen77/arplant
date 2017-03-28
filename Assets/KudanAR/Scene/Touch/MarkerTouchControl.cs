using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

/// <summary>
/// Class that takes touch input and uses it to rotate and scale objects and activate a script.
/// </summary>
public class MarkerTouchControl : MonoBehaviour 
{
	#if UNITY_ANDROID || UNITY_IOS
	/// <summary>
	/// The rate at which the pinch control scales the object.
	/// </summary>
	float zoomSpeed;

	/// <summary>
	/// The rate at which the swipe control rotates the object.
	/// </summary>
	float moveSpeed;

	/// <summary>
	/// The amount that the finger can move on the screen before it is considered to be a movement and not a tap.
	/// </summary>
	float roughDiff;

	/// <summary>
	/// Save the orignalSize.
	/// </summary>
	float orignalSize;

	/// <summary>
	/// The time duration that a finger touched on the screen.
	/// </summary>
	float touchDuration;

	/// <summary>
	/// The touch of finger.
	/// </summary>
	Touch touch;

	/// <summary>
	/// The script that activates when the sreen is tapped.
	/// </summary>
	Activated1Script activate1;

	/// <summary>
	/// The script that activates when the sreen is tapped.
	/// </summary>
	Activated2Script activate2;

	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start()
	{
		zoomSpeed = 0.5f;
		moveSpeed = 60f;
		roughDiff = 3f;
		orignalSize = this.transform.localScale.x;

		activate1 = this.GetComponent<Activated1Script>();
		activate1.enabled = false;
		activate2 = this.GetComponent<Activated2Script>();
		activate2.enabled = false;
	}

	/// <summary>
	/// Update this instance.
	/// </summary>
	void Update()
	{
		processDrag ();
		processZoom ();
		processTap ();
	}

	/// <summary>
	/// Checks for pinch controls.
	/// </summary>
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
				float scaleChange = this.transform.localScale.x - this.transform.localScale.x * deltaDistance * zoomSpeed / 500;

				//To avoid the scale being negative
				if(scaleChange < 0){
					scaleChange = 0;
				}

				//Scale object
				this.transform.localScale = new Vector3 (scaleChange, scaleChange, scaleChange);
			}
		}
	}

	/// <summary>
	/// Checks for drag controls.
	/// </summary>
	void processDrag()
	{
		if (Input.touchCount == 1) 
		{
			//Store input
			Touch fing = Input.GetTouch (0);

			if(fing.phase == TouchPhase.Moved)	//If the finger has moved since the last frame
			{
				//Find the amount the finger has moved, and apply a transformation to this gameobject based on that amount
				Vector2 fingMove = fing.deltaPosition;
				Vector3 deltMove;
				deltMove.x = fingMove.x * moveSpeed * Time.deltaTime;
				deltMove.y = fingMove.y * moveSpeed * Time.deltaTime;
				deltMove.z = 0;

				//Create appropriate vector, y is height in local the position
				//When scale larger, move larger. Scale smaller, move smaller
				float positionChangeX = this.transform.localPosition.x - this.transform.localScale.x * deltMove.z / orignalSize;
				float positionChangeY = this.transform.localPosition.y + this.transform.localScale.x * deltMove.y / orignalSize;
				float positionChangeZ = this.transform.localPosition.z - this.transform.localScale.x * deltMove.x / orignalSize;

				this.transform.localPosition = new Vector3 (positionChangeX, positionChangeY, positionChangeZ);
			}
		}
		if (Input.touchCount == 2) {
			//Store inputs
			Touch fing1 = Input.GetTouch (0);
			Touch fing2 = Input.GetTouch (1);

			if (fing1.phase == TouchPhase.Moved || fing2.phase == TouchPhase.Moved) {	//If either finger has moved since the last frame
				//Get previous positions
				Vector2 fing1Prev = fing1.position - fing1.deltaPosition;
				Vector2 fing2Prev = fing2.position - fing2.deltaPosition;

				//Find vector magnitude between touches in each frame
				float prevTouchDeltaMag = (fing1Prev - fing2Prev).magnitude;		
				float touchDeltaMag = (fing1.position - fing2.position).magnitude;

				//Find difference in distances
				float deltaDistance = prevTouchDeltaMag - touchDeltaMag;

				if (deltaDistance < 2 * roughDiff) {
					//translate object
					Vector2 fingMove = fing1.deltaPosition;
					Vector3 deltMove;

					deltMove.x = fingMove.y * moveSpeed * Time.deltaTime;
					deltMove.y = 0;
					deltMove.z = 0;

					//Create appropriate vector, y is height in local the position
					//When scale larger, move larger. Scale smaller, move smaller
					float positionChangeX = this.transform.localPosition.x + this.transform.localScale.x * deltMove.x / orignalSize;
					float positionChangeY = this.transform.localPosition.y + this.transform.localScale.x * deltMove.y / orignalSize;
					float positionChangeZ = this.transform.localPosition.z + this.transform.localScale.x * deltMove.z / orignalSize;

					this.transform.localPosition = new Vector3 (positionChangeX, positionChangeY, positionChangeZ);
				}
			}
		}
	}

	/// <summary>
	/// Checks for tap controls.
	/// </summary>
	void processTap()
	{
		if(Input.touchCount == 1) //if there is a touch
		{
			touchDuration += Time.deltaTime;
			touch = Input.GetTouch (0) ;

			if(touch.phase == TouchPhase.Ended && touchDuration < 0.2f) //making sure it only check the touch once && it was a short touch/tap and not a dragging.
				StartCoroutine("singleOrDouble");
		}
		else
			touchDuration = 0.0f;
	}

	IEnumerator singleOrDouble(){
		yield return new WaitForSeconds(0.3f);
		if(touch.tapCount == 1)
			Debug.Log ("Single");
		else if(touch.tapCount == 2){
			//this coroutine has been called twice. We should stop the next one here otherwise we get two double tap
			StopCoroutine("singleOrDouble");
			Debug.Log ("Double");
			activate1.enabled = !activate1.enabled;
		}
	}
	#endif
}