using UnityEngine;
using System.Collections;

public class MainGameFade : MonoBehaviour {

	public GameObject fadeObj;
	// Use this for initialization
	void Start () {
		fadeObj.SendMessage("FadeIn");
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void FadeOut(){
		fadeObj.SendMessage("FadeOut");
	}
}
