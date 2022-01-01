using BoM.Core;
using UnityEngine;
using UnityEngine.Pool;

namespace BoM.Skills {
	[CreateAssetMenu(fileName = "Skill", menuName = "BoM/Skill", order = 1)]
	public class Skill : ScriptableObject, ISkill {
		public short Id { get; set; }
		public string Name { get => name; }

		public Instance prefab;
		public PositionType position;
		public RotationType rotation;
		public float coolDown;
		public ObjectPool<Instance> pool;
	}
}
