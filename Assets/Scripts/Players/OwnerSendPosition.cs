using UnityEngine;
using Unity.Netcode;
using Unity.Collections;

namespace BoM.Players {
	public class OwnerSendPosition : NetworkBehaviour {
		public Player player;
		public OwnerMovement movement;
		private CustomMessagingManager messenger;
		private ulong serverId;
		private Vector3 lastPositionSent;
		private Vector3 lastDirectionSent;

		public override void OnNetworkSpawn() {
			serverId = NetworkManager.Singleton.ServerClientId;
			messenger = NetworkManager.Singleton.CustomMessagingManager;
		}

		private void FixedUpdate() {
			SendPositionToServer();
		}

		private void SendPositionToServer() {
			if(IsHost) {
				player.RemotePosition = transform.position;
				player.RemoteDirection = movement.direction;
				return;
			}

			if(messenger == null) {
				return;
			}

			if(transform.position == lastPositionSent && movement.direction == lastDirectionSent) {
				return;
			}

			using FastBufferWriter writer = new FastBufferWriter(24, Allocator.Temp);
			writer.WriteValueSafe(transform.position);
			writer.WriteValueSafe(movement.direction);

			var delivery = NetworkDelivery.Unreliable;

			if(movement.direction == Vector3.zero) {
				delivery = NetworkDelivery.Reliable;
			}

			messenger.SendNamedMessage("client position", serverId, writer, delivery);

			lastPositionSent = transform.position;
			lastDirectionSent = movement.direction;
		}
	}
}