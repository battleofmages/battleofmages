using BoM.Core;
using UnityEngine;
using Unity.Netcode;

namespace BoM.Players {
	public class ProxyMovement : NetworkBehaviour, IController {
		public Player player;
		public Movement movement;
		public long maxLatency;
		public Vector3 direction { get; set; }
		public Health health;
		private Vector3 lastRemoteDirection;

		private void FixedUpdate() {
			var latency = GetLatency();
			UpdateDirection(latency);
			movement.Move(direction);
		}

		private void UpdateDirection(float latency) {
			if(health.isDead) {
				direction = Const.ZeroVector;
				return;
			}

			var expectedPosition = CalculatePosition(player.RemotePosition, player.RemoteDirection, latency);
			direction = expectedPosition - transform.localPosition;

			if(StartedMoving()) {
				player.controller.Move(direction);
			}

			if(direction.sqrMagnitude < 0.01f) {
				direction = Const.ZeroVector;
			}
		}

		public Vector3 CalculatePosition(Vector3 position, Vector3 direction, float latency) {
			return position + direction * movement.speed * latency;
		}

		private bool StartedMoving() {
			bool startedMoving = (lastRemoteDirection == Const.ZeroVector && player.RemoteDirection != Const.ZeroVector);
			lastRemoteDirection = player.RemoteDirection;
			return startedMoving;
		}

		private float GetLatency() {
			float latency = 0f;

			if(IsServer) {
				latency = player.Latency.oneWay;
			}

			if(IsClient && Player.Main != null) {
				latency = Player.Main.Latency.oneWay;
			}

			if(latency > maxLatency) {
				return maxLatency;
			}

			return latency;
		}
	}
}
