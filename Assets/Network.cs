using UnityEngine;
using Unity.Netcode;

public class Network : MonoBehaviour {
	public Transform spawn;

	private void Start() {
		NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
	}

	private void ApprovalCheck(byte[] connectionData, ulong clientId, NetworkManager.ConnectionApprovedDelegate callback) {
		var approve = true;
		var offset = Random.insideUnitCircle * spawn.GetComponent<SphereCollider>().radius;
		var position = new Vector3(spawn.position.x + offset.x, spawn.position.y, spawn.position.z + offset.y);

		callback(approve, null, approve, position, Quaternion.identity);
	}

	public void StartClient() {
		NetworkManager.Singleton.StartClient();
		Init();
	}

	public void StartServer() {
		NetworkManager.Singleton.StartServer();
		Init();
	}

	public void StartHost() {
		NetworkManager.Singleton.StartHost();
		Init();
	}

	public void Init() {
		NetworkManager.Singleton.CustomMessagingManager.RegisterNamedMessageHandler("pos", (senderClientId, reader) => {
			var player = Player.ByClientId(senderClientId);
			reader.ReadValueSafe(out Vector3 position);
			player.OnPositionReceived(position);
		});
	}

	public void Disconnect() {
		NetworkManager.Singleton.Shutdown();
	}
}
