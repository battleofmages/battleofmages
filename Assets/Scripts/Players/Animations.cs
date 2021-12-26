using UnityEngine;

namespace BoM.Players {
	public class Animations : MonoBehaviour {
		public Animator Animator;
		public Player Player;
		public Health health;
		private const float ikSpeed = 20f;
		private float ikWeight;
		private float ikWeightTarget;

		private void Update() {
			var groundSpeed = Player.controller.velocity;
			groundSpeed.y = 0f;

			Animator.SetFloat("Speed", groundSpeed.sqrMagnitude);
			Animator.SetFloat("Gravity", Player.gravity.Speed);
			Animator.SetBool("Grounded", Player.controller.isGrounded);
			Animator.SetBool("Dead", health.isDead);

			UpdateIK();
		}

		private void UpdateIK() {
			var current = Animator.GetCurrentAnimatorStateInfo(0);
			var next = Animator.GetNextAnimatorStateInfo(0);

			if((current.IsName("Idle") || current.IsName("Land")) && !Animator.IsInTransition(0)) {
				ikWeightTarget = 1f;
			} else if(next.IsName("Idle") || next.IsName("Land")) {
				ikWeightTarget = 1f;
			} else {
				ikWeightTarget = 0f;
			}

			ikWeight = Mathf.Lerp(ikWeight, ikWeightTarget, Time.deltaTime * ikSpeed);

			Animator.SetFloat("IKLeftFootWeight", ikWeight);
			Animator.SetFloat("IKRightFootWeight", ikWeight);
		}

		// Input actions will trigger these animations, so we reset them to false
		// in the LateUpdate function. If they were in Update instead, the animations
		// would never be triggered.
		private void LateUpdate() {
			Animator.SetBool("Attack", false);
		}
	}
}
