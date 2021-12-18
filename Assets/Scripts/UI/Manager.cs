using UnityEngine;
using UnityEngine.InputSystem;

namespace BoM.UI {
	public class Manager : MonoBehaviour {
		public static Manager Instance { get; private set; }
		
		public PlayerInput playerInput;
		public Chat.Chat chat;

		public static PlayerInput PlayerInput { get{ return Instance.playerInput; } }
		public static Chat.Chat Chat { get{ return Instance.chat; } }

		private void Awake() {
			Instance = this;
		}

		public static void Activate() {
			UnityEngine.Cursor.visible = true;
			UnityEngine.Cursor.lockState = CursorLockMode.None;
			PlayerInput.SwitchCurrentActionMap("UI");
			Chat.inputField.GetComponent<CanvasGroup>().alpha = 1f;
		}

		public static void Deactivate() {
			UnityEngine.Cursor.visible = false;
			UnityEngine.Cursor.lockState = CursorLockMode.Locked;
			PlayerInput.SwitchCurrentActionMap("Player");
			Chat.inputField.GetComponent<CanvasGroup>().alpha = 0.5f;
			Chat.inputField.DeactivateInputField();
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
