using BoM.Core;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;

namespace BoM.Players {
	public class ServerSendPosition : NetworkBehaviour {
		public Player player;
		public ProxyMovement movement;
		private CustomMessagingManager messenger;
		private Vector3 lastPositionSent;
		private Vector3 lastDirectionSent;
		private FastBufferWriter writer;

		private void OnEnable() {
			messenger = NetworkManager.Singleton.CustomMessagingManager;
			writer = new FastBufferWriter(32, Allocator.Persistent);
		}

		private void OnDisable() {
			writer.Dispose();
		}

		private void FixedUpdate() {
			BroadcastPosition();
		}

		public void BroadcastPosition() {
			if(transform.localPosition == lastPositionSent && movement.direction == lastDirectionSent) {
				return;
			}

			writer.Seek(0);
			writer.WriteValueSafe(player.ClientId);
			writer.WriteValueSafe(transform.localPosition);
			writer.WriteValueSafe(player.RemoteDirection);

			var delivery = NetworkDelivery.Unreliable;

			if(player.RemoteDirection == Const.ZeroVector) {
				delivery = NetworkDelivery.Reliable;
			}

			messenger.SendNamedMessageToAll("server position", writer, delivery);

			lastPositionSent = transform.localPosition;
			lastDirectionSent = movement.direction;
		}
	}
}
