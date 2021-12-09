using UnityEngine;
using Unity.Netcode;

public class Network : MonoBehaviour {
	public Transform spawn;

	private void Start() {
		NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
		Game.Chat.NewMessage += message => {
			if(message == "/dc") {
				Disconnect();
			}
		};
	}

	private void ApprovalCheck(byte[] connectionData, ulong clientId, NetworkManager.ConnectionApprovedDelegate callback) {
		var approve = true;
		var offset = Random.insideUnitCircle * spawn.GetComponent<SphereCollider>().radius;
		var position = new Vector3(spawn.position.x + offset.x, spawn.position.y, spawn.position.z + offset.y);

		callback(approve, null, approve, position, Quaternion.identity);
	}

	public void StartClient() {
		NetworkManager.Singleton.StartClient();
	}

	public void StartServer() {
		NetworkManager.Singleton.StartServer();
	}

	public void StartHost() {
		NetworkManager.Singleton.StartHost();
	}

	public void Disconnect() {
		NetworkManager.Singleton.Shutdown();
	}
}
