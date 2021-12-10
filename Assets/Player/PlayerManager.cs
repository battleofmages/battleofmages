using System.Collections.Generic;

public static class PlayerManager {
	private static List<Player> players = new List<Player>();
	private static Dictionary<ulong, Player> clientIdToPlayer = new Dictionary<ulong, Player>();

	public static List<Player> All {
		get {
			return players;
		}
	}

	public static void Add(Player player) {
		players.Add(player);
		clientIdToPlayer.Add(player.ClientId, player);
	}

	public static void Remove(Player player) {
		players.Remove(player);
		clientIdToPlayer.Remove(player.ClientId);
	}

	public static Player FindClientId(ulong clientId) {
		if(clientIdToPlayer.TryGetValue(clientId, out Player player)) {
			return player;
		}

		return null;
	}
}
