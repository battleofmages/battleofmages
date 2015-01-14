using uLobby;
using UnityEngine;
using UnityEngine.UI;

public class AddFriendButton : MonoBehaviour {
	public GameObject addFriendUI;
	public InputField nameInputField;

	// OnEnable
	void OnEnable() {
		GetComponent<Button>().interactable = false;
	}

	// AddFriend
	public void AddFriend() {
		Lobby.RPC("AddFriend", Lobby.lobby, nameInputField.text, "General");
		nameInputField.text = "";
	}

	// ToggleAddFriendUI
	public void ToggleAddFriendUI() {
		bool active = !addFriendUI.activeSelf;
		GetComponent<Button>().interactable = active;
		addFriendUI.SetActive(active);
	}

	// Validate
	public void Validate() {
		var playerName = nameInputField.text;
		var account = PlayerAccount.GetByPlayerName(playerName);
		var validPlayerName = Validator.playerName.IsMatch(playerName);

		// Is not a known friend on the friends list
		if(account == null) {
			GetComponent<Button>().interactable = validPlayerName;
			return;
		}
		
		GetComponent<Button>().interactable =
			validPlayerName &&
			PlayerAccount.mine.friendsList.value.CanAdd(account) &&
			playerName != PlayerAccount.mine.playerName.value;
	}
}
