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

		public int enemyTeamLayerMask {
			get {
				int layerMask = 0;

				foreach(var team in Manager.Teams) {
					if(team == this) {
						continue;
					}

					layerMask |= 1 << team.layer;
				}

				return layerMask;
			}
		}
	}
}
