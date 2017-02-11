using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class StartScreen : MonoBehaviour {

	public GameObject fadeObject;

	// Use this for initialization
	void Start () {
		StartCoroutine(Fade());
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	IEnumerator Fade(){
		yield return new WaitForSeconds(2f);
		fadeObject.SendMessage("FadeIn");
		yield return new WaitForSeconds(5f);
		fadeObject.SendMessage("FadeOut");
		yield return new WaitForSeconds(3f);
		SceneManager.LoadScene("Prototype");
	}

}
