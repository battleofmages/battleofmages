using UnityEngine;

namespace BoM.Players {
	// Data
	public class MovementData : MonoBehaviour {
		public CharacterController Controller;
		public float BaseSpeed;
		public float Speed { get; set; }

		[SerializeField] protected Gravity gravity;
		[SerializeField] protected Flight flight;
	}

	// Logic
	public class Movement : MovementData {
		private void Awake() {
			Speed = BaseSpeed;
		}

		public void Move(Vector3 direction) {
			if(!flight.enabled) {
				direction.y = 0f;
			}

			direction.Normalize();
			direction *= Speed;

			if(!flight.enabled) {
				direction.y = gravity.Speed;
			}

			Controller.Move(direction * Time.deltaTime);
		}
	}
}
