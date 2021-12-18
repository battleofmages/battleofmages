using System;
using UnityEngine;

namespace BoM.Skills {
	[Serializable]
	public class Skill {
		public string name;
		public GameObject prefab;
		public PositionType position;
		public RotationType rotation;

		[NonSerialized]
		public Element element;
	}
}
