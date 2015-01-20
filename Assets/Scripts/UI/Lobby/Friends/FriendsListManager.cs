using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using uLobby;

public class FriendsListManager : MonoBehaviour, Initializable {
	public Transform friendsGroupRoot;
	public GameObject friendsGroupPrefab;
	public GameObject friendPrefab;

	// Init
	public void Init() {
		Lobby.AddListener(this);
		
		// Construct friends list on login
		Login.instance.onLogIn += () => {
			PlayerAccount.mine.friendsList.Connect(
				this,
				data => {
					ConstructFriendsList();
				},
				false
			);
		};
		
		// Disconnect listeners on logout
		Login.instance.onLogOut += () => {
			PlayerAccount.mine.friendsList.Disconnect(this);
		};
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
