using UnityEngine;
using UnityEngine.InputSystem;

namespace BoM.UI {
	public class Manager : MonoBehaviour {
		public static Manager Instance { get; private set; }
		
		public PlayerInput playerInput;
		public Chat chat;

		public static PlayerInput PlayerInput { get{ return Instance.playerInput; } }
		public static Chat Chat { get{ return Instance.chat; } }

		private void Awake() {
			Instance = this;
		}

		public static void Activate() {
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;
			PlayerInput.SwitchCurrentActionMap("UI");
			var inputFieldCanvas = Chat.inputField.GetComponent<CanvasGroup>();

			inputFieldCanvas.FadeIn(
				0.15f,
				value => inputFieldCanvas.alpha = value
			);
		}

		public static void Deactivate() {
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;
			PlayerInput.SwitchCurrentActionMap("Player");
			Chat.inputField.DeactivateInputField();
			var inputFieldCanvas = Chat.inputField.GetComponent<CanvasGroup>();

			inputFieldCanvas.FadeOut(
				0.15f,
				value => inputFieldCanvas.alpha = value
			);
		}

		// Overloads to use it in input actions
		public static void Activate(InputAction.CallbackContext context) {
			Activate();
		}

		public static void Deactivate(InputAction.CallbackContext context) {
			Deactivate();
		}

		public static void ActivateAndSelectChat(InputAction.CallbackContext context) {
			Activate();
			Chat.inputField.Select();
			Chat.inputField.ActivateInputField();
		}
	}
}
