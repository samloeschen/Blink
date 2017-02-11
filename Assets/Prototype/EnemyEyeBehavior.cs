using UnityEngine;
using System.Collections;

public class EnemyEyeBehavior : MonoBehaviour {

	[Header("Movement")]
	public float moveDist;
	public float moveDamp;
	[Header("Blinking")]
	public float blinkSpeed;
	public float blinkFrequencyMin;
	public float blinkFrequencyMax;
	public float blinkTime;
	public AnimationCurve blinkCurve;
	private float blinkFrequencyTime;
	private float blinkFrequencyTimer;
	private EnemyGeneric enemy;
	private Vector3 velocity;
	// Use this for initialization
	void Start () {
		enemy = transform.parent.GetComponent<EnemyGeneric>();
		//StartCoroutine(Blink());
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 target = new Vector3(enemy.transform.position.x, enemy.transform.position.y, -1f) + ((enemy.target-enemy.transform.position).normalized*moveDist);
		transform.position = Vector3.SmoothDamp(transform.position,target, ref velocity, moveDamp);
		blinkFrequencyTimer+=Time.deltaTime/blinkFrequencyTime;
		blinkFrequencyTimer=Mathf.Clamp01(blinkFrequencyTimer);
		if(blinkFrequencyTimer==1f){
			StartCoroutine(Blink());
			blinkFrequencyTimer=0f;
			blinkFrequencyTime=Random.Range(blinkFrequencyMin, blinkFrequencyMax);
		}
	}
	public IEnumerator Blink(){
		float t=0;
		while(t<1f){
			t+=Time.deltaTime/blinkTime;
			float scale = blinkCurve.Evaluate(Mathf.LerpUnclamped(1,0,Mathf.PingPong(t*2,1)/1));
			transform.localScale=new Vector3(1,scale,1);
			yield return null;
		}
	}
}
