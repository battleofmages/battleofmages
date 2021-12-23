using BoM.Core;
using System;
using UnityEngine;
using UnityEngine.Pool;

namespace BoM.Skills {
	[Serializable]
	public class Skill : ISkill {
		public short Id { get; set; }

		[SerializeField]
		private string name;

		public string Name {
			get {
				return name;
			}
		}

		public Instance prefab;
		public PositionType position;
		public RotationType rotation;
		public ObjectPool<Instance> pool;
	}
}
