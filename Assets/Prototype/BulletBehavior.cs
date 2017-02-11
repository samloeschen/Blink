using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BulletBehavior : MonoBehaviour {
	public float bulletSpeed;
	public float bulletTime;
	public int bulletState;
	public List<Sprite> bulletSprites;
	public List<ParticleSystem> particles;
	[HideInInspector]
	public Vector2 bulletDir;
	private Rigidbody2D rb;
	private SpriteRenderer rend;
	private Vector2 last;
	private float t;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody2D>();
		rend = GetComponent<SpriteRenderer>();
		rend.sprite = bulletSprites[bulletState];
	}
	
	// Update is called once per frame
	void Update () {
		if(t<bulletTime){
			t += Time.deltaTime;
			Vector2 move = bulletDir*bulletSpeed*Time.deltaTime;
			rb.position += move;
		}else{
			DestroyBullet();
		}
	}
	void LateUpdate(){
		last = rb.position;
	}
	void OnTriggerEnter2D(Collider2D col){
		CharacterControl player = col.GetComponent<CharacterControl>();
		if(player){
			if(player.state!=bulletState){
				player.GetComponent<CharacterControl>().ApplyDamage();
			}
		}
	}
	void DestroyBullet(){
		Instantiate(particles[bulletState],transform.position,Quaternion.identity);
		Destroy(gameObject);
	}
}
