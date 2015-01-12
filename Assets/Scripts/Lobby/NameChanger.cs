using UnityEngine;
using UnityEngine.UI;
using uLobby;

public class NameChanger : MonoBehaviour {
	public Button acceptButton;
	public InputField nameField;

	private bool validName;
	private string lastPlayerNameChecked;
	private float lastPlayerNameCheckedTime;

	// Start
	void Start() {
		// Disabled
		acceptButton.gameObject.SetActive(false);

		// Receive RPCs
		Lobby.AddListener(this);
	}

	// Validate
	public void Validate() {
		validName = Validator.playerName.IsMatch(nameField.text);
		acceptButton.gameObject.SetActive(false);
	}

	// Accept
	public void Accept() {
		Lobby.RPC("NameChange", Lobby.lobby, nameField.text);
	}

	// Update
	void Update() {
		// Check availability
		if(validName && nameField.text != lastPlayerNameChecked && Time.time - lastPlayerNameCheckedTime >= 0.5f) {
			Lobby.RPC("CheckName", Lobby.lobby, nameField.text);
			lastPlayerNameCheckedTime = Time.time;
			lastPlayerNameChecked = nameField.text;
		}
	}

#region RPCs
	[RPC]
	void NameCheck(string playerName, bool available) {
		NotificationManager.instance.CreateNotification(
			"<color=yellow>" + playerName + "</color>" + (
				available ?
				" is available." :
				" <color=red>is not available</color>."
			),
			2.5f
		);

		if(available && playerName == nameField.text)
			acceptButton.gameObject.SetActive(true);
	}
#endregion
}
