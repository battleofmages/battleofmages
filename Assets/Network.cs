using UnityEngine;
using Unity.Netcode;

public class Network : MonoBehaviour {
	public Transform spawn;

	private void ApprovalCheck(byte[] connectionData, ulong clientId, NetworkManager.ConnectionApprovedDelegate callback) {
		var approve = true;
		var offset = Random.insideUnitCircle * spawn.GetComponent<SphereCollider>().radius;
		var position = new Vector3(spawn.position.x + offset.x, spawn.position.y, spawn.position.z + offset.y);

		callback(approve, null, approve, position, Quaternion.identity);
	}

	public void StartClient() {
		NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
		NetworkManager.Singleton.StartClient();
		Init();
	}

	public void StartServer() {
		NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
		NetworkManager.Singleton.StartServer();
		Init();
	}

	public void StartHost() {
		NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
		NetworkManager.Singleton.StartHost();
		Init();
	}

	public void Init() {
		NetworkManager.Singleton.CustomMessagingManager.RegisterNamedMessageHandler("position request", (senderClientId, reader) => {
			reader.ReadValueSafe(out Vector3 position);
			Player.ByClientId(senderClientId).PositionRequest(position);
		});

		NetworkManager.Singleton.CustomMessagingManager.RegisterNamedMessageHandler("position confirmed", (senderClientId, reader) => {
			reader.ReadValueSafe(out ulong clientId);
			reader.ReadValueSafe(out Vector3 position);
			Player.ByClientId(clientId).PositionConfirmed(position);
		});
	}

	public void Disconnect() {
		NetworkManager.Singleton.Shutdown();
	}
}
