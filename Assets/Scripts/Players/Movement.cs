using UnityEngine;

namespace BoM.Players {
	public class Movement : MonoBehaviour {
		public CharacterController controller;
		public Gravity gravity;
		public float speed;

		public void Move(Vector3 direction) {
			direction.y = 0f;
			direction.Normalize();

			direction *= speed;
			direction.y = gravity.Speed;

			controller.Move(direction * Time.deltaTime);
		}
	}
}
