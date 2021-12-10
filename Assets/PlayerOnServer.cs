using UnityEngine;
using Unity.Netcode;
using Unity.Collections;

public partial class Player : NetworkBehaviour {
	public void PositionRequest(Vector3 newPosition) {
		transform.position = newPosition;

		// Share position to other clients
		foreach(var player in players) {
			if(player.clientId == clientId) {
				continue;
			}

			using FastBufferWriter writer = new FastBufferWriter(20, Allocator.Temp);
			writer.WriteValueSafe(clientId);
			writer.WriteValueSafe(transform.position);
			messagingManager.SendNamedMessage("position confirmed", player.clientId, writer, NetworkDelivery.UnreliableSequenced);
		}
	}

	public void PositionConfirmed(Vector3 newPosition) {
		transform.position = newPosition;
	}
}
