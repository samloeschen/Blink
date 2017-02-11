using UnityEngine;
using System.Collections;

public class TestEnemy2 : EnemyGeneric{

	[Header("TestEnemy")]	
	public float moveSpeed;
	public float targetCheckRate; //time in seconds between target update checks
	public float centerDist;
	private float t;
	private Vector3 move;

	public override void Start () {
		base.Start();
		StartCoroutine(UpdateTarget());
	}
	public override void Update () {
		base.Update();
		UpdateMovement();
	}
	void FixedUpdate(){
		if(!damage && !knockback){
			rb.velocity=move;
		}else{
			rb.velocity=Vector2.zero;
		}
	}
	void UpdateMovement(){
		Vector3 moveDir = (target-transform.position).normalized;
		move=moveDir*moveSpeed;
	}
	IEnumerator UpdateTarget(){
		bool running = true;
		while(running){
			if(transform.position.magnitude>centerDist){
				target=Vector2.zero;
			}else{
				target = Random.insideUnitCircle.normalized*100f;
			}
			if((transform.position-target).magnitude<1f){
				target = Random.insideUnitCircle.normalized*100f;
			}
			yield return new WaitForSeconds(targetCheckRate);
		}
	}
}

