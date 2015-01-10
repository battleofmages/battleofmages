using UnityEngine;
using UnityEngine.UI;
using System;

public class AccountDataConnector : MonoBehaviour {
	[Serializable]
	public struct AccountDataConnection {
		public string propertyName;
		public Text[] textFields;
	}

	public AccountDataConnection[] connections;

	// Start
	void Start() {
		foreach(var connection in connections) {
			var textFields = connection.textFields;

			AsyncProperty<string>.GetProperty(PlayerAccount.mine, connection.propertyName).Connect((val) => {
				foreach(var textField in textFields) {
					textField.text = val;
				}
			});
		}
	}
}
