using UnityEngine;
using Unity.Netcode;
using Unity.Collections;

public class Server: NetworkBehaviour {
	public Player player;
	public Client client;
	public Proxy proxy;
	private Vector3 lastPositionSent;

	private void OnEnable() {
		
	}

	private void OnDisable() {
		
	}

	private void FixedUpdate() {
		if(transform.position != player.Position) {
			var direction = new Vector3(player.Position.x, 0, player.Position.z) - new Vector3(transform.position.x, 0, transform.position.z);
			player.Move(direction);
		}

		if(transform.position != lastPositionSent) {
			BroadcastPosition();
			lastPositionSent = transform.position;
		}
	}

	public void BroadcastPosition() {
		using FastBufferWriter writer = new FastBufferWriter(20, Allocator.Temp);
		writer.WriteValueSafe(player.ClientId);
		writer.WriteValueSafe(player.Position);
		NetworkManager.Singleton.CustomMessagingManager.SendNamedMessageToAll("server position", writer, NetworkDelivery.UnreliableSequenced);
	}

	public void PositionRequest(Vector3 newPosition) {
		transform.position = newPosition;

		// Share position to other clients
		foreach(var otherPlayer in PlayerManager.All) {
			if(otherPlayer.ClientId == player.ClientId) {
				continue;
			}

			using FastBufferWriter writer = new FastBufferWriter(20, Allocator.Temp);
			writer.WriteValueSafe(player.ClientId);
			writer.WriteValueSafe(transform.position);
			//messagingManager.SendNamedMessage("position confirmed", player.ClientId, writer, NetworkDelivery.UnreliableSequenced);
		}
	}

#region RPC
	[ServerRpc]
	public void NewMessageServerRpc(string message) {
		client.NewMessageClientRpc(message);
	}

	[ServerRpc]
	public void NewPositionServerRpc(Vector3 position) {
		transform.position = position;
		proxy.NewPositionClientRpc(position);
	}
#endregion
}