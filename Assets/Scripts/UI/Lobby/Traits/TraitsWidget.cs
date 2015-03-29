using UnityEngine;
using BoM.UI.Lobby;

public class TraitsWidget : MonoBehaviour, Initializable {
	// Init
	public void Init() {
		Login.instance.onLogIn += account => {
			account.traits.Connect(
				this,
				data => { LogManager.General.Log("Traits: " + data.ToString());
			}
			);
		};
	}
}