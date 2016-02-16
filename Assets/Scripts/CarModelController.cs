using UnityEngine;
using System.Collections;

public class CarModelController : MonoBehaviour {

	float t = 1;
	int turnDirection;
	float turnAngle = 45f;

	// Use this for initialization
	void Start () {
		t = 1;
	}
	
	// Update is called once per frame
	void Update () {
		if (t < 1) {
			t += Time.deltaTime * 3;
			transform.localRotation = Quaternion.Euler(new Vector3(0, Mathf.LerpAngle(0, turnDirection * turnAngle, Mathf.Sin(Mathf.PI * t)), 0));
		}		
	}

	public void StartRotation (int turnDirection) {
		this.turnDirection = turnDirection;
		t = 0;
	}
}
