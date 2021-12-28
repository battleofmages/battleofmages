using UnityEngine;

namespace BoM.Cameras {
	public class Center : MonoBehaviour {
		[SerializeField] private float MouseSensitivity = 0.05f;
		[SerializeField] private float GamepadSensitivity = 150f;
		[SerializeField] private float MinClampRotationX;
		[SerializeField] private float MaxClampRotationX;

		private Vector2 angles;
		private Vector2 mouse;
		private Vector2 gamepad;

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
