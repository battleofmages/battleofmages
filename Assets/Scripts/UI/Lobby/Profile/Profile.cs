using UnityEngine.UI;

public class Profile : SingletonMonoBehaviour<Profile> {
	public PlayerAccount displayedAccount;
	public AccountDataConnection[] connections;
	
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
	}
	
	// ViewMyProfile
	public void ViewMyProfile() {
		ViewProfile(PlayerAccount.mine);
	}
}
