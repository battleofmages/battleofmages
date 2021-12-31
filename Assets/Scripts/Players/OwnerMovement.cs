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

		public Vector3 Direction { get; protected set; }
		public Vector3 inputDirection { get; set; }
	}

	// Logic
	public class OwnerMovement : OwnerMovementData, IController {
		private void FixedUpdate() {
			UpdateDirection();
			movement.Move(Direction);
		}

		private void UpdateDirection() {
			if(health.isDead) {
				Direction = Const.ZeroVector;
				return;
			}

			if(flight.enabled) {
				Direction = (cursor.FarPoint - transform.position).normalized + cam.transform.TransformDirection(inputDirection) * 0.5f;
			} else {
				Direction = cam.transform.TransformDirection(inputDirection);
				Direction.Set(Direction.x, 0f, Direction.z);
			}
		}
	}
}
