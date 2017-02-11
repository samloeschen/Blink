using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyGeneric : MonoBehaviour {

	public int maxHealth;
	[HideInInspector]
	public int cHealth;
	[HideInInspector]
	public int state;
	[HideInInspector]
	public bool death = false;



	[Header("Knockback")]
	public float knockbackDistMin;
	public float knockbackDistMax;
	public float knockbackRadius;
	public float knockbackSpeed;
	public AnimationCurve knockbackCurve;
	protected bool knockback = false;

	[Header("Colors")]
	public List<Color> stateColors;
	public Color takeDamageColor;
	public float damageFreezeFrames;
	public int damageFlashes;

	[Header("ParticleEffects")]
	public List<GameObject> particlePrefabs;
	public List<GameObject> flashPrefabs;

	[Header("Damage")]
	public float damageRadius;

	public SpriteRenderer eyeSprite;
	public Sprite eyeNormal;
	public Sprite eyeDamage;
	[HideInInspector]
	public Vector3 target;

	[HideInInspector]
	protected God god;

	protected Rigidbody2D rb;
	protected SpriteRenderer rend;
	protected bool damage = false;
	public virtual void Start(){
		god = God.instance;
		cHealth=maxHealth;
		SpriteRenderer[] list=(SpriteRenderer[])GetComponentsInChildren<SpriteRenderer>();
		eyeSprite=list[1];
		eyeSprite.sprite=eyeNormal;
		//eyeSprite.sprite=eyeNormal;
		state = Random.Range(0,2);
		rb=GetComponent<Rigidbody2D>();
		rend=GetComponent<SpriteRenderer>();
		rend.color = stateColors[state];
	}
	public virtual void Update(){
		if(!death && !knockback){
			float dist = Vector3.Distance(transform.position, god.GetPlayerPosition());
			if(dist<damageRadius){
				god.GetPlayer().ApplyDamage();
			}
		}
	}
	public virtual int ApplyDamage(){
		if(!damage && !knockback){
			cHealth-=1;
			if(cHealth<0){
				cHealth=0;
				death=true;
			}
			if(cHealth==0){
				death=true;
				DoKnockback();
			}
			damage=true;
			StartCoroutine(DamageAnimation());
			return 1;
		}else{
			return 0;
		}
	}
	IEnumerator DamageAnimation(){
		eyeSprite.sprite=eyeDamage;
		rend.color=takeDamageColor;
		eyeSprite.transform.position += Vector3.up/2;
		yield return new WaitForSeconds(Time.deltaTime*damageFreezeFrames);
		rend.color=stateColors[state];
		if(cHealth==0){
			death=true;
			StartCoroutine(DeathAnimation());
		}else{
			eyeSprite.transform.position -= Vector3.up/2;
			eyeSprite.sprite=eyeNormal;
			damage=false;
		}
	}
	IEnumerator DeathAnimation(){
		Instantiate(particlePrefabs[state],transform.position,Quaternion.identity);
		GameObject flash = (GameObject)Instantiate(flashPrefabs[state],transform.position,Quaternion.identity);
		flash.transform.localScale = Vector3.one*(transform.localScale.x*7f);
		eyeSprite.color = new Color(1f,1f,1f,1f);
		death=true;
		for(int i=0;i<5;i++){
			if(eyeSprite.color.a==1f){
				eyeSprite.color = new Color(1f,1f,1f,0f);
			}else{
				eyeSprite.color = new Color(1f,1f,1f,1f);
			}
			yield return new WaitForSeconds(0.05f);
		}
		eyeSprite.color = new Color(1f,1f,1f,0f);
		rend.color = new Color(stateColors[state].r,stateColors[state].g,stateColors[state].b,0f);
		god.score++;
		Destroy(gameObject);
	}
	public IEnumerator Knockback(Vector2 to, float speed){
		if(!knockback){
			knockback=true;
			float t=0f;
			Vector2 startPos = (Vector2)transform.position;
			while(t<1f){
				t+=Time.deltaTime/speed;
				t=Mathf.Clamp01(t);
				transform.position = Vector2.LerpUnclamped(startPos, to, knockbackCurve.Evaluate(t));
				yield return null;
			}
			knockback=false;
		}
	}
	public void DoKnockback(){
		Collider2D[] hit = Physics2D.OverlapCircleAll(transform.position, knockbackRadius, 1<<LayerMask.NameToLayer("Enemy"));
		for(int i=0;i<hit.Length;i++){
			if(hit[i].GetInstanceID()!=this.GetInstanceID()){
				EnemyGeneric enemy = hit[i].GetComponent<EnemyGeneric>();
				if(enemy.state!=this.state){
					float dist = Mathf.Lerp(knockbackDistMax, 
						knockbackDistMin, 
						Vector3.Distance(transform.position, hit[i].transform.position)/knockbackRadius);
					Vector2 to = enemy.transform.position + (enemy.transform.position-transform.position).normalized*dist;
					enemy.StartCoroutine(enemy.Knockback(to, knockbackSpeed));
				}
			}
		}
	}
	public virtual void OnTriggerEnter2D(Collider2D col){
		if(!death && !knockback){
			CharacterControl player = col.GetComponent<CharacterControl>();
			if(player){
				//player.ApplyDamage();
				Debug.Log("player dmg" + "" + gameObject.name);
			}
		}
	}
}
