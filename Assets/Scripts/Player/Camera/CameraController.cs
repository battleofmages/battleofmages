using UnityEngine;

namespace BoM {
	public class CameraController : MonoBehaviour {
		public Vector2 angles;
		private Vector2 mouse;
		private Vector2 gamepad;

		void Update() {
			if(mouse != Vector2.zero) {
				angles.x -= mouse.y * Game.Config.mouseSensitivity;
				angles.y += mouse.x * Game.Config.mouseSensitivity;
			}
			
			if(gamepad != Vector2.zero) {
				angles.x -= gamepad.y * Game.Config.gamepadSensitivity * Time.deltaTime;
				angles.y += gamepad.x * Game.Config.gamepadSensitivity * Time.deltaTime;
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
