using UnityEngine;
using Unity.Netcode;

namespace BoM.Players {
	public class OwnerMovement : NetworkBehaviour, IController {
		public Player player;
		public Movement movement;
		public Vector3 direction { get; set; }
		public Vector3 inputDirection { get; set; }

		private void FixedUpdate() {
			direction = player.cam.transform.TransformDirection(inputDirection);
			direction = new Vector3(direction.x, 0f, direction.z);

			movement.Move(direction);
		}
	}
}
