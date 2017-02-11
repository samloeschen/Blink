using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class God : MonoBehaviour {

	//singleton
	public static God instance;
	public Text scoreText;
	public Text gameOverText;
	public Text restartText;
	public int score;
	public CharacterControl player;
	public bool restart;
	public bool gameOver;
	
	void Awake(){
		God[] gods = FindObjectsOfType(typeof(God)) as God[];
		if(gods.Length>1){
			Destroy(gameObject);
		}else{
			instance=this;
		}
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		UpdateScore();
		if(restart){
			if(Input.GetButtonDown("Fire1")){
				SceneManager.LoadScene("Prototype");
				restart=false;
			}
		}
	}

	public Vector3 GetPlayerPosition(){
		if(!gameOver){
			return player.transform.position;
		}else{
			return Vector3.zero;
		}
	}
	public CharacterControl GetPlayer(){
		return player;
	}
	public void UpdateScore(){
		scoreText.text = "SCORE: "+score;
	}
	public IEnumerator FadeInRestart(){
		gameOver=true;
		Color cacheColor = gameOverText.color;
		gameOverText.enabled=true;
		restartText.enabled=true;
		float t=0f;
		while(t<1f){
			t+=Time.deltaTime/2.0f;
			float a = Mathf.Lerp(0,1,t);
			gameOverText.color = new Color(cacheColor.r, cacheColor.g, cacheColor.b, a);
			restartText.color = new Color(cacheColor.r, cacheColor.g, cacheColor.b, a);
			yield return null;
		}
		restart = true;
	}

}
