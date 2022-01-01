using UnityEngine;

namespace BoM.Cameras {
	// Data
	public class CenterData : MonoBehaviour {
		[SerializeField] protected float MouseSensitivity = 0.05f;
		[SerializeField] protected float GamepadSensitivity = 150f;
		[SerializeField] protected float MinClampRotationX;
		[SerializeField] protected float MaxClampRotationX;

		protected Vector2 angles;
		protected Vector2 mouse;
		protected Vector2 gamepad;
	}

	// Logic
	public class Center : CenterData {
		private void Update() {
			if(mouse != Vector2.zero) {
				angles.x -= mouse.y * MouseSensitivity;
				angles.y += mouse.x * MouseSensitivity;
			}

			if(gamepad != Vector2.zero) {
				angles.x -= gamepad.y * GamepadSensitivity * Time.deltaTime;
				angles.y += gamepad.x * GamepadSensitivity * Time.deltaTime;
			}

			angles.x = Mathf.Clamp(angles.x, MinClampRotationX, MaxClampRotationX);
			transform.eulerAngles = angles;
		}

		public void MouseLook(Vector2 input) {
			mouse = input;
		}

		public void GamepadLook(Vector2 input) {
			gamepad = input;
		}

		public void SetRotation(Quaternion rotation) {
			angles.x = rotation.eulerAngles.x;
			angles.y = rotation.eulerAngles.y;
		}
	}
}
