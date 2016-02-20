using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MovementController : MonoBehaviour {

	float speed = 6f;
	float defaultDistanceBtwCP = 3f;
	float currDistanceBtwCP = 3f;

	float cPointsProgress = 0;
	public CarModelController carModelController;
	public Transform currCPointT;
	CPointController currCPointController;
	public Transform nextCPointT;
	CPointController nextCPointController;
	Transform nextTurnCPointT;
	bool awaitingTurn;
	float shift = 0;
	int nextShift = 0;
	int awaitingTurnDirection = 0;
	public int direction = 1;
	bool betweenDirections = false;

	void Start () {
		SwitchToCPoint(currCPointT);
	}

	void Update () {

		if (nextCPointT == null) {
			enabled = false;
			return;
		}
		shift = Mathf.Lerp (shift, nextShift, Time.deltaTime * 3 * speed);
		Vector3 startPoint = new Vector3 (currCPointT.position.x, currCPointT.position.y + 0.1f, currCPointT.position.z);
		if (betweenDirections) {
			startPoint += currCPointT.right * -1 * shift  * 2;
		}else {
			startPoint += currCPointT.right * shift  * 2;
		}

		Vector3 endPoint = new Vector3 (nextCPointT.position.x, nextCPointT.position.y + 0.1f, nextCPointT.position.z)  + nextCPointT.right * shift * 2;

		transform.position = Vector3.Lerp(startPoint, endPoint, cPointsProgress);


		Quaternion nextRotation = Quaternion.Euler(nextCPointT.rotation.eulerAngles.x, nextCPointT.rotation.eulerAngles.y, 0);
		Quaternion prevRotation = Quaternion.Euler(currCPointT.rotation.eulerAngles.x, currCPointT.rotation.eulerAngles.y, 0);
		if (direction == -1) {
			if (!betweenDirections) {
				prevRotation *= Quaternion.Euler(Vector3.up * 180);		
			}
			nextRotation *= Quaternion.Euler(Vector3.up * 180);	
		}	
		transform.rotation = Quaternion.Lerp(prevRotation, nextRotation, cPointsProgress);

		float speedModifier = currDistanceBtwCP / defaultDistanceBtwCP;
		cPointsProgress += Time.deltaTime * speed / speedModifier;

		if(cPointsProgress > 1)
		{
			cPointsProgress = 0;

			SwitchToCPointV2(nextCPointT);
		}
	}

	void  SwitchToCPointV2 (Transform cPoint) {
		currCPointT = cPoint;
		currCPointController = cPoint.GetComponent<CPointController>();
		betweenDirections = false;

		if (direction == 1) {
			nextCPointT = currCPointT.GetComponent<CPointController>().pointAhead;
		}else {
			nextCPointT = currCPointT.GetComponent<CPointController>().invPointAhead;
		}
		nextCPointController = nextCPointT.GetComponent<CPointController>();


		if (awaitingTurn && nextCPointController.pointLeft != nextTurnCPointT) {
			awaitingTurn = false;
			nextCPointT = nextTurnCPointT;
			nextCPointController = nextTurnCPointT.GetComponent<CPointController>();
			nextShift = ShiftAfterTurn ();
			CheckTurnDirection(awaitingTurnDirection);
		}else {
			bool betweenSectors = currCPointT.parent != nextCPointT.parent;
			if (betweenSectors) {
				CheckNextSegmentDirection();	
			}
		}
		currDistanceBtwCP = Vector3.Distance(currCPointT.position, nextCPointT.position);
	}

	void CheckNextSegmentDirection () {
		float dotProduct = Vector3.Dot(currCPointT.TransformDirection(Vector3.forward*direction), nextCPointT.TransformDirection(Vector3.forward));
		bool sameDirWithCP = dotProduct > 0;
		int newDirection = sameDirWithCP ? 1 : -1;
		if (newDirection != direction) { //Need to change the shift to the opposite one, if directions don't match
			shift *= -1;
			nextShift *= -1;
			betweenDirections = true;
		}
		direction = newDirection;
	}

	void CheckTurnDirection (int turnDirection) {
		float dotProduct = Vector3.Dot(currCPointT.TransformDirection(Vector3.right * turnDirection), nextCPointT.TransformDirection(Vector3.forward));
		Debug.Log("Turn direction: " + turnDirection + " " +"Dot product: " + dotProduct);
		bool sameDirWithCP = dotProduct > 0;
		int newDirection = sameDirWithCP ? 1 : -1;
		if (newDirection != direction) {
			betweenDirections = true;
		}
		direction = newDirection;
//		Debug.Log("New direction: " +  direction);
	}

	public void TryTurnLeft (bool turnLeft) {
		if (direction == -1) turnLeft = !turnLeft;
//		if (turnLeft) {
//			Debug.Log("Turning left: " + currCPointController.pointLeft);	
//			Debug.Log("Direction: " + direction);
//		}else {
//			Debug.Log("Turning right: " + currCPointController.pointRight);	
//			Debug.Log("Direction: " + direction);
//		}

		if (turnLeft && currCPointController.pointLeft != null) {
			awaitingTurn = true;
			nextTurnCPointT = currCPointController.pointLeft;
			awaitingTurnDirection = -1;
		}else if(!turnLeft && currCPointController.pointRight != null) {
			awaitingTurn = true;
			nextTurnCPointT = currCPointController.pointRight;
			awaitingTurnDirection = 1;
		} 
	}

	public void Reverse () {
		direction *= -1;
		cPointsProgress = 1 - cPointsProgress;
		Transform swapTempT = currCPointT;
		currCPointT = nextCPointT;
		currCPointController = nextCPointController.GetComponent<CPointController>();
		nextCPointT = swapTempT;
		nextCPointController = nextCPointT.GetComponent<CPointController>();
	}

	public void changeSpeed (bool increase) {
		speed *= increase ? 2 : 0.5f;
	}

	int ShiftAfterTurn () {
		float closestShiftDistance = Mathf.Infinity;
		Vector3 turnDrift = transform.forward;
		int bestShift = 0;
		for (int shiftVariant = nextCPointController.leftSpace; shiftVariant <= nextCPointController.rightSpace; shiftVariant++) {
			float distToShift = Vector3.Distance(transform.TransformDirection(Vector3.forward) + turnDrift, new Vector3 (nextCPointT.position.x, nextCPointT.position.y, nextCPointT.position.z)  + nextCPointT.right * shiftVariant * 2);
			if (distToShift < closestShiftDistance) {
				closestShiftDistance = distToShift;
				bestShift = shiftVariant;
			}
		}
		return bestShift;
	}

	public void HandleHorzontalInput (int inputDir) {
		if (direction == -1) {
			inputDir *= -1;
		}
		int futurePosition = inputDir + nextShift;
		bool spaceAvailable = futurePosition <= nextCPointController.rightSpace && futurePosition >= nextCPointController.leftSpace;
		if (spaceAvailable) {
			nextShift += inputDir;	
//			carModelController.StartRotation(inputDir * direction);
		}
	}
}