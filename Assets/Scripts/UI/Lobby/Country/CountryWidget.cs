using UnityEngine;
using BoM.UI.Lobby;

public class CountryWidget : MonoBehaviour {
	// Start
	void Start() {
		// Construct friends list on login
		Login.instance.onLogIn += account => {
			account.country.Connect(
				this,
				data => {
					// ...
				},
				true
			);
		};
		
		// Disconnect listeners on logout
		Login.instance.onLogOut += account => {
			account.country.Disconnect(this);
			account.country.Clear();
		};
	}
}
