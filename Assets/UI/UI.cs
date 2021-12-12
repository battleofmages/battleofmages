using UnityEngine;
using UnityEngine.InputSystem;

public class UI : MonoBehaviour {
	public static void Activate() {
		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.None;
		Game.Input.SwitchCurrentActionMap("UI");
		Game.Chat.inputField.GetComponent<CanvasGroup>().alpha = 1f;
	}

	public static void Deactivate() {
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
		Game.Input.SwitchCurrentActionMap("Player");
		Game.Chat.inputField.GetComponent<CanvasGroup>().alpha = 0.5f;
		Game.Chat.inputField.DeactivateInputField();
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
		Game.Chat.inputField.Select();
		Game.Chat.inputField.ActivateInputField();
	}
}
