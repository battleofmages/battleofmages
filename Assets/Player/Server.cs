using UnityEngine;
using Unity.Netcode;
using Unity.Collections;

public class Server: NetworkBehaviour {
	public Player player;
	public Client client;
	public Proxy proxy;
	public float movementPrediction;
	private Vector3 lastPositionSent;
	private Vector3 lastDirectionSent;

	private void FixedUpdate() {
		var direction = player.Position - transform.position;
		var prediction = player.Direction * movementPrediction;
		player.Move(direction + prediction);

		if(player.Position != lastPositionSent || player.Direction != lastDirectionSent) {
			BroadcastPosition();
			lastPositionSent = player.Position;
			lastDirectionSent = player.Direction;
		}
	}

	public void BroadcastPosition() {
		using FastBufferWriter writer = new FastBufferWriter(32, Allocator.Temp);
		writer.WriteValueSafe(player.ClientId);
		writer.WriteValueSafe(player.Position);
		writer.WriteValueSafe(player.Direction);

		var delivery = NetworkDelivery.UnreliableSequenced;

		if(player.Direction == Vector3.zero) {
			delivery = NetworkDelivery.ReliableSequenced;
		}

		NetworkManager.Singleton.CustomMessagingManager.SendNamedMessageToAll("server position", writer, delivery);
	}

#region RPC
	[ServerRpc]
	public void NewMessageServerRpc(string message) {
		client.NewMessageClientRpc(message);
	}
#endregion
}