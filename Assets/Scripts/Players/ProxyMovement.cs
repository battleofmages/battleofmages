using BoM.Core;
using UnityEngine;
using Unity.Netcode;

namespace BoM.Players {
	// Data
	public class ProxyMovementData : NetworkBehaviour {
		public Vector3 Direction { get; set; }

		[SerializeField] protected Player player;
		[SerializeField] protected Movement movement;
		[SerializeField] protected long maxLatency;
		[SerializeField] protected Health health;

		protected Vector3 lastRemoteDirection;
	}

	// Logic
	public class ProxyMovement : ProxyMovementData, IController {
		private void FixedUpdate() {
			var latency = GetLatency();
			UpdateDirection(latency);
			movement.Move(Direction);
		}

		private void UpdateDirection(float latency) {
			if(health.isDead) {
				Direction = Const.ZeroVector;
				return;
			}

			var expectedPosition = CalculatePosition(player.RemotePosition, player.RemoteDirection, movement.Speed, latency);
			Direction = expectedPosition - transform.localPosition;

			if(StartedMoving()) {
				player.Controller.Move(Direction);
			}

			if(Direction.sqrMagnitude < 0.01f) {
				Direction = Const.ZeroVector;
			}
		}

		public static Vector3 CalculatePosition(Vector3 position, Vector3 direction, float speed, float latency) {
			return position + direction * speed * latency;
		}

		private bool StartedMoving() {
			bool startedMoving = (lastRemoteDirection == Const.ZeroVector && player.RemoteDirection != Const.ZeroVector);
			lastRemoteDirection = player.RemoteDirection;
			return startedMoving;
		}

		private float GetLatency() {
			float latency = 0f;

			if(IsServer) {
				latency = player.Latency.OneWay;
			}

			if(IsClient && Player.Main != null) {
				latency = Player.Main.Latency.OneWay;
			}

			if(latency > maxLatency) {
				return maxLatency;
			}

			return latency;
		}
	}
}
