using BoM.Core;
using UnityEngine;
using Unity.Netcode;

namespace BoM.Players {
	public class ProxyMovement : NetworkBehaviour, IController {
		public Player player;
		public Movement movement;
		public long maxLatency;
		public Vector3 direction { get; set; }
		private Vector3 lastRemoteDirection;
		private new Transform transform;

		private void Awake() {
			transform = base.transform;
		}

		private void FixedUpdate() {
			var latency = GetLatency();
			UpdatePosition(latency);
		}

		public void UpdatePosition(float latency) {
			var expectedPosition = CalculatePosition(player.RemotePosition, player.RemoteDirection, latency);
			direction = expectedPosition - transform.localPosition;

			if(StartedMoving()) {
				player.controller.Move(direction);
			}

			if(direction.sqrMagnitude < 0.01f) {
				direction = Const.ZeroVector;
			}

			movement.Move(direction);
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
				latency = player.latency.oneWay;
			}

			if(IsClient && Player.main != null) {
				latency = Player.main.latency.oneWay;
			}

			if(latency > maxLatency) {
				return maxLatency;
			}

			return latency;
		}
	}
}
