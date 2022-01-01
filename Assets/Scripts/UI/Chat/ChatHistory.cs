using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using TMPro;

namespace BoM.UI {
	// Data
	public class ChatHistoryData : MonoBehaviour {
		[SerializeField] protected TMP_InputField inputField;
		protected List<string> messages = new List<string>();
		protected int index;
		protected string currentMessage;
	}

	// Logic
	public class ChatHistory : ChatHistoryData {
		public void Add(string message) {
			messages.Add(message);
		}

		public void ScrollToStart() {
			index = 0;
		}

		public void OnNavigate(InputAction.CallbackContext value) {
			if(!inputField.isFocused) {
				return;
			}

			var y = value.ReadValue<Vector2>().y;

			if(y < 0.5f && y > -0.5f) {
				return;
			}

			if(index == 0) {
				currentMessage = inputField.text;
			}

			var oldIndex = index;

			if(y > 0.5f) {
				index++;

				if(index > messages.Count) {
					index = messages.Count;
					return;
				}
			} else {
				index--;

				if(index < 0) {
					index = 0;
					return;
				}
			}

			if(index == 0) {
				inputField.text = currentMessage;
			} else {
				inputField.text = messages[messages.Count - index];
			}
		}
	}
}
