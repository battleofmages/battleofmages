using UnityEngine;

namespace BoM.Players {
	// Data
	public class MovementData : MonoBehaviour {
		public CharacterController controller;
		public Gravity gravity;
		public Flight flight;
		public float baseSpeed;
		public float speed { get; set; }
	}

	// Logic
	public class Movement : MovementData {
		private void Awake() {
			speed = baseSpeed;
		}

		public void Move(Vector3 direction) {
			if(!flight.enabled) {
				direction.y = 0f;
			}

			direction.Normalize();
			direction *= speed;

			if(!flight.enabled) {
				direction.y = gravity.Speed;
			}

			controller.Move(direction * Time.deltaTime);
		}
	}
}
