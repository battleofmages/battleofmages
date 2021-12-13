using UnityEngine;
using Unity.Netcode;
using Unity.Collections;

public class Server: NetworkBehaviour {
	public Player player;
	public Client client;
	public Proxy proxy;

	private Vector3 lastPositionSent;
	private Vector3 lastDirectionSent;

	private void FixedUpdate() {
		BroadcastPosition();
	}

	public void BroadcastPosition() {
		if(player.Position == lastPositionSent && player.Direction == lastDirectionSent) {
			return;
		}

		using FastBufferWriter writer = new FastBufferWriter(32, Allocator.Temp);
		writer.WriteValueSafe(player.ClientId);
		writer.WriteValueSafe(player.Position);
		writer.WriteValueSafe(player.Direction);

		var delivery = NetworkDelivery.UnreliableSequenced;

		if(player.Direction == Vector3.zero) {
			delivery = NetworkDelivery.Reliable;
		}

		NetworkManager.Singleton.CustomMessagingManager.SendNamedMessageToAll("server position", writer, delivery);

		lastPositionSent = player.Position;
		lastDirectionSent = player.Direction;
	}

#region RPC
	[ServerRpc]
	public void JumpServerRpc() {
		if(!player.Jump()) {
			return;
		}

		proxy.JumpClientRpc();
	}

	[ServerRpc]
	public void NewMessageServerRpc(string message) {
		client.NewMessageClientRpc(message);
	}
#endregion
}