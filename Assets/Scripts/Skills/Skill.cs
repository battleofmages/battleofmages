using BoM.Core;
using System;
using UnityEngine.Pool;

namespace BoM.Skills {
	[Serializable]
	public class Skill : ISkill {
		public string name;
		public Instance prefab;
		public PositionType position;
		public RotationType rotation;
		public ObjectPool<Instance> pool;

		[NonSerialized]
		public Element element;

		public string Name {
			get {
				return name;
			}
		}
	}
}
