using UnityEngine;
using UnityEngine.Pool;

namespace BoM.Skills {
	public abstract class Instance : MonoBehaviour {
		[System.NonSerialized]
		public Skill skill;
		public ObjectPool<Instance> pool;

		public abstract void Init();

		public void Release() {
			pool.Release(this);
		}
	}
}
