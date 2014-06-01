using UnityEngine;
using System.Collections;

public class SpawnEnemy : MonoBehaviour {
	public GameObject creatorPrefab;
	public GameObject proxyPrefab;

	// Start
	void Start() {
		if(!uLink.Network.isServer)
			return;
		
		if(GameManager.isTown || GameManager.isWorld)
			SpawnSingleEnemy();
	}
	
	// SpawnSingleEnemy
	void SpawnSingleEnemy() {
		// Instantiate new enemy
		var enemy = uLink.Network.Instantiate(
			uLink.NetworkPlayer.server,
			proxyPrefab,
			creatorPrefab,
			creatorPrefab,
			transform.position + Vector3.up,
			transform.rotation,
			0,						// Network group
			""						// Initial data
		);

		// Make it respawn on death
		var entity = enemy.GetComponent<Entity>();
		entity.onDeath += StartRespawnTimer;
	}

	// StartRespawnTimer
	void StartRespawnTimer() {
		StartCoroutine(RespawnDelayed(Config.instance.enemyRespawnTime));
	}

	// RespawnDelayed
	IEnumerator RespawnDelayed(float time) {
		yield return new WaitForSeconds(time);
		SpawnSingleEnemy();
	}
}
