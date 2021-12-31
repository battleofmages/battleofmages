using UnityEngine;

namespace BoM.Players {
	// Data
	public class AnimationsData : MonoBehaviour {
		public Animator Animator;

		[SerializeField] protected Player player;
		[SerializeField] protected Gravity gravity;
		[SerializeField] protected Health health;

		protected const float ikSpeed = 20f;
		protected float ikWeight;
		protected float ikWeightTarget;

		protected readonly int idle = UnityEngine.Animator.StringToHash("Movement.Idle");
		protected readonly int land = UnityEngine.Animator.StringToHash("Movement.Land");
	}

	// Logic
	public class Animations : AnimationsData {
		private void Update() {
			var groundSpeed = player.Controller.velocity;
			groundSpeed.y = 0f;

			Animator.SetFloat("Speed", groundSpeed.sqrMagnitude);
			Animator.SetFloat("Gravity", gravity.Speed);
			Animator.SetBool("Grounded", player.Controller.isGrounded);
			Animator.SetBool("Dead", health.isDead);

			UpdateIK();
		}

		private void UpdateIK() {
			var current = Animator.GetCurrentAnimatorStateInfo(0);
			var next = Animator.GetNextAnimatorStateInfo(0);

			if((current.fullPathHash == idle || current.fullPathHash == land) && !Animator.IsInTransition(0)) {
				ikWeightTarget = 1f;
			} else if(next.fullPathHash == idle || next.fullPathHash == land) {
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
