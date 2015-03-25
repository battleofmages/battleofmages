using UnityEngine;
using UnityEngine.UI;

namespace BoM.UI.Lobby {
	// PlayerNameInput
	public class PlayerNameInput : MonoBehaviour {
		private InputField nameField;

		// Start
		void Start() {
			nameField = GetComponent<InputField>();

			// Intercept adding of characters
			nameField.onValidateInput += (text, charIndex, addedChar) => {
				// Japanese space
				if(addedChar == '　')
					addedChar = ' ';
				
				if(!System.Char.IsLetter(addedChar) && addedChar != ' ')
					return default(char);
				
				string newText = text.Insert(charIndex, addedChar.ToString());
				string prettified = newText.PrettifyPlayerName();
				
				if(newText.Length == prettified.Length)
					return prettified[charIndex];
				else
					return default(char);
			};
		}

		// Prettify
		public void Prettify() {
			nameField.text = nameField.text.PrettifyPlayerName();
		}
	}
}