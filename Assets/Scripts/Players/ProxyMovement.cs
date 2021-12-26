using UnityEngine;
using Unity.Netcode;

namespace BoM.Players {
	public class ProxyMovement : NetworkBehaviour, IController {
		public Player player;
		public Movement movement;
		public long maxLatency;
		public Vector3 direction { get; set; }
		private Vector3 lastRemoteDirection;

		private void FixedUpdate() {
			float latency = 0f;

			if(IsServer) {
				latency = player.latency.oneWay;
			}

			if(IsClient && Player.main != null) {
				latency = Player.main.latency.oneWay;
			}

			if(latency > maxLatency) {
				latency = maxLatency;
			}

			UpdatePosition(latency);
		}

		public void UpdatePosition(float latency) {
			var expectedPosition = CalculatePosition(player.RemotePosition, player.RemoteDirection, latency);

			if(StartedMoving()) {
				player.controller.Move(expectedPosition - transform.localPosition);
			}

			direction = expectedPosition - transform.localPosition;

			if(direction.sqrMagnitude < 0.01f) {
				direction = Vector3.zero;
			}

			movement.Move(direction);
		}

		public static Vector3 CalculatePosition(Vector3 position, Vector3 direction, float latency) {
			return position + direction * latency;
		}

		private bool StartedMoving() {
			bool startedMoving = (lastRemoteDirection == Vector3.zero && player.RemoteDirection != Vector3.zero);
			lastRemoteDirection = player.RemoteDirection;
			return startedMoving;
		}
	}
}
