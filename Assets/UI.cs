using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class UI : MonoBehaviour {
	public static void Activate() {
		Cursor.visible = true;
		Game.Input.SwitchCurrentActionMap("UI");
	}

	public static void Deactivate() {
		Cursor.visible = false;
		Game.Input.SwitchCurrentActionMap("Player");
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
		EventSystem.current.SetSelectedGameObject(Game.Chat.inputField.gameObject);
	}
}
