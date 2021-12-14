using UnityEngine;
using Unity.Netcode;
using Unity.Collections;

public class Network : MonoBehaviour {
	public Transform spawn;

	private void Start() {
		NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
	}

	public void StartClient() {
		Run("client");
	}

	public void StartServer() {
		Run("server");
	}

	public void StartHost() {
		Run("host");
	}

	public void Disconnect() {
		NetworkManager.Singleton.Shutdown();
	}

	private void ApprovalCheck(byte[] connectionData, ulong clientId, NetworkManager.ConnectionApprovedDelegate callback) {
		var approve = true;
		var offset = Random.insideUnitCircle * spawn.GetComponent<SphereCollider>().radius;
		var position = new Vector3(spawn.position.x + offset.x, spawn.position.y, spawn.position.z + offset.y);

		callback(approve, null, approve, position, Quaternion.identity);
	}

	private void Run(string type) {
		switch(type) {
			case "client":
				NetworkManager.Singleton.StartClient();
				break;

			case "server":
				NetworkManager.Singleton.StartServer();
				break;

			case "host":
				NetworkManager.Singleton.StartHost();
				break;
		}

		if(NetworkManager.Singleton.IsClient) {
			ClientReceiveMessages();
		}
		
		if(NetworkManager.Singleton.IsServer) {
			ServerReceiveMessages();
		}
	}

	private void ClientReceiveMessages() {
		NetworkManager.Singleton.CustomMessagingManager.RegisterNamedMessageHandler("server position", (senderClientId, reader) => {
			reader.ReadValueSafe(out ulong clientId);
			reader.ReadValueSafe(out Vector3 position);
			reader.ReadValueSafe(out Vector3 direction);

			var player = PlayerManager.FindClientId(clientId);

			if(player == null) {
				return;
			}

			player.Position = position;
			player.Direction = direction;
		});
	}

	private void ServerReceiveMessages() {
		NetworkManager.Singleton.CustomMessagingManager.RegisterNamedMessageHandler("client position", (senderClientId, reader) => {
			var sender = PlayerManager.FindClientId(senderClientId);

			if(sender == null) {
				return;
			}

			reader.ReadValueSafe(out Vector3 position);
			reader.ReadValueSafe(out Vector3 direction);
			sender.Position = position;
			sender.Direction = direction;
		});
	}
}