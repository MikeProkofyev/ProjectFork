using UnityEngine;
using System.Collections;

[RequireComponent (typeof(MovementController))]
public class InputHandler : MonoBehaviour {

	MovementController movementController;

	Vector2 touchStartPosition;
	float longTouchDuration = .5f;
	float touchStartTime, rButtonStartTime, lButtonStartTime = Mathf.Infinity;

	// Use this for initialization
	void Awake () {
		movementController = GetComponent <MovementController> ();
		touchStartTime = rButtonStartTime = lButtonStartTime = Mathf.Infinity;
	}
		
	
	// Update is called once per frame
	void Update () {
		#if UNITY_IPHONE
		if (Application.isMobilePlatform) {
			HandleTouches ();
		}
		#endif

		#if UNITY_EDITOR_OSX
		HandleKeys ();
		#endif
	}

	void HandleTouches ()	{
		if (Input.touchCount > 0) {
			Touch touch = Input.touches[0];
			switch (touch.phase) {
			case TouchPhase.Began:
				touchStartPosition = touch.position;
				touchStartTime = Time.time;
				break;
			case TouchPhase.Ended:

				if (touchStartTime + longTouchDuration < Time.time) {
					touchStartTime = Mathf.Infinity;
					HandleLongTap (touchStartPosition);
				}else {
					float swipeHorizontal = touch.position.x - touchStartPosition.x;
					float swipeVertical = touch.position.y - touchStartPosition.y;
					//HANDLE SWIPES
					if (Mathf.Abs(swipeHorizontal) > 200) {
						int shift = swipeHorizontal > 0 ? 1 : -1;
						movementController.HandleHorzontalInput(shift);
					} else if(Mathf.Abs(swipeVertical) > 200) {
						bool increaseSpeed = swipeVertical > 0 ? true : false;
						movementController.changeSpeed(increaseSpeed);
					} else { //HANDLE TAP 
						HandleTap (touchStartPosition);
					}	
				}
				break;
			}

		}
	}

	void HandleLongTap (Vector2 touchPosition) {
		if (Screen.width/2 < touchPosition.x) {
			movementController.HandleTurnLeft(false);
		} else {
			movementController.HandleTurnLeft(true);
		}
	}

	void HandleTap (Vector2 tapPosition) {
		if (Screen.width/2 < tapPosition.x) {
			movementController.HandleHorzontalInput(1);
		} else {
			movementController.HandleHorzontalInput(-1);
		}
	}

	void HandleKeys () {
		if (Input.GetKey("right")) {
			lButtonStartTime = Mathf.Infinity;
			if (rButtonStartTime == Mathf.Infinity) rButtonStartTime = Time.time;
		} else if (Input.GetKey("left")) {
			rButtonStartTime = Mathf.Infinity;
			if (lButtonStartTime == Mathf.Infinity) lButtonStartTime = Time.time;
		} else {
			if (rButtonStartTime < Mathf.Infinity) {
				bool pressedLong = rButtonStartTime + longTouchDuration < Time.time;
				if (pressedLong) {
					Debug.Log(Time.time - rButtonStartTime);
					movementController.HandleTurnLeft(false);
				}else {
					movementController.HandleHorzontalInput(1);
				}
				rButtonStartTime = Mathf.Infinity;
			} else if(lButtonStartTime < Mathf.Infinity){
				bool pressedLong = lButtonStartTime + longTouchDuration < Time.time;
				if (pressedLong) {
					Debug.Log(Time.time - lButtonStartTime);
					movementController.HandleTurnLeft(true);
				}else {
					movementController.HandleHorzontalInput(-1);
				}
				lButtonStartTime = Mathf.Infinity;	
			}
		}
			


		if (Input.GetKeyDown("up")) {
			movementController.changeSpeed(true);
		} else if(Input.GetKeyDown("down")) {
			movementController.Reverse();
		}
	}
}