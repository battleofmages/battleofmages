using UnityEngine;
using UnityEngine.InputSystem;

namespace BoM.UI {
	// Data
	public class ManagerData : MonoBehaviour {
		[SerializeField] protected PlayerInput playerInput;
		[SerializeField] protected Chat chat;
	}

	// Logic
	public class Manager : ManagerData {
		public static PlayerInput PlayerInput { get => instance.playerInput; }
		public static Chat Chat { get => instance.chat; }
		private static Manager instance;

		private void Awake() {
			instance = this;
		}

		public static void Activate() {
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;
			PlayerInput.SwitchCurrentActionMap("UI");
			var inputFieldCanvas = Chat.InputField.GetComponent<CanvasGroup>();

			inputFieldCanvas.FadeIn(
				UI.Settings.FadeDuration,
				value => inputFieldCanvas.alpha = value
			);

			Chat.InputField.onEndEdit.AddListener(OnEndEdit);
		}

		public static void Deactivate() {
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;
			PlayerInput.SwitchCurrentActionMap("Player");
			Chat.InputField.DeactivateInputField();
			var inputFieldCanvas = Chat.InputField.GetComponent<CanvasGroup>();

			inputFieldCanvas.FadeOut(
				UI.Settings.FadeDuration,
				value => inputFieldCanvas.alpha = value
			);

			Chat.InputField.onEndEdit.RemoveListener(OnEndEdit);
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
			Chat.InputField.Select();
			Chat.InputField.ActivateInputField();
		}

		public static void OnEndEdit(string text) {
			Deactivate();
		}
	}
}
