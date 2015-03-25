using UnityEngine;
using uLobby;
using BoM.UI.Lobby;

public class InviteToPartyButton : MonoBehaviour, Initializable {
	// Init
	public void Init() {
		gameObject.SetActive(false);

		// Callback
		AccountChangedCallBack updateButtonVisibility = account => {
			PlayerAccount.mine.party.Get(party => {
				gameObject.SetActive(party.CanAdd(account));
			});
		};

		// On displayed account change
		Profile.instance.onDisplayedAccountChanged += updateButtonVisibility;

		// Connect listener on login
		Login.instance.onLogIn += account => {
			account.party.Connect(
				this,
				party => updateButtonVisibility(Profile.instance.displayedAccount),
				false
			);
		};
		
		// Disconnect listeners on logout
		Login.instance.onLogOut += (account) => {
			account.party.Disconnect(this);
		};
	}

	// InviteToParty
	public void InviteToParty() {
		Lobby.RPC("InviteToParty", Lobby.lobby, Profile.instance.displayedAccount.id);
	}
}
