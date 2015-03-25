using UnityEngine.UI;
using uLobby;
using BoM.Async;

public class Profile : SingletonMonoBehaviour<Profile>, Initializable {
	public PlayerAccount displayedAccount;
	public AccountDataConnection[] connections;

	public event AccountChangedCallBack onDisplayedAccountChanged;

	// Init
	public void Init() {
		Awake();
	}

	// OnEnable
	void OnEnable() {
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
		if(PlayerAccount.mine == null)
			return;

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
