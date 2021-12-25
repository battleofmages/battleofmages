using BoM.Core;
using UnityEngine;
using System.Collections.Generic;

namespace BoM.Teams {
	[CreateAssetMenu(fileName = "Teams", menuName = "BoM/Team Manager", order = 101)]
	public class Manager : ScriptableObject {
		public List<Team> teams;

		private void OnEnable() {
			byte count = 0;

			foreach(var team in teams) {
				team.Id = count;
				count++;
			}
		}

		public void FindSpawns() {
			foreach(var team in teams) {
				team.FindSpawn();
			}
		}

		public int GetEnemyTeamsLayerMask(Team allies) {
			int layerMask = 0;

			foreach(var team in teams) {
				if(team == allies) {
					continue;
				}

				layerMask |= 1 << team.layer;
			}

			return layerMask;
		}
	}
}
