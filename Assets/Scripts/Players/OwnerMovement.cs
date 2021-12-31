using BoM.Core;
using UnityEngine;
using Unity.Netcode;

namespace BoM.Players {
	// Data
	public class OwnerMovementData : NetworkBehaviour {
		[SerializeField] protected Camera cam;
		[SerializeField] protected Cursor cursor;
		[SerializeField] protected Movement movement;
		[SerializeField] protected Flight flight;
		[SerializeField] protected Health health;

		public Vector3 direction { get; protected set; }
		public Vector3 inputDirection { get; set; }
	}

	// Logic
	public class OwnerMovement : OwnerMovementData, IController {
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
				direction = (cursor.FarPoint - transform.position).normalized + cam.transform.TransformDirection(inputDirection) * 0.5f;
			} else {
				direction = cam.transform.TransformDirection(inputDirection);
				direction.Set(direction.x, 0f, direction.z);
			}
		}
	}
}
