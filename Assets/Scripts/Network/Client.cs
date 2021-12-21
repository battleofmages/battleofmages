using UnityEngine;
using Unity.Netcode;

namespace BoM.Network {
	public static class Client {
		public static void Start(string accountId) {
			NetworkManager.Singleton.NetworkConfig.ConnectionData = System.Text.Encoding.UTF8.GetBytes(accountId);
			NetworkManager.Singleton.StartClient();
			Listen();
		}

		public static void Listen() {
			NetworkManager.Singleton.CustomMessagingManager.RegisterNamedMessageHandler("server position", ServerPosition);
		}

		public static void ServerPosition(ulong senderClientId, FastBufferReader reader) {
			reader.ReadValueSafe(out ulong clientId);
			reader.ReadValueSafe(out Vector3 position);
			reader.ReadValueSafe(out Vector3 direction);

			var player = PlayerManager.FindClientId(clientId);

			if(player == null) {
				return;
			}

			player.RemotePosition = position;
			player.RemoteDirection = direction;
		}
	}
}