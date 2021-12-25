using UnityEngine;
using Unity.Netcode;
using Unity.Collections;

namespace BoM.Players {
	public class ServerSendPosition: NetworkBehaviour {
		public Player player;
		public ProxyMovement movement;
		private CustomMessagingManager messenger;
		private Vector3 lastPositionSent;
		private Vector3 lastDirectionSent;

		public void OnEnable() {
			messenger = NetworkManager.Singleton.CustomMessagingManager;
		}

		private void FixedUpdate() {
			BroadcastPosition();
		}

		public void BroadcastPosition() {
			if(transform.localPosition == lastPositionSent && movement.direction == lastDirectionSent) {
				return;
			}

			using FastBufferWriter writer = new FastBufferWriter(32, Allocator.Temp);
			writer.WriteValueSafe(player.ClientId);
			writer.WriteValueSafe(transform.localPosition);
			writer.WriteValueSafe(movement.direction);

			var delivery = NetworkDelivery.Unreliable;

			if(movement.direction == Vector3.zero) {
				delivery = NetworkDelivery.Reliable;
			}

			messenger.SendNamedMessageToAll("server position", writer, delivery);

			lastPositionSent = transform.localPosition;
			lastDirectionSent = movement.direction;
		}
	}
}
