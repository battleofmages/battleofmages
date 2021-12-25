using BoM.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BoM.Teams {
	[Serializable]
	public class Team {
		public byte Id { get; set; }
		public string name;
		[SerializeField, Layer] public int layer;
		[NonSerialized] public List<IPlayer> players;
		[NonSerialized] public Transform spawn;

		public int layerMask {
			get {
				return 1 << layer;
			}
		}
	}
}
