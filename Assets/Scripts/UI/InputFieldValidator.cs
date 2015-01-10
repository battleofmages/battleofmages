using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class InputFieldValidator : MonoBehaviour {
	public string validationType;
	private InputField inputField;
	private Text textComponent;
	private Color defaultTextColor;

	// Validate
	public void Validate() {
		if(inputField == null) {
			inputField = GetComponent<InputField>();

			if(inputField == null) {
				LogManager.General.LogWarning("InputFieldValidator used on an object without an InputField");
				return;
			}

			textComponent = inputField.textComponent;
			defaultTextColor = textComponent.color;
		}

		var regex = (Regex)typeof(Validator).GetField(validationType).GetValue(null);
		if(regex.IsMatch(inputField.text)) {
			textComponent.color = defaultTextColor;
		} else {
			textComponent.color = new Color(defaultTextColor.r, defaultTextColor.g, defaultTextColor.b, defaultTextColor.a * 0.25f);
		}
	}
}
