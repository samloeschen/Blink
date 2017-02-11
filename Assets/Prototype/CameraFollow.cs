using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {

	public Transform target;
	public float moveDamping;
	private Vector3 velocity;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		FollowTarget();
	}
	void FollowTarget(){
		Vector3 delta = new Vector3(target.position.x, target.position.y,-10f)-transform.position;
		Vector3 move = Vector3.SmoothDamp(transform.position, transform.position+delta, ref velocity, moveDamping);
		transform.position=move;
	}
}
