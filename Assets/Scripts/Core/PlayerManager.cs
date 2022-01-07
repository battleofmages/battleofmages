using System.Collections.Generic;

namespace BoM.Core {
	public static class PlayerManager {
		public static List<IPlayer> All { get => players; }

		private static readonly List<IPlayer> players = new List<IPlayer>();
		private static readonly Dictionary<ulong, IPlayer> clientIdToPlayer = new Dictionary<ulong, IPlayer>();

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
