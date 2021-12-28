using BoM.Core;
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
		private FastBufferWriter writer;

		private void OnEnable() {
			writer = new FastBufferWriter(24, Allocator.Persistent);
		}

		private void OnDisable() {
			writer.Dispose();
		}

		public override void OnNetworkSpawn() {
			serverId = NetworkManager.Singleton.ServerClientId;
			messenger = NetworkManager.Singleton.CustomMessagingManager;
		}

		private void FixedUpdate() {
			if(IsHost) {
				player.RemotePosition = transform.localPosition;
				player.RemoteDirection = movement.direction;
				return;
			}

			if(messenger == null) {
				return;
			}

			if(transform.localPosition == lastPositionSent && movement.direction == lastDirectionSent) {
				return;
			}

			SendPosition(serverId);

			lastPositionSent = transform.localPosition;
			lastDirectionSent = movement.direction;
		}

		private void SendPosition(ulong receiver) {
			writer.Seek(0);
			writer.WriteValueSafe(transform.localPosition);
			writer.WriteValueSafe(movement.direction);

			var delivery = NetworkDelivery.Unreliable;

			if(movement.direction == Const.ZeroVector) {
				delivery = NetworkDelivery.Reliable;
			}

			messenger.SendNamedMessage("client position", receiver, writer, delivery);
		}
	}
}
