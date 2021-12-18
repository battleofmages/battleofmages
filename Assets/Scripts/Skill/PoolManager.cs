using UnityEngine;
using UnityEngine.Pool;
using System.Collections.Generic;

namespace BoM.Skill {
	public static class PoolManager {
		public static Dictionary<Instance, ObjectPool<Instance>> pools = new Dictionary<Instance, ObjectPool<Instance>>();

		public static ObjectPool<Instance> GetPool(Instance prefab) {
			if(pools.TryGetValue(prefab, out ObjectPool<Instance> pool)) {
				return pool;
			}

			var newPool = new ObjectPool<Instance>(() => {
				return GameObject.Instantiate(prefab);
			}, instance => {
				instance.gameObject.SetActive(true);
			}, instance => {
				instance.gameObject.SetActive(false);
			}, instance => {
				GameObject.Destroy(instance.gameObject);
			}, false, 32, 64);

			pools.Add(prefab, newPool);
			return newPool;
		}
	}
}
