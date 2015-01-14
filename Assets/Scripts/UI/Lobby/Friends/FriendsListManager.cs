using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using uLobby;

public class FriendsListManager : MonoBehaviour {
	public Transform friendsGroupRoot;
	public GameObject friendsGroupPrefab;
	public GameObject friendPrefab;

	// Start
	void Start() {
		Lobby.AddListener(this);
	}

	// OnEnable
	void OnEnable() {
		if(PlayerAccount.mine == null)
			return;

		PlayerAccount.mine.friendsList.Get(data => {
			ConstructFriendsList();
		});
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

#region RPCs
	[RPC]
	void AddFriendError(string playerName, BoM.AddFriendError error) {
		string reason;

		switch(error) {
			case BoM.AddFriendError.PlayerDoesntExist:
				reason = "Player doesn't exist";
				break;

			case BoM.AddFriendError.AlreadyInFriendsList:
				reason = "Already in friends list";
				break;

			case BoM.AddFriendError.CantAddYourself:
				reason = "Can't add yourself to friends list";
				break;

			default:
				reason = "Unknown error";
				break;
		}

		NotificationManager.instance.CreateNotification(reason + " (<color=yellow>" + playerName + "</color>)", 4f);
	}
#endregion
}
