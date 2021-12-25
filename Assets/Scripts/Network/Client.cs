using UnityEngine;
using Unity.Netcode;

namespace BoM.Network {
	[CreateAssetMenu(fileName="Client", menuName="BoM/Client", order=50)]
	public class Client : ScriptableObject {
		public string accountId;

		public void Start() {
			NetworkManager.Singleton.NetworkConfig.ConnectionData = System.Text.Encoding.UTF8.GetBytes(accountId);
			NetworkManager.Singleton.StartClient();
			Listen();
		}

		public void Listen() {
			NetworkManager.Singleton.CustomMessagingManager.RegisterNamedMessageHandler("server position", ServerPosition);
		}

		public void ServerPosition(ulong senderClientId, FastBufferReader reader) {
			reader.ReadValueSafe(out ulong clientId);
			reader.ReadValueSafe(out Vector3 position);
			reader.ReadValueSafe(out Vector3 direction);

			var player = PlayerManager.GetByClientId(clientId);

			if(player == null) {
				return;
			}

			player.RemotePosition = position;
			player.RemoteDirection = direction;
		}
	}
}
