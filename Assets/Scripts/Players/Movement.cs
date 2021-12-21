using UnityEngine;
using Unity.Netcode;

namespace BoM.Players {
	public class Movement : NetworkBehaviour {
		public Player player;
		public long maxLatency;
		private Vector3 lastRemoteDirection;

		private void FixedUpdate() {
			long latency = 0;

			if(IsServer) {
				latency = player.latency.oneWay;
			}

			if(IsClient && Player.main != null) {
				latency = Player.main.latency.oneWay;
			}

			if(latency > maxLatency) {
				latency = maxLatency;
			}

			UpdatePosition(latency * 0.001f);
		}

		public void UpdatePosition(float latency) {
			var expectedPosition = CalculatePosition(player.RemotePosition, player.RemoteDirection, latency);

			if(StartedMoving()) {
				player.controller.Move(expectedPosition - transform.position);
			}

			var towardsExpectedPosition = expectedPosition - transform.position;

			if(towardsExpectedPosition.sqrMagnitude < 0.01f) {
				towardsExpectedPosition = Vector3.zero;
			}

			player.Move(towardsExpectedPosition);
		}

		private Vector3 CalculatePosition(Vector3 position, Vector3 direction, float latency) {
			return position + direction * latency;
		}

		private bool StartedMoving() {
			bool startedMoving = (lastRemoteDirection == Vector3.zero && player.RemoteDirection != Vector3.zero);
			lastRemoteDirection = player.RemoteDirection;
			return startedMoving;
		}
	}
}