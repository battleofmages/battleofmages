using UnityEngine;

namespace BoM.Cameras {
	public class Center : MonoBehaviour {
		public float mouseSensitivity = 0.05f;
		public float gamepadSensitivity = 150f;
		public float minClampRotationX;
		public float maxClampRotationX;
		
		private Vector2 angles;
		private Vector2 mouse;
		private Vector2 gamepad;

		void Update() {
			if(mouse != Vector2.zero) {
				angles.x -= mouse.y * mouseSensitivity;
				angles.y += mouse.x * mouseSensitivity;
			}
			
			if(gamepad != Vector2.zero) {
				angles.x -= gamepad.y * gamepadSensitivity * Time.deltaTime;
				angles.y += gamepad.x * gamepadSensitivity * Time.deltaTime;
			}

			angles.x = Mathf.Clamp(angles.x, minClampRotationX, maxClampRotationX);
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
