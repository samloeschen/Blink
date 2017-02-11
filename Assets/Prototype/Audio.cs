using UnityEngine;
using System.Collections;

public class Audio : MonoBehaviour {


	void Awake(){
		Audio[] audios = FindObjectsOfType(typeof(Audio)) as Audio[];
		if(audios.Length>1){
			Destroy(gameObject);
		}
		DontDestroyOnLoad(gameObject);
	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
