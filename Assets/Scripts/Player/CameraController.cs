using UnityEngine;

namespace BoM.Player {
	public class CameraController : MonoBehaviour {
		public float mouseSensitivity = 0.05f;
		public float gamepadSensitivity = 100f;
		
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
			
			transform.eulerAngles = angles;
		}

		public void MouseLook(Vector2 input) {
			mouse = input;
		}

		public void GamepadLook(Vector2 input) {
			gamepad = input;
		}
	}
}
