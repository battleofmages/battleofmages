using System;
using System.Collections.Generic;
using UnityEngine;

namespace BoM.Players {
	[Serializable]
	public class Team {
		public string name;
		[SerializeField, Core.Layer] public int layer;
		[NonSerialized] public List<Player> players;
	}
}
