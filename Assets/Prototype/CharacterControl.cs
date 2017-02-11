using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Vectrosity;
using UnityEngine.UI;


public class CharacterControl : MonoBehaviour {

	[Header("States")]
	public List<Color> stateColors;
	public List<Material> stateMaterials;
	[HideInInspector]
	public int state=1;

	[Header("Health")]
	public int maxHealth;
	[HideInInspector]
	public int cHealth;
	public float damageCoolDownTime;
	public Color damageColor;
	private float damageCoolDownTimer=1f;

	[Header("Movement")]
	public bool mouseControl;
	public float walkSpeed;
	public float blinkSpeedMin;
	public float blinkSpeedMax;
	public float blinksToMax;  
	public float blinkDistMin;
	public float blinkDistMax;
	public AnimationCurve blinkCurve;
	public AnimationCurve scaleCurve;
	public float blinkScale;
	private float cMoveSpeed=0f;
	private float cBlinkSpeed=0f;
	private float blinks=0;
	[HideInInspector]
	public bool blinkFlag = false;
	public float radiusPadding;
	private Vector2 move;

	[Header("Blink Timing")]
	public GameObject blinkDoneFlash;
	public float blinkComboTimeMin;
	public float blinkComboTimeMax;
	private float cBlinkComboTime;
	public float blinkCooldownTime;
	private float comboTime;
	private float cooldownTime;
	private bool comboFlag = false;
	private bool cooldownFlag = false;
	private bool comboBreak = false;


	[Header("BlinkLine")]
	public float blinkLineWidth;
	public Color32 blinkLineColor;		
	private VectorLine blinkLine;
	public List<Vector3> blinkLinePoints;
	private Vector3 activePoint;

	[Header("Particles")]
	public List<GameObject> particleContainers;


	[Header("Children")]
	public EyesBehavior eyes;
	private Vector2 blinkDir;
	private Vector2 blinkFrom;
	private Vector2 blinkTo;
	private float blinkLerp;
	private SpriteRenderer rend;
	private SpriteRenderer eyesRend;

	[Header("Combo Ring")]
	public float ringLine3DDepth;
	public float ringLineMinRadius;
	public float ringLineMaxRadius;
	public AnimationCurve ringRadiusCurve;
	private float ringLineCRadius;
	private float ringLineTempRadius;
	public List<Color> ringLineColors;
	public int ringLineSmoothness;
	public float ringLineWidth;
	private VectorLine ringLine;
	private Transform ringTransform;

	[Header("UI")]
	public List<Heart> hearts;
	public Text comboText;
	private int combo;

	[Header("Camera Effects")]
	public float colorVal;
	public AnimationCurve colorCurve;

	public float scanLineTime;
	public float scanLineAmount;
	public AnimationCurve scaneLineCurve;

	[Header("Death")]
	public List<ParticleSystem> deathParticles;
	public List<GameObject> deathFlash;

