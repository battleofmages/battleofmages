using BoM.Core;
using System.Collections.Generic;
using UnityEngine;

namespace BoM.Network {
	[CreateAssetMenu(fileName = "Match", menuName = "BoM/Match", order = 80)]
	public class Match : ScriptableObject {
		[SerializeField] private List<ListWrapper<string>> teams;

		public int GetTeamIdByAccountId(string id) {
			for(int teamId = 0; teamId < teams.Count; teamId++) {
				foreach(var accountId in teams[teamId]) {
					if(accountId == id) {
						return teamId;
					}
				}
			}

			return -1;
		}
	}
}
