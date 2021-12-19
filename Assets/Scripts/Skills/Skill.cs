using System;
using UnityEngine.Pool;

namespace BoM.Skills {
	[Serializable]
	public class Skill {
		public string name;
		public Instance prefab;
		public PositionType position;
		public RotationType rotation;
		public ObjectPool<Instance> pool;

		[NonSerialized]
		public Element element;

		public Instance Instantiate() {
			var instance = pool.Get();
			instance.skill = this;
			return instance;
		}
	}
}
