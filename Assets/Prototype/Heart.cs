using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Heart : MonoBehaviour {

	private Image rend;
	// Use this for initialization
	void Start () {
		rend = GetComponent<Image>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Enable(){

	}
	public void Disable(){
		rend.enabled = false;
	}
}
