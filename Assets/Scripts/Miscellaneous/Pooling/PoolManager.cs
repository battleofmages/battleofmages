using UnityEngine;
using System.Collections;

public class PoolManager : SingletonMonoBehaviour<PoolManager> {
	public GameObject blizzardProjectilePrefab;
	
	public GameObjectPool blizzard;
	
	void OnLevelWasLoaded() {
		MakePoolable(blizzardProjectilePrefab.transform);
		foreach(Transform child in blizzardProjectilePrefab.transform) {
			MakePoolable(child);
		}
		
		blizzard = new GameObjectPool(blizzardProjectilePrefab, 200);
	}
	
	void MakePoolable(Transform child) {
		var trails = child.GetComponent<TrailRenderer>();
		if(trails)
			trails.autodestruct = false;
		
		var autoDestruct = child.GetComponent<AutoDestruct>();
		if(autoDestruct)
			autoDestruct.enabled = false;
	}
}
