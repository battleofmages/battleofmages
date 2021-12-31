using BoM.Core;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;

namespace BoM.Players {
	// Data
	public class ServerSendPositionData : NetworkBehaviour {
		public Player player;
		public ProxyMovement movement;
		protected CustomMessagingManager messenger;
		protected Vector3 lastPositionSent;
		protected Vector3 lastDirectionSent;
		protected FastBufferWriter writer;
	}

	// Logic
	public class ServerSendPosition : ServerSendPositionData {
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
