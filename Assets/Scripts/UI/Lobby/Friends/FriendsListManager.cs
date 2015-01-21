using UnityEngine;
using uLobby;
using System.Collections.Generic;
using System.Linq;

public class FriendsListManager : SingletonMonoBehaviour<FriendsListManager>, Initializable {
	public Transform friendsGroupRoot;
	public GameObject friendsGroupPrefab;
	public GameObject friendPrefab;

	public Dictionary<Friend, FriendWidget> friendToWidget;

	// Init
	public void Init() {
		Lobby.AddListener(this);
		
		// Construct friends list on login
		Login.instance.onLogIn += (account) => {
			account.friendsList.Connect(
				this,
				data => {
					ConstructFriendsList();
				},
				false
			);
		};
		
		// Disconnect listeners on logout
		Login.instance.onLogOut += (account) => {
			account.friendsList.Disconnect(this);
		};
	}

	// SortGroup
	public void SortGroup(FriendsGroup group) {
		group.friends = group.friends.OrderByDescending(
			x => x.account.onlineStatus.value
		).ThenBy(
			x => x.account.playerName.value
		).ToList();

		for(int i = 0; i < group.friends.Count; i++) {
			var friend = group.friends[i];
			friendToWidget[friend].transform.SetSiblingIndex(i + 1);
		}
	}

	// ConstructFriendsList
	public void ConstructFriendsList() {
		var friendsList = PlayerAccount.mine.friendsList.value;

		DeleteFriendsList();
		friendToWidget = new Dictionary<Friend, FriendWidget>(friendsList.friendsCount);

		UIListBuilder<FriendsGroup>.Build(
			friendsList.groups,
			friendsGroupPrefab,
			friendsGroupRoot,
			(clone, group) => {
				// Set friends group instance
				var widget = clone.GetComponent<FriendsGroupWidget>();
				widget.friendsGroup = group;
				
				clone.name = group.name;
				widget.textComponent.text = group.name;
				
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
				var friendWidget = clone.GetComponent<FriendWidget>();
				friendWidget.friend = friend;
				friendWidget.group = group;
				
				// Save in dictionary
				friendToWidget[friend] = friendWidget;
			},
			1
		);
	}

	// DeleteFriendsList
	void DeleteFriendsList() {
		friendsGroupRoot.DeleteChildrenWithComponent<FriendsGroupWidget>();
	}

	// ErrorSuffix
	string ErrorSuffix(string accountId) {
		return " (<color=yellow>" + PlayerAccount.Get(accountId).playerName.value + "</color>)";
	}

#region RPCs
	[RPC]
	void AddFriendError(string accountId, BoM.AddFriendError error) {
		string reason;

		switch(error) {
			case BoM.AddFriendError.PlayerDoesntExist:
				reason = "Player doesn't exist";
				reason += " (<color=yellow>" + accountId + "</color>)";
				break;

			case BoM.AddFriendError.AlreadyInFriendsList:
				reason = "Already in friends list";
				reason += ErrorSuffix(accountId);
				break;

			case BoM.AddFriendError.CantAddYourself:
				reason = "Can't add yourself to friends list";
				reason += ErrorSuffix(accountId);
				break;

			default:
				reason = "Unknown error";
				reason += ErrorSuffix(accountId);
				break;
		}

		NotificationManager.instance.CreateNotification(reason, 4f);
	}

	[RPC]
	void RemoveFriendError(string accountId, BoM.RemoveFriendError error) {
		string reason;
		
		switch(error) {
			case BoM.RemoveFriendError.PlayerDoesntExist:
				reason = "Player doesn't exist";
				reason += ErrorSuffix(accountId);
				break;
				
			case BoM.RemoveFriendError.GroupDoesntExist:
				reason = "Group doesn't exist";
				reason += ErrorSuffix(accountId);
				break;
				
			default:
				reason = "Unknown error";
				reason += ErrorSuffix(accountId);
				break;
		}
		
		NotificationManager.instance.CreateNotification(reason, 4f);
	}
#endregion
}
