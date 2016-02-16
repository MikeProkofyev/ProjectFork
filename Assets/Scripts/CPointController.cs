using UnityEngine;
using System.Collections;

public class CPointController : MonoBehaviour {

	public Transform pointAhead;
	public Transform pointLeft;
	public Transform pointRight;
	public Transform invPointAhead;

	public int rightSpace = 1;
	public int leftSpace = -1;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public bool PointingTheSameDirection (Vector3 playerPosition) {
		return Vector3.Dot(playerPosition.normalized, transform.position.normalized) >= 0 ? true : false;
	}
}