	private God god;
	private Rigidbody2D rb;
	void Start () {
		god = God.instance;
		cMoveSpeed=walkSpeed;
		rend = GetComponent<SpriteRenderer>();
		eyesRend = eyes.GetComponent<SpriteRenderer>();
		rb=GetComponent<Rigidbody2D>();
		ringTransform = new GameObject("ringTransform").transform;
		ringTransform.position = new Vector3(transform.position.x, transform.position.y, ringLine3DDepth);
		ringLine = new VectorLine("RingLine", new List<Vector3>(ringLineSmoothness), ringLineWidth);
		ringLine.drawTransform=ringTransform;
		ringLine.MakeCircle(Vector3.zero,ringLineMinRadius);
		ringLine.SetColor(ringLineColors[0]);
		ringLine.Draw3D();

		cHealth=maxHealth;
		StartCoroutine(HeartsFade());
	}
	void FixedUpdate(){
		if(!blinkFlag){
			rb.velocity=move;
		}
	}
	void Update () {
		if(Input.GetButtonDown("FlipState")){
			FlipState();
		}
		if(!blinkFlag){
			if(!god.gameOver){
				move=(Vector2)GetMove();
			}else{
				move=Vector3.zero;
			}

			if(!cooldownFlag){
				if(Input.GetButtonDown("Fire1")){
					if(mouseControl){
						DoBlink();
					}else{
						if(GetMove().sqrMagnitude!=0f){
							DoBlink();
						}
					}
				}
			}
		}else if(blinkFlag){
			if(Input.GetButtonDown("Fire1")){
				comboBreak=true;
			}
			cBlinkSpeed = Mathf.Lerp(blinkSpeedMin, blinkSpeedMax, blinks/blinksToMax);
			cBlinkComboTime = Mathf.Lerp(blinkComboTimeMax, blinkComboTimeMin, blinks/blinksToMax);
			blinkLerp+=Time.deltaTime*cBlinkSpeed;
			blinkLerp=Mathf.Clamp01(blinkLerp);
			transform.position=Vector2.Lerp(blinkFrom, blinkTo, blinkCurve.Evaluate(blinkLerp));
			float scaleLerp=0f;
			scaleLerp = Mathf.LerpUnclamped(1f, blinkScale, scaleCurve.Evaluate(blinkLerp));
			ringLineCRadius = Mathf.Lerp(ringLineTempRadius, ringLineMaxRadius, ringRadiusCurve.Evaluate(blinkLerp));
			transform.localScale = new Vector3(1f, scaleLerp, 1f);
			Camera.main.GetComponent<AnalogGlitch>().colorDrift = Mathf.Lerp(0, colorVal, colorCurve.Evaluate(blinkLerp));
			if(blinkLerp>=(1f-Time.deltaTime)){
				GameObject clone = (GameObject)Instantiate(blinkDoneFlash, transform.position, Quaternion.identity);
				clone.transform.parent=this.transform;
			}
			//check damage
			Collider2D[] colHit = Physics2D.OverlapCircleAll((Vector2)transform.position,
				radiusPadding,
				1<<LayerMask.NameToLayer("Enemy"));
			for(int i=0;i<colHit.Length;i++){
				EnemyGeneric enemy = colHit[i].GetComponent<EnemyGeneric>();
				if(enemy.state==state){
					combo+=enemy.ApplyDamage();
				}else{
					ApplyDamageSelf();
					combo=0;
				}
				//colHit[i].GetComponent<EnemyGeneric>().ApplyDamage();
			}	
			if(blinkLerp==1f){
				blinkFlag=false;
				if(comboBreak){
					comboFlag=false;
					cooldownFlag=true;
					comboTime=0f;
					ringLineCRadius=ringLineMinRadius;
				}else{
					comboFlag=true;
					comboTime=0;
				}
			}
		}
		if(comboFlag){
			comboTime+=Time.deltaTime/cBlinkComboTime;
			comboTime=Mathf.Clamp01(comboTime);
			cMoveSpeed = Mathf.Lerp(0, walkSpeed, comboTime);
			if(!blinkFlag){
				ringLineCRadius=Mathf.Lerp(ringLineMaxRadius, ringLineMinRadius, ringRadiusCurve.Evaluate(comboTime));
			}
			if(comboTime==1f){
				comboFlag=false;
				cooldownFlag=true;
				comboTime=0f;
				cooldownTime=0f;
				combo=0;
			}
		}else if(cooldownFlag){
			cooldownTime+=Time.deltaTime/blinkCooldownTime;
			cooldownTime=Mathf.Clamp01(cooldownTime);
			if(cooldownTime==1f){
				DiscardLine();
				cooldownFlag=false;
				comboBreak=false;
				cooldownTime=0f;
				blinks=0;
				cMoveSpeed=walkSpeed;
				combo=0;
			}
		}
		damageCoolDownTimer+=Time.deltaTime/damageCoolDownTime;
		damageCoolDownTimer=Mathf.Clamp01(damageCoolDownTimer);
		if(!god.gameOver){
			UpdateRingLine();
		}
		UpdateCombo();

		if(Input.GetKeyDown(KeyCode.Mouse0)){
			mouseControl=true;
		}
		if(Input.GetKeyDown(KeyCode.JoystickButton0)||Input.GetKeyDown(KeyCode.JoystickButton5)){
			mouseControl=false;
		}
	}
	public void UpdateRingLine(){
		ringTransform.position=new Vector3(transform.position.x, transform.position.y, ringLine3DDepth);
		ringTransform.localScale = Vector3.one*ringLineCRadius;
		ringLine.Draw3D();
	}
	public Vector3 GetMove(){
		Vector2 delta;
		float verticalInput = Input.GetAxis("Vertical");
		float horizontalInput = Input.GetAxis("Horizontal");
		delta = new Vector2(horizontalInput, verticalInput);
		return (delta*cMoveSpeed);
	}
	void DoBlink(){
		blinkFlag=true;
		blinkLerp=0f;
		ringLineTempRadius = ringLineCRadius;
		float blinkDist = Mathf.Lerp(blinkDistMin,blinkDistMax,(blinks/blinksToMax));
		if(mouseControl){
			Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			blinkDir = ((Vector2)mousePos-(Vector2)transform.position).normalized;
		}else{
			blinkDir = GetMove().normalized;
		}
		blinkFrom = transform.position;
		int mask = 1<<8;
		RaycastHit2D rHit = Physics2D.Raycast(blinkFrom,
			blinkDir,
			blinkDist,
			mask);
		if(rHit.collider!=null){
			if(radiusPadding>=rHit.distance){
				blinkDist=radiusPadding;
			}else{
				blinkDist=(rHit.distance-radiusPadding);
			}
		}
		blinkTo = blinkFrom+(blinkDir*blinkDist);
		transform.rotation = Quaternion.Euler(0f, 0f,Vector3.Angle(Vector3.right*Mathf.Sign(blinkDir.y), blinkDir));
		if(blinks==0f){
			blinkLinePoints = new List<Vector3>();
			activePoint = transform.position;
			blinkLinePoints.Add(activePoint);
			blinkLinePoints.Add(transform.position);
			blinkLine = new VectorLine("BlinkLine", blinkLinePoints, blinkLineWidth, LineType.Continuous, Joins.Weld);
			blinkLine.SetColor(blinkLineColor);
		}else{
			blinkLinePoints.Add(transform.position);
			blinkLine.SetColor(blinkLineColor);
		}
		blinks++;
	}
	void FlipState(){
		eyesRend.color=stateColors[state];
		StartCoroutine(eyes.Blink());
		particleContainers[state].SetActive(false);
		if(state==0){
			state++;

		}else{
			state--;
		}
		rend.color=stateColors[state];
		particleContainers[state].SetActive(true);
		ringLine.SetColor(ringLineColors[state]);
	}
	void DiscardLine(){
		VectorLine.Destroy(ref blinkLine);
	}
	void OnCollisionEnter2D(Collision2D col){
	}
	public void ApplyDamage(){
		if(!blinkFlag){
			if(damageCoolDownTimer==1 && !god.gameOver){
				damageCoolDownTimer=0f;
				if(cHealth>0){
					hearts[cHealth-1].Disable();
				}else{
					Instantiate(deathParticles[state], transform.position,Quaternion.identity);
					Instantiate(deathFlash[state], transform.position, Quaternion.identity);
					god.StartCoroutine(god.FadeInRestart());
					GetComponent<SpriteRenderer>().enabled=false;
					particleContainers[state].SetActive(false);
					VectorLine.Destroy(ref ringLine);
					eyesRend.enabled=false;

					//this.enabled=false;
				}
				cHealth-=1;
				StartCoroutine(DamageBlink());
				StartCoroutine(DamageEffect());
			}
		}
	}
	public void ApplyDamageSelf(){
		if(damageCoolDownTimer==1 && !god.gameOver){
			damageCoolDownTimer=0f;
			if(cHealth>0){
				hearts[cHealth-1].Disable();
			}else{
				Instantiate(deathParticles[state], transform.position,Quaternion.identity);
				Instantiate(deathFlash[state], transform.position, Quaternion.identity);
				god.StartCoroutine(god.FadeInRestart());
				GetComponent<SpriteRenderer>().enabled=false;
				particleContainers[state].SetActive(false);
				eyesRend.enabled=false;
				VectorLine.Destroy(ref ringLine);
				//this.enabled=false;
			}
			cHealth-=1;
			StartCoroutine(DamageBlink());
			StartCoroutine(DamageEffect());
		}
	}
	IEnumerator DamageBlink(){
		int flashes = 5;
		for(int i=0;i<flashes;i++){
			if(rend.color==damageColor){
				rend.color=stateColors[state];
			}else{
				rend.color=damageColor;
			}
			yield return new WaitForSeconds(damageCoolDownTime/flashes);
		}
		rend.color=stateColors[state];
	}
	IEnumerator HeartsFade(){
		for(int i=0;i<hearts.Count;i++){
			hearts[i].GetComponent<Image>().color = new Color(1f,1f,1f,0f);
		}
		yield return new WaitForSeconds(3.0f);
		float t=0f;
		while(t<1f){
			t+=Time.deltaTime/2.0f;
			t=Mathf.Clamp01(t);
			for(int i=0;i<hearts.Count;i++){
				hearts[i].GetComponent<Image>().color = new Color(1f,1f,1f,t);
			}
			yield return null;
		}
	}
	void UpdateCombo(){
		comboText.text="COMBO: "+combo;
	}
	IEnumerator DamageEffect(){
		float t=0f;
		while (t<1f){
			t+=Time.deltaTime/scanLineTime;
			t=Mathf.Clamp01(t);
			Camera.main.GetComponent<AnalogGlitch>().scanLineJitter = Mathf.Lerp(0, scanLineAmount, scaneLineCurve.Evaluate(t));
			yield return null;
		}
		Camera.main.GetComponent<AnalogGlitch>().scanLineJitter=0f;
	}
}
