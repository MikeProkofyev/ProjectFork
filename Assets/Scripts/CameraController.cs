using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	Transform playerTransform;


	void Awake () {
		playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform> ();
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.LookAt(playerTransform);
//		transform.position = playerTransform.GetComponent<MovementController> ().currCPointT.position;
	}
}
