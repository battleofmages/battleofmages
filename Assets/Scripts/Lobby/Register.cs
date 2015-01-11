using UnityEngine;
using UnityEngine.UI;
using uLobby;
using System;

public class Register : SingletonMonoBehaviour<Register>, Initializable {
	public InputField emailField;
	public InputField passwordField;
	public InputField repeatPasswordField;
	public InputField loginEmailField;

	// Init
	public void Init() {
		// Add this class as a listener to different account events
		AccountManager.OnAccountRegistered += OnAccountRegistered;
		AccountManager.OnRegisterFailed += OnRegisterFailed;
		
		// Receive lobby events
		Lobby.AddListener(this);
	}

	// OnEnable
	void OnEnable() {
		// Load login field text
		emailField.text = loginEmailField.text;
	}

#region Callbacks
	// OnAccountRegistered
	void OnAccountRegistered(Account account) {
		
	}
	
	// OnRegisterFailed
	void OnRegisterFailed(string accountName, AccountError error) {
		
	}
#endregion

	[RPC]
	void ReceiveAccountInfo(string accountId, string propertyName, string typeName, string json) {
		LogManager.General.Log("Received " + propertyName + ": " + json);

		var val = GenericSerializer.ReadObject(Type.GetType(typeName), json);
		var account = PlayerAccount.Get(accountId);

		var propertyField = account.GetType().GetField(propertyName);
		var property = propertyField.GetValue(account);
		var propertyType = propertyField.FieldType;
		var valueProperty = propertyType.GetProperty("value");

		valueProperty.SetValue(property, val, null);

		if(account.isMine && propertyName == "playerName" && (UIManager.instance.currentState == "Waiting" || UIManager.instance.currentState == "Ask Name")) {
			if(string.IsNullOrEmpty((string)val))
				UIManager.instance.currentState = "Ask Name";
			else
				UIManager.instance.currentState = "Lobby";
		}
	}
}
