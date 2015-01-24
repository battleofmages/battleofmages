using UnityEngine;
using UnityEngine.UI;

public class FriendButton : MonoBehaviour, Initializable {
	public Button addButton;
	public Button removeButton;

	// Init
	public void Init() {
		addButton.gameObject.SetActive(false);
		removeButton.gameObject.SetActive(false);

		AccountChangedCallBack updateFriendship = newAccount => {
			PlayerAccount.mine.friendsList.Get(
				friendsList => {
					if(newAccount == null || newAccount == PlayerAccount.mine) {
						addButton.gameObject.SetActive(false);
						removeButton.gameObject.SetActive(false);
						return;
					}
					
					var canAdd = friendsList.CanAdd(newAccount);
					addButton.gameObject.SetActive(canAdd);
					removeButton.gameObject.SetActive(!canAdd);
				},
				false
			);
		};

		// On displayed account change
		Profile.instance.onDisplayedAccountChanged += updateFriendship;

		// On login
		Login.instance.onLogIn += (account) => {
			// On friends list update
			account.friendsList.Connect(
				this,
				data => {
					updateFriendship(Profile.instance.displayedAccount);
				},
				false
			);
		};

		// On logout
		Login.instance.onLogOut += (account) => {
			account.friendsList.Disconnect(this);
		};
	}
}
