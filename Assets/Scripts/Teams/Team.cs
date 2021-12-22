using BoM.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BoM.Teams {
	[Serializable]
	public class Team {
		public string name;
		[SerializeField, Layer] public int layer;
		[NonSerialized] public List<IPlayer> players;
	}
}
