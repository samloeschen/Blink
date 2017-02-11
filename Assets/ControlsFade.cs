using UnityEngine;
using System.Collections;

public class ControlsFade : MonoBehaviour {
	public float fadeTime;
	public float waitTime;
	// Use this for initialization
	private Color cacheColor;
	void Start () {
		StartCoroutine(Fade());
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	IEnumerator Fade(){
		cacheColor=GetComponent<SpriteRenderer>().color;
		yield return new WaitForSeconds(waitTime);
		float t = 0f;
		while(t<1f){
			t+=Time.deltaTime/fadeTime;
			t=Mathf.Clamp01(t);
			float a = Mathf.Lerp(cacheColor.a, 0f, t);
			GetComponent<SpriteRenderer>().color = new Color(cacheColor.r, cacheColor.g, cacheColor.b, a);
			yield return null;
		}
	}
}
