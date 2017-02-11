using UnityEngine;
using System.Collections;

public class EyesBehavior : MonoBehaviour {

	[Header("Positioning")]
	public float zDepth;
	[Header("Blinking (heh)")]
	public float blinkFrequencyMin;
	public float blinkFrequencyMax;
	public float blinkTime;
	public AnimationCurve blinkCurve;
	private float blinkFrequencyTime;
	private float blinkFrequencyTimer;
	[Header("Move Offset")]
	public float moveDist;
	public float moveDamp;
	private float targetPos;
	private Vector3 velocity;

	private CharacterControl character;
	// Use this for initialization
	void Start () {
		character = transform.parent.GetComponent<CharacterControl>();
		blinkFrequencyTime=Random.Range(blinkFrequencyMin, blinkFrequencyMax);
	}
	
	// Update is called once per frame
	void Update () {
		transform.rotation = Quaternion.identity;
		Vector3 target = new Vector3(character.transform.position.x, character.transform.position.y, -1f) + (character.GetMove().normalized * moveDist);
		transform.position=Vector3.SmoothDamp(transform.position, target, ref velocity, moveDamp);
		blinkFrequencyTimer+=Time.deltaTime/blinkFrequencyTime;
		blinkFrequencyTimer=Mathf.Clamp01(blinkFrequencyTimer);
		if(blinkFrequencyTimer==1f){
			StartCoroutine(Blink());
			blinkFrequencyTimer=0f;
			blinkFrequencyTime=Random.Range(blinkFrequencyMin, blinkFrequencyMax);
		}
	}
	public IEnumerator Blink(){
		float t = 0;
		while(t<1f){
			t+=Time.deltaTime/blinkTime;
			float scale = blinkCurve.Evaluate(Mathf.LerpUnclamped(1, 0, Mathf.PingPong(t*2, 1)/1));
			transform.localScale= new Vector3(1, scale, 1);
			yield return null;
		}
	}
}
