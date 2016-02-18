using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	Transform playerTransform;


	void Awake () {
		playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform> ();
	}

	// Use this for initialization
	void Start () {
		transform.LookAt(playerTransform);
	}
	
	// Update is called once per frame
	void Update () {
		
		transform.position = Vector3.Slerp(transform.position, playerTransform.position + Vector3.up*6 + -6 * playerTransform.forward, Time.deltaTime * 5f);


		Quaternion neededRotation = Quaternion.LookRotation(playerTransform.position - transform.position);
		transform.rotation = Quaternion.Slerp(transform.rotation, neededRotation, Time.deltaTime * 5f);
	}
}
