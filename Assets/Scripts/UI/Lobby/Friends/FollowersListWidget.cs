using BoM.UI.Lobby;

public class FollowersListWidget : SingletonMonoBehaviour<FollowersListWidget>, Initializable {
	// Init
	public void Init() {
		// Construct friends list on login
		Login.instance.onLogIn += account => {
			account.followers.Connect(
				this,
				data => {
					ConstructFollowersList();
				},
				false
			);
		};
		
		// Disconnect listeners on logout
		Login.instance.onLogOut += (account) => {
			account.followers.Disconnect(this);
		};
	}

	// ConstructFollowersList
	void ConstructFollowersList() {
		//var followers = PlayerAccount.mine.followers.value;
	}
}
