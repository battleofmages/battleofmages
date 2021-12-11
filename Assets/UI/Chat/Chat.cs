using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections.Generic;

public delegate void NewMessageHandler(string message);

public class Chat : MonoBehaviour {
	public GameObject messagesContainer;
	public Scrollbar scrollBar;
	public TMP_InputField inputField;
	public InputActionAsset inputActions;
	public ChatHistory history;
	public event NewMessageHandler NewMessage;
	private TextMeshProUGUI[] messages;

	private static Dictionary<string, Color> channels = new Dictionary<string, Color>(){
		{"Global", Color.white},
		{"Announcement", Color.cyan},
		{"Map", new Color(1.0f, 0.85f, 0.6f, 1f)},
		{"System", new Color(1f, 1f, 0.5f, 1f)},
		{"Debug", new Color(1f, 1f, 1f, 1f)}
	};

	private void Start() {
		messages = messagesContainer.GetComponentsInChildren<TextMeshProUGUI>();
		Clear();
		scrollBar.value = 0f;
	}

	private void OnEnable() {
		Application.logMessageReceived += OnLog;
		inputActions.FindAction("Submit").performed += OnSubmit;
		inputActions.FindAction("Navigate").performed += history.OnNavigate;
	}

	private void OnDisable() {
		Application.logMessageReceived -= OnLog;
		inputActions.FindAction("Submit").performed -= OnSubmit;
		inputActions.FindAction("Navigate").performed -= history.OnNavigate;
	}

	public void Clear() {
		foreach(var message in messages) {
			message.text = "";
		}
	}

	public void Write(string channel, string message) {
		if(messages == null || messages.Length == 0 || messages[0] == null) {
			return;
		}

		messages[0].text = $"<alpha=#66>[{channel}] <alpha=#FF>{message}";
		messages[0].color = GetChannelColor(channel);
		messages[0].transform.SetAsLastSibling();

		// The last index becomes the latest message
		messages[messages.Length-1] = messages[0];

		// Update the remaining indices
		for(int i = 0; i < messages.Length-1; i++) {
			messages[i] = messages[i+1];
		}

		// Snap scrollbar to the latest message once the layout is updated on the next frame
		if(scrollBar.value < 0.01f) {
			StartCoroutine(ResetScrollBar());
		}
	}

	IEnumerator<int> ResetScrollBar() {
		yield return 0;
		yield return 0;
		scrollBar.value = 0f;
	}

	public void OnSubmit(InputAction.CallbackContext value) {
		if(!inputField.isFocused || inputField.text == "") {
			return;
		}

		NewMessage?.Invoke(inputField.text);
		history.Add(inputField.text);
		history.ScrollToStart();
		inputField.text = "";
	}

	private void OnLog(string message, string stack, LogType type) {
		Write("Debug", message);
	}

	public static Color GetChannelColor(string channel) {
		return channels[channel];
	}
}
