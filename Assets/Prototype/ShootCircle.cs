using UnityEngine;
using System.Collections;

public class ShootCircle : MonoBehaviour {

	// Use this for initialization

	public BulletBehavior bulletPrefab;
	public float shootRate;
	public float shootRadius;
	public int shots;
	private float shootTimer;
	private EnemyGeneric enemy;

	void Start () {
		enemy = GetComponent<EnemyGeneric>();
	}
	// Update is called once per frame
	void Update () {
		shootTimer+=Time.deltaTime/shootRate;
		shootTimer=Mathf.Clamp01(shootTimer);
		if(shootTimer==1){
			DoShot();
			shootTimer=0f;
		}
		if(enemy.death){
			this.enabled=false;
		}
	}
	void DoShot(){
		for(int i=0;i<shots;i++){
			float ang = i*Mathf.PI*2/shots;
			Vector3 pos = new Vector3(Mathf.Cos(ang), Mathf.Sin(ang), 0f) * shootRadius;
			BulletBehavior bullet = (BulletBehavior)Instantiate(bulletPrefab,
				transform.position+pos,
				Quaternion.identity);
			bullet.bulletDir=((transform.position+pos)-transform.position).normalized;
			bullet.bulletState=enemy.state;
		}
	}	
}
