using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

namespace BoM.UI {
	// Data
	public class ChatData : MonoBehaviour {
		public TMP_InputField InputField;

		[SerializeField] protected GameObject messagesContainer;
		[SerializeField] protected Scrollbar scrollBar;
		[SerializeField] protected InputActionAsset inputActions;
		[SerializeField] protected ChatHistory history;
		protected TextMeshProUGUI[] messages;
	}

	// Logic
	public class Chat : ChatData {
		public static event Action<string> MessageReceived;
		public static event Action<string> MessageSubmitted;
		private static Dictionary<string, Color> channels;

		private void Start() {
			channels = new Dictionary<string, Color>(){
				{"Global", Color.white},
				{"Announcement", Color.cyan},
				{"Map", new Color(1.0f, 0.85f, 0.6f, 1f)},
				{"Combat", new Color(1f, 1f, 0.5f, 1f)},
				{"System", new Color(1f, 1f, 0.5f, 1f)},
				{"Debug", new Color(1f, 1f, 1f, 1f)}
			};

			messages = messagesContainer.GetComponentsInChildren<TextMeshProUGUI>();
			Clear();
			scrollBar.value = 0f;
		}

		private void OnEnable() {
			Application.logMessageReceived += OnDebugLog;
			inputActions.FindAction("Submit").performed += OnSubmit;
			inputActions.FindAction("Navigate").performed += history.OnNavigate;
		}

		private void OnDisable() {
			Application.logMessageReceived -= OnDebugLog;
			inputActions.FindAction("Submit").performed -= OnSubmit;
			inputActions.FindAction("Navigate").performed -= history.OnNavigate;
		}

		public void Clear() {
			foreach(var message in messages) {
				message.text = "";
			}
		}

		public void Write(string channel, string contents) {
			if(messages == null || messages.Length == 0 || messages[0] == null) {
				return;
			}

			var newMessage = messages[0];
			newMessage.text = $"<alpha=#66>[{channel}] <alpha=#FF>{contents}";
			newMessage.color = GetChannelColor(channel);
			newMessage.transform.SetAsLastSibling();

			newMessage.FadeIn(
				UI.Settings.FadeDuration,
				value => newMessage.alpha = value
			);

			// The last index becomes the latest message
			messages[messages.Length - 1] = newMessage;

			// Update the remaining indices
			for(int i = 0; i < messages.Length - 1; i++) {
				messages[i] = messages[i + 1];
			}

			// Snap scrollbar to the latest message once the layout is updated on the next frame
			if(scrollBar.value < 0.01f) {
				StartCoroutine(ResetScrollBar());
			}

			MessageReceived?.Invoke(contents);
		}

		System.Collections.IEnumerator ResetScrollBar() {
			yield return new WaitForEndOfFrame();
			yield return new WaitForEndOfFrame();
			scrollBar.value = 0f;
		}

		public void OnSubmit(InputAction.CallbackContext value) {
			if(!InputField.isFocused || InputField.text == "") {
				return;
			}

			MessageSubmitted?.Invoke(InputField.text);
			history.Add(InputField.text);
			history.ScrollToStart();
			InputField.text = "";
		}

		private void OnDebugLog(string message, string stack, LogType type) {
			Write("Debug", message);
		}

		public static Color GetChannelColor(string channel) {
			return channels[channel];
		}
	}
}
