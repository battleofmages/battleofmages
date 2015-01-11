using UnityEngine.UI;
using System;

public class AccountDataConnector : SingletonMonoBehaviour<AccountDataConnector> {
	[Serializable]
	public struct AccountDataConnection {
		public string propertyName;
		public Text[] textFields;
	}

	public AccountDataConnection[] connections;

	// Start
	void Start() {
		ViewMyProfile();
	}

	// ViewProfile
	public void ViewProfile(PlayerAccount account) {
		foreach(var connection in connections) {
			var textFields = connection.textFields;
			
			AsyncProperty<string>.GetProperty(account, connection.propertyName).Get((val) => {
				foreach(var textField in textFields) {
					textField.text = val;
				}
			});
		}
	}

	// ViewMyProfile
	public void ViewMyProfile() {
		ViewProfile(PlayerAccount.mine);
	}
}
