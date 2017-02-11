using UnityEngine;
using System.Collections;

public class BlinkFlash : MonoBehaviour {

	public float frames;
	private SpriteRenderer rend;
	private float startAlpha;
	// Use this for initialization
	void Start () {
		rend = GetComponent<SpriteRenderer>();
		startAlpha = rend.color.a;
		rend.color = new Color(1,1,1,0f);
		StartCoroutine(Flash());
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	IEnumerator Flash(){
		rend.color = new Color(1,1,1,startAlpha);
		yield return new WaitForSeconds(frames*Time.deltaTime);
		rend.color = new Color(1,1,1,0f);
		Destroy(gameObject);
	}
}
