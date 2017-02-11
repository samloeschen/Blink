using UnityEngine;
using System.Collections;

public class Shoot : MonoBehaviour {

	// Use this for initialization

	public BulletBehavior bulletPrefab;
	public float shootRate;
	public int shots;
	public float shotTime; //time between shots in a burst for example
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
			StartCoroutine(DoShot());
			shootTimer=0f;
		}
		if(enemy.death){
			this.enabled=false;
		}
	}
	IEnumerator DoShot(){
		for(int i=0;i<shots;i++){
			BulletBehavior bullet = (BulletBehavior)Instantiate(bulletPrefab,
				transform.position,
				Quaternion.identity);
			bullet.bulletDir=(enemy.target-transform.position).normalized;
			bullet.bulletState=enemy.state;
			yield return new WaitForSeconds(shotTime/shots);
		}
	}
	
}
