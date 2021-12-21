using BoM.Core;
using System.Collections.Generic;

namespace BoM.Network {
	public static class PlayerManager {
		private static List<IPlayer> players = new List<IPlayer>();
		private static Dictionary<ulong, IPlayer> clientIdToPlayer = new Dictionary<ulong, IPlayer>();

		public static List<IPlayer> All {
			get {
				return players;
			}
		}

		public static void AddPlayer(IPlayer player) {
			players.Add(player);
			clientIdToPlayer.Add(player.ClientId, player);
		}

		public static void RemovePlayer(IPlayer player) {
			players.Remove(player);
			clientIdToPlayer.Remove(player.ClientId);
		}

		public static IPlayer GetByClientId(ulong clientId) {
			if(clientIdToPlayer.TryGetValue(clientId, out IPlayer player)) {
				return player;
			}

			return null;
		}
	}
}
