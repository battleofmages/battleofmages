using UnityEngine.UI;
using uLobby;

public class Profile : SingletonMonoBehaviour<Profile> {
	public PlayerAccount displayedAccount;
	public AccountDataConnection[] connections;

	public event AccountChangedCallBack onDisplayedAccountChanged;
	
	// OnEnable
	void OnEnable() {
		if(PlayerAccount.mine == null)
			return;

		ViewMyProfile();
	}
	
	// ViewProfile
	public void ViewProfile(PlayerAccount account) {
		displayedAccount = account;

		foreach(var connection in connections) {
			var textFields = connection.textFields;
			
			AsyncProperty<string>.GetProperty(account, connection.propertyName).Get((val) => {
				foreach(var textField in textFields) {
					textField.text = val;
				}
			});
		}

		if(onDisplayedAccountChanged != null)
			onDisplayedAccountChanged(account);
	}
	
	// ViewMyProfile
	public void ViewMyProfile() {
		ViewProfile(PlayerAccount.mine);
	}

	// AddFriend
	public void AddFriend() {
		Lobby.RPC("AddFriendAccountToGroup", Lobby.lobby, displayedAccount.id, "General");
	}

	// RemoveFriend
	public void RemoveFriend() {
		if(!PlayerAccount.mine.friendsList.available)
			return;

		var friendsGroup = PlayerAccount.mine.friendsList.value.GetGroupByAccount(displayedAccount.id);

		if(friendsGroup == null) {
			LogManager.General.LogError("Couldn't remove friend from friends list: " + displayedAccount);
			return;
		}

		Lobby.RPC("RemoveFriendAccountFromGroup", Lobby.lobby, displayedAccount.id, friendsGroup.name);
	}
}
