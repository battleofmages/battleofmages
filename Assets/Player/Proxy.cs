using UnityEngine;
using Unity.Netcode;

public class Proxy : NetworkBehaviour {
	public Player player;
	public Camera cam;

	private void OnEnable() {
		CameraManager.AddCamera(cam);
	}

	private void OnDisable() {
		CameraManager.RemoveCamera(cam);
	}

	private void FixedUpdate() {
		if(transform.position != player.Position) {
			var direction = new Vector3(player.Position.x, 0, player.Position.z) - new Vector3(transform.position.x, 0, transform.position.z);
			player.Move(direction);
		}
	}

#region RPC
	[ClientRpc]
	public void NewPositionClientRpc(Vector3 position) {
		Debug.Log($"Proxy {player.Name} received new position");
		transform.position = position;
	}
#endregion
}