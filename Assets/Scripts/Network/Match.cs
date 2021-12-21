using System.Collections.Generic;

namespace BoM.Network {
	public class Match {
		public List<List<string>> teams;

		public Match() {
			teams = new List<List<string>>();
		}

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
