using UnityEngine;
using UnityEngine.UI;
using System;
using BoM.Async;

namespace BoM.UI.Lobby {
	// AccountDataConnection
	[Serializable]
	public struct AccountDataConnection {
		public string propertyName;
		public Text[] textFields;
	}

	// AccountDataConnector
	public class AccountDataConnector : MonoBehaviour {
		public AccountDataConnection[] connections;

		// OnEnable
		void OnEnable() {
			ConnectMyAccount();
		}

		// Connect
		public void Connect(PlayerAccount account) {
			foreach(var connection in connections) {
				var textFields = connection.textFields;
				
				AsyncProperty<string>.GetProperty(account, connection.propertyName).Get((val) => {
					foreach(var textField in textFields) {
						textField.text = val;
					}
				});
			}
		}

		// ConnectMyAccount
		public void ConnectMyAccount() {
			Connect(PlayerAccount.mine);
		}
	}
}