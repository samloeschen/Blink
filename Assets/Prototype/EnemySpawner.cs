using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class EnemySpawner : MonoBehaviour {
	public List<Transform> spawnNodes;

	[Header("SpawnTiming")]
	public float startSpawnFrequencyMin;
	public float startSpawnFrequencyMax;
	public float finalSpawnFrequencyMin;
	public float finalSpawnFrequencyMax;
	public float minsToFinalFrequency;
	private float cFrequency=0f;

	[Header("SpawnAmounts")]
	public float startSpawnAmountMin;
	public float startSpawnAmountMax;
	public float finalSpawnAmountMin;
	public float finalSpawnAmountMax;
	public float minsToFinalAmount;
	private float cAmount=0f;

	[Header("SpawnVariety")]
	public List<EnemyGeneric> enemyPrefabs;
	public List<float> spawnWeights;
	public int startSpawnIndex;
	public float minsToMaxIndex;
	private float cIndex=0;

	[Header("Positioning")]
	public float minOffsetDist;
	public float maxOffsetDist;
	public float extraPadding;

	private float currentSpawnTime;
	private float currentSpawnTimer;

	private God god;

	// Use this for initialization
	void Start () {
		god=God.instance;
		//currentSpawnTime=2f;
	}
	
	// Update is called once per frame
	void Update () {
		if(!god.gameOver){
			cFrequency+=Time.deltaTime/(minsToFinalFrequency*60f);
			cFrequency=Mathf.Clamp01(cFrequency);
			cAmount+=Time.deltaTime/(minsToFinalAmount*60f);
			cAmount=Mathf.Clamp01(cAmount);
			cIndex+=Time.deltaTime/(minsToMaxIndex*60f);
			cIndex=Mathf.Clamp01(cIndex);

			currentSpawnTimer+=Time.deltaTime/currentSpawnTime;
			currentSpawnTimer=Mathf.Clamp01(currentSpawnTimer);
			if(currentSpawnTimer==1f){
				Spawn();
				currentSpawnTimer=0f;
			}
		}
	}
	void Spawn(){
		float min = Mathf.Lerp(startSpawnAmountMin, finalSpawnAmountMin,cAmount);
		float max = Mathf.Lerp(startSpawnAmountMax, finalSpawnAmountMax,cAmount);
		float amount = Random.Range(min,max);
		int maxIndex = Mathf.FloorToInt(Mathf.Lerp(startSpawnIndex, enemyPrefabs.Count,cIndex))+1;
		List<Transform> nodes = new List<Transform>();
		for(int i=0;i<2;i++){
			float dist=0f;
			Transform greatest = null;
			for(int i2=0;i2<spawnNodes.Count;i2++){
				if(!nodes.Contains(spawnNodes[i2])){
					float sqrOffset = (god.GetPlayerPosition()-spawnNodes[i2].position).sqrMagnitude;
					if(sqrOffset>dist){
						dist=sqrOffset;
						greatest=spawnNodes[i2];
					}
				}
			}
			nodes.Add(greatest);
		}
		for(int i=0;i<amount;i++){
			int index=0;
			bool roll = true;
			while(roll){
				index = Random.Range(0,maxIndex);
				if(Random.value<=spawnWeights[index]){
					roll=false;
				}
			}
			EnemyGeneric enemy = enemyPrefabs[index];
			Vector2 pos = (Vector2)nodes[Random.Range(0,2)].position;
			Vector2 offset = Random.insideUnitCircle.normalized*(Random.Range(minOffsetDist,maxOffsetDist));
			int mask1 = LayerMask.NameToLayer("Enemy");
			int mask2 = LayerMask.NameToLayer("Wall");
			Collider2D[] hit = new Collider2D[1];
			while(Physics2D.OverlapCircleNonAlloc(pos+offset,enemy.damageRadius*3f,hit,mask1|mask2)>0){
				offset = Random.insideUnitCircle.normalized*(Random.Range(minOffsetDist,maxOffsetDist)+extraPadding);
			}
			Instantiate(enemy, pos+offset, Quaternion.identity);
		}
		min = Mathf.Lerp(startSpawnFrequencyMin, finalSpawnFrequencyMin, cFrequency);
		max = Mathf.Lerp(startSpawnFrequencyMax, finalSpawnFrequencyMax, cFrequency);
		currentSpawnTime = Random.Range(min,max);
	}
}
