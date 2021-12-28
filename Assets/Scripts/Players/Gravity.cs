using BoM.Core;
using UnityEngine;

namespace BoM.Players {
	public class Gravity : MonoBehaviour {
		public CharacterController Controller;
		public float SpeedWhenGrounded = -2f;
		public float AllowJumpTime = 0.1f;
		public float AllowJumpCloseToGroundTime = 0.2f;
		public float MaxGroundDistance = 0.3f;
		public float Speed { get; set; }

		private float notGroundedTime;
		private float originalStepOffset;

		public bool CanJump {
			get {
				if(notGroundedTime < AllowJumpTime) {
					return true;
				}

				if(notGroundedTime < AllowJumpCloseToGroundTime && Physics.Raycast(transform.localPosition, Const.DownVector, MaxGroundDistance)) {
					return true;
				}

				return false;
			}
		}

		private void Start() {
			originalStepOffset = Controller.stepOffset;
		}

		private void Update() {
			notGroundedTime += Time.deltaTime;

			if(Controller.isGrounded && Speed < 0f) {
				notGroundedTime = 0f;
				Speed = SpeedWhenGrounded;
				Controller.stepOffset = originalStepOffset;
			} else {
				Controller.stepOffset = 0f;
			}

			Speed += Physics.gravity.y * Time.deltaTime;
		}
	}
}
