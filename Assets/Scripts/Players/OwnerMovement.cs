using BoM.Core;
using UnityEngine;
using Unity.Netcode;

namespace BoM.Players {
	public class OwnerMovement : NetworkBehaviour, IController {
		public Player player;
		public Movement movement;
		public Flight flight;
		public Vector3 direction { get; private set; }
		public Vector3 inputDirection { get; set; }
		public Health health;

		private void FixedUpdate() {
			UpdateDirection();
			movement.Move(direction);
		}

		private void UpdateDirection() {
			if(health.isDead) {
				direction = Const.ZeroVector;
				return;
			}

			if(flight.enabled) {
				direction = (player.cursor.FarPoint - transform.position).normalized + player.cam.transform.TransformDirection(inputDirection) * 0.5f;
			} else {
				direction = player.cam.transform.TransformDirection(inputDirection);
				direction.Set(direction.x, 0f, direction.z);
			}
		}
	}
}
