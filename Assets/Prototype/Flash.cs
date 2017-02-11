using UnityEngine;
using System.Collections;

public class Flash : MonoBehaviour {

	public int frames;
	// Use this for initialization
	void Start () {
		StartCoroutine(FlashFrame());
	}
	// Update is called once per frame
	void Update () {
	}
	IEnumerator FlashFrame(){
		yield return new WaitForSeconds(frames*Time.deltaTime);
		Destroy(gameObject);
	}
}
