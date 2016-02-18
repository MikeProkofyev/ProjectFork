using UnityEngine;
using System.Collections;

public class CarModelController : MonoBehaviour {

	float t = 0;
	int turnDirection;
	float turnAngle = 45f;
	float turnSpeed = 8;

	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
//		if (t < 1) {
//			t += Time.deltaTime * 3;
//			transform.localRotation = Quaternion.Euler(new Vector3(0, Mathf.LerpAngle(0, turnDirection * turnAngle, Mathf.Sin(Mathf.PI * t)), 0));
//		}


		if (t > 0) {
			t -= Time.deltaTime * turnSpeed;
			transform.Rotate(0, turnDirection * Time.deltaTime * 120, 0);
		}
		else if (transform.localEulerAngles.y != 0) {  //Probably wrong?
			transform.Rotate(0, -1 * turnDirection * Time.deltaTime * 120, 0);
		}

		float clampedYRotation = 0;
		if (turnDirection == 1) {
			clampedYRotation = ClampAngle(transform.localRotation.eulerAngles.y, 0, turnAngle);
		}else {
			clampedYRotation = ClampAngle(transform.localRotation.eulerAngles.y, -turnAngle, 0);
		}

		transform.localRotation = Quaternion.Euler(new Vector3(0 ,clampedYRotation , 0));	

		Debug.Log(transform.localRotation.eulerAngles);

	}

	public void StartRotation (int turnDirection, float turnForce) {
		this.turnDirection = turnDirection;
		t = turnForce;
		Debug.Log("Dir: " + this.turnDirection + " force " + t);
	}

	float ClampAngle(float angle, float min,float max) {

		if (angle<90 || angle>270){       // if angle in the critic region...
			if (angle>180) angle -= 360;  // convert all angles to -180..+180
			if (max>180) max -= 360;
			if (min>180) min -= 360;
		}    
		angle = Mathf.Clamp(angle, min, max);
		if (angle<0) angle += 360;  // if angle negative, convert to 0..360
		return angle;
	}
}
