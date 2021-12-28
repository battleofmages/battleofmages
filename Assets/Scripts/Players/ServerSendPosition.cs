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
			if(transform.position == lastPositionSent && player.RemoteDirection == lastDirectionSent) {
				return;
			}

			BroadcastPosition();

			lastPositionSent = transform.position;
			lastDirectionSent = player.RemoteDirection;
		}

		private void BroadcastPosition() {
			writer.Seek(0);
			writer.WriteValueSafe(player.ClientId);
			writer.WriteValueSafe(transform.position);
			writer.WriteValueSafe(player.RemoteDirection);

			var delivery = NetworkDelivery.Unreliable;

			if(player.RemoteDirection == Const.ZeroVector) {
				delivery = NetworkDelivery.Reliable;
			}

			messenger.SendNamedMessageToAll("server position", writer, delivery);
		}
	}
}
