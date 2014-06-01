using UnityEngine;
using System.Collections;

public class SpawnEnemies : MonoBehaviour {
	public GameObject creatorPrefab;
	public GameObject proxyPrefab;
	public float interval;
	public int limit;
	
	// Start
	void Start() {
		if(!uLink.Network.isServer)
			return;

		if(GameManager.isTown || GameManager.isWorld)
			InvokeRepeating("SpawnSingleEnemy", 0.001f, interval);
	}
	
	// SpawnSingleEnemy
	void SpawnSingleEnemy() {
		if(Enemy.allEnemies.Count < limit) {
			uLink.Network.Instantiate(
				uLink.NetworkPlayer.server,
				proxyPrefab,
				creatorPrefab,
				creatorPrefab,
				transform.position,
				transform.rotation,
				0,						// Network group
				""						// Initial data
			);
			
			/*var enemy = obj.GetComponent<EnemyOnServer>();
			
			enemy.skillBuild = SkillBuild.GetStarterBuild();
			//enemy.custom = new CharacterCustomization();
			
			enemy.networkView.RPC("ReceiveSkillBuild", uLink.RPCMode.All, enemy.skillBuild);
			//enemy.networkView.RPC("ReceiveCharacterCustomization", uLink.RPCMode.All, enemy.custom);
			enemy.networkView.RPC("ReceiveCharacterStats", uLink.RPCMode.All, new CharacterStats());
			enemy.networkView.RPC("ReceiveArtifactTree", uLink.RPCMode.All, Jboy.Json.WriteObject(ArtifactTree.GetStarterArtifactTree()));
			
			// After the skill build has been sent, switch the attunement
			enemy.networkView.RPC("SwitchAttunement", uLink.RPCMode.All, (byte)0);*/
		}
	}
}
