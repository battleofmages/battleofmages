using UnityEngine;
using System.Collections;

public class GameObjectPool {
	private GameObject[] pool;
	private int poolIndex;
	private readonly int poolSize;
	
	public GameObjectPool(GameObject prefab, int size) {
		poolIndex = 0;
		poolSize = size;
		pool = new GameObject[poolSize];
		
		for(int i = 0; i < poolSize; i++) {
			pool[i] = (GameObject)GameObject.Instantiate(prefab);
			pool[i].SetActive(false);
		}
	}
	
	public GameObject Instantiate(Vector3 position, Quaternion rotation) {
		var inst = this.nextInstance;
		inst.transform.position = position;
		inst.transform.rotation = rotation;
		inst.SetActive(true);
		return inst;
	}
	
	public GameObject nextInstance {
		get {
			var obj = pool[poolIndex++];
			
			if(poolIndex == poolSize)
				poolIndex = 0;
			
			return obj;
		}
	}
}
