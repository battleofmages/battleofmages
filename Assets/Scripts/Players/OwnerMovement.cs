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

			direction = player.cam.transform.TransformDirection(inputDirection);

			if(!flight.enabled) {
				direction.Set(direction.x, 0f, direction.z);
			}
		}
	}
}
