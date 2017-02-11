using UnityEngine;
using System.Collections;

public class TrailOffset : MonoBehaviour {

	public float offsetDist;
	public CharacterControl character;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		transform.position = character.transform.position+character.GetMove().normalized*offsetDist;
	
	}
}
