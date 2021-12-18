using UnityEngine;

namespace BoM.Player {
	public class Gravity : MonoBehaviour {
		public CharacterController Controller;
		public float JumpHeight;
		public float SpeedWhenGrounded = -2f;
		public float AllowJumpTime = 0.1f;
		public float AllowJumpCloseToGroundTime = 0.2f;
		public float MaxGroundDistance = 0.3f;
		public float Speed { get; private set; }

		private float jumpSpeed;
		private float notGroundedTime;
		private float originalStepOffset;

		public bool CanJump {
			get {
				if(notGroundedTime < AllowJumpTime) {
					return true;
				}

				if(notGroundedTime < AllowJumpCloseToGroundTime && Physics.Raycast(transform.position, Vector3.down, MaxGroundDistance)) {
					return true;
				}

				return false;
			}
		}

		private void Start() {
			jumpSpeed = Mathf.Sqrt(JumpHeight * 2 * -Physics.gravity.y);
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

		public bool Jump() {
			if(!CanJump) {
				return false;
			}

			Speed = jumpSpeed;
			return true;
		}
	}
}
