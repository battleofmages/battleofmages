using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public delegate void NewMessageHandler(string message);

public class Chat : MonoBehaviour {
	public GameObject messagesContainer;
	public TMP_InputField inputField;
	public InputActionAsset inputActions;
	public event NewMessageHandler NewMessage;
	private TextMeshProUGUI[] messages;

	private void Start() {
		messages = messagesContainer.GetComponentsInChildren<TextMeshProUGUI>();
		Clear();
	}

	public void Clear() {
		foreach(var message in messages) {
			message.text = "";
		}
	}

	public void Write(string message) {
		messages[0].text = message;
		messages[0].transform.SetAsLastSibling();

		// The last index becomes the latest message
		messages[messages.Length-1] = messages[0];

		// Update the remaining indices
		for(int i = 0; i < messages.Length-1; i++) {
			messages[i] = messages[i+1];
		}
	}

	public void OnSubmit(InputAction.CallbackContext value) {
		if(!inputField.isFocused || inputField.text == "") {
			return;
		}

		NewMessage?.Invoke(inputField.text);
		inputField.text = "";
	}

	private void OnEnable() {
		Application.logMessageReceived += OnLog;
		inputActions.FindAction("Submit").performed += OnSubmit;
	}

	private void OnDisable() {
		Application.logMessageReceived -= OnLog;
		inputActions.FindAction("Submit").performed -= OnSubmit;
	}

	private void OnLog(string message, string stack, LogType type) {
		Write(message);
	}
}
