using BoM.Core;
using UnityEngine;

namespace BoM.Players {
	// Data
	public class GravityData : MonoBehaviour {
		public float Speed { get; set; }

		[SerializeField] protected CharacterController controller;
		[SerializeField] protected float speedWhenGrounded = -2f;
		[SerializeField] protected float allowJumpTime = 0.1f;
		[SerializeField] protected float allowJumpCloseToGroundTime = 0.2f;
		[SerializeField] protected float maxGroundDistance = 0.3f;

		protected float notGroundedTime;
		protected float originalStepOffset;
	}

	// Logic
	public class Gravity : GravityData {
		private void Start() {
			originalStepOffset = controller.stepOffset;
		}

		private void Update() {
			notGroundedTime += Time.deltaTime;

			if(controller.isGrounded && Speed < 0f) {
				notGroundedTime = 0f;
				Speed = speedWhenGrounded;
				controller.stepOffset = originalStepOffset;
			} else {
				controller.stepOffset = 0f;
			}

			Speed += Physics.gravity.y * Time.deltaTime;
		}

		public bool CanJump {
			get {
				if(notGroundedTime < allowJumpTime) {
					return true;
				}

				if(notGroundedTime < allowJumpCloseToGroundTime && Physics.Raycast(transform.localPosition, Const.DownVector, maxGroundDistance)) {
					return true;
				}

				return false;
			}
		}
	}
}
