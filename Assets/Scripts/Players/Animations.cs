using UnityEngine;

namespace BoM.Players {
	public class Animations : MonoBehaviour {
		public Animator Animator;
		public Player Player;

		private void Update() {
			var groundSpeed = Player.controller.velocity;
			groundSpeed.y = 0f;

			Animator.SetFloat("Speed", groundSpeed.sqrMagnitude);
			Animator.SetFloat("Gravity", Player.gravity.Speed);
			Animator.SetBool("Grounded", Player.controller.isGrounded);
		}

		// Input actions will trigger these animations, so we reset them to false
		// in the LateUpdate function. If they were in Update instead, the animations
		// would never be triggered.
		private void LateUpdate() {
			Animator.SetBool("Attack", false);
		}
	}
}
