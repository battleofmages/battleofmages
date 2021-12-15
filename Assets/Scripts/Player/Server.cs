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
		if(player.RemotePosition == lastPositionSent && player.RemoteDirection == lastDirectionSent) {
			return;
		}

		using FastBufferWriter writer = new FastBufferWriter(32, Allocator.Temp);
		writer.WriteValueSafe(player.ClientId);
		writer.WriteValueSafe(player.RemotePosition);
		writer.WriteValueSafe(player.RemoteDirection);

		var delivery = NetworkDelivery.UnreliableSequenced;

		if(player.RemoteDirection == Vector3.zero) {
			delivery = NetworkDelivery.Reliable;
		}

		NetworkManager.Singleton.CustomMessagingManager.SendNamedMessageToAll("server position", writer, delivery);

		lastPositionSent = player.RemotePosition;
		lastDirectionSent = player.RemoteDirection;
	}

#region RPC
	[ServerRpc]
	public void JumpServerRpc() {
		if(!player.gravity.Jump()) {
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