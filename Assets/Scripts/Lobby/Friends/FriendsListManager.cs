using UnityEngine;
using UnityEngine.UI;
using uLobby;

public class FriendsListManager : MonoBehaviour {
	public Transform friendsGroupRoot;

	public GameObject addFriendUI;
	public InputField addFriendInputField;
	public Button addFriendAcceptField;

	public GameObject friendsGroupPrefab;
	public GameObject friendPrefab;

	// OnEnable
	void OnEnable() {
		if(PlayerAccount.mine == null)
			return;

		PlayerAccount.mine.friendsList.Get(data => {
			ConstructFriendsList();
		});

		addFriendUI.SetActive(false);
	}

	// ConstructFriendsList
	void ConstructFriendsList() {
		var friendsList = PlayerAccount.mine.friendsList.value;

		DeleteFriendsList();

		UIListBuilder<FriendsGroup>.Build(
			friendsList.groups,
			friendsGroupPrefab,
			friendsGroupRoot,
			(clone, group) => {
				// Set friends group instance
				clone.GetComponent<FriendsGroupWidget>().friendsGroup = group;
				
				clone.name = group.name;
				clone.GetComponentInChildren<Text>().text = group.name;
				
				BuildFriendsGroup(group, clone);
			}
		);
	}

	// BuildFriendsGroup
	void BuildFriendsGroup(FriendsGroup group, GameObject groupObject) {
		UIListBuilder<Friend>.Build(
			group.friends,
			friendPrefab,
			groupObject,
			(clone, friend) => {
				// Set friend instance
				clone.GetComponent<FriendWidget>().friend = friend;
			},
			1
		);
	}

	// DeleteFriendsList
	void DeleteFriendsList() {
		friendsGroupRoot.DeleteChildrenWithComponent<FriendsGroupWidget>();
	}

	// ToggleAddFriendUI
	public void ToggleAddFriendUI() {
		bool active = !addFriendUI.activeSelf;
		addFriendInputField.interactable = active;
		addFriendUI.SetActive(active);
	}

	// AddFriend
	public void AddFriend() {
		addFriendInputField.interactable = false;
		addFriendAcceptField.interactable = false;

		Lobby.RPC("SetFriendsList", Lobby.lobby, PlayerAccount.mine.id);
	}
}
