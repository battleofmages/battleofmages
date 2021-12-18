using System;
using UnityEngine;

namespace BoM.Skill {
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
