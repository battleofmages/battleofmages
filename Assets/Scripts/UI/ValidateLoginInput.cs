using UnityEngine;
using UnityEngine.UI;

public class ValidateLoginInput : MonoBehaviour {
	public Button loginButton;

	private InputField inputField;
	private string accountEmail;
	private string newEmail;

	// Start
	void Start() {
		inputField = GetComponent<InputField>();
		accountEmail = inputField.text.text;
	}

	// Validate
	public void Validate() {
		newEmail = inputField.text.text;

		if(accountEmail != newEmail) {
			accountEmail = newEmail;
			loginButton.interactable = Login.instance.canLogIn;
		}
	}
}
