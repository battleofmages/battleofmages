using uLobby;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FriendsGUI : LobbyModule<FriendsGUI> {
	public GUIStyle groupHeaderStyle;
	public GUIStyle groupNameStyle;
	public GUIStyle friendNameStyle;
	
	public GUIContent addFriendContent;
	public GUIContent removeFriendContent;
	public GUIContent noteContent;
	public GUIContent newGroupContent;
	public GUIContent removeGroupContent;
	
	private Vector2 scrollPosition;
	private FriendsList friendsList;

	// Start
	void Start() {
		friendsList = null;

		// Listen to lobby events
		Lobby.AddListener(this);
	}

	// Draw
	public override void Draw() {
		friendsList = InGameLobby.instance.displayedAccount.friends;
		if(friendsList == null)
			return;
		
		// Body
		using(new GUIScrollView(ref scrollPosition)) {
			using(new GUIVertical()) {
				// Friends
				foreach(var group in friendsList.groups) {
					DrawFriendsGroup(group);
				}
				
				// Footer
				DrawFooter();
				
				// Followers
				DrawFollowers();
			}
		}
	}
	
	// DrawFooter
	void DrawFooter() {
		using(new GUIHorizontal()) {
			GUILayout.FlexibleSpace();
			
			// Create new group
			if(GUIHelper.Button(newGroupContent)) {
				new TextFieldWindow(
					"Enter new group name:",
					"",
					groupName => {
						Lobby.RPC("AddFriendsGroup", Lobby.lobby, groupName);
						friendsList.groups.Add(new FriendsGroup(groupName));
					}
				).acceptText = "Create";
			}
		}
	}

	// Draw friends group
	void DrawFriendsGroup(FriendsGroup friendsGroup) {
		// Header
		using(new GUIHorizontal(groupHeaderStyle)) {
			GUI.color = friendsGroup.color;
			GUILayout.Label(friendsGroup.name, groupNameStyle);
			GUI.color = Color.white;
			GUILayout.FlexibleSpace();

			// Add friend
			if(GUIHelper.Button(addFriendContent)) {
				new FriendAddWindow(
					"Add a friend:",
					"",
					friendsGroup
				);
			}
			
			// Remove group
			if(GUIHelper.Button(removeGroupContent)) {
				new Confirm(
					_("Do you really want to remove the group <b>{0}</b>? All friends in this group will be deleted from your list.", friendsGroup.name),
					() => {
						LogManager.General.Log(_("Removed friends list group {0}", friendsGroup.name));
						Lobby.RPC("RemoveFriendsGroup", Lobby.lobby, friendsGroup.name);
						friendsList.RemoveGroup(friendsGroup.name);
					}
				);
			}
		}
		
		// Draw friends in this group
		using(new GUIVertical("box")) {
			if(friendsGroup.friends.Count == 0) {
				GUILayout.Label("This group doesn't have any contacts yet.", friendNameStyle);
			} else {
				foreach(var friend in friendsGroup.friends) {
					DrawFriend(friendsGroup, friend);
				}
			}
		}
	}

	// Draw friend
	void DrawFriend(FriendsGroup friendsGroup, Friend friend) {
		if(string.IsNullOrEmpty(friend.accountId)) {
			LogManager.General.LogError("FriendsGUI: Account ID is empty");
			return;
		}
		
		// TODO: ...
		var account = PlayerAccount.Get(friend.accountId);
		string playerName = account.playerName;
		if(string.IsNullOrEmpty(playerName))
			return;
		
		// New row
		using(new GUIHorizontal()) {
			// Draw the player name
			DrawPlayerName(playerName, new GUIContent(playerName), friendNameStyle);
			
			// Space
			GUILayout.FlexibleSpace();
			
			// Note
			noteContent.tooltip = friend.note;
			if(GUIHelper.Button(noteContent)) {
				new TextAreaWindow(
					_("Note for player <b>{0}</b>:", playerName),
					friend.note,
					newNote => {
						friend.note = newNote;
						Lobby.RPC("SetFriendNote", Lobby.lobby, playerName, friendsGroup.name, newNote);
					}
				);
			}
			
			// Remove friend
			if(GUIHelper.Button(removeFriendContent)) {
				new Confirm(
					_("Do you really want to remove <b>{0}</b> from your friends list?", playerName),
					() => {
						Lobby.RPC("RemoveFriend", Lobby.lobby, playerName, friendsGroup.name);
					}
				);
			}
		}
	}
	
	// DrawFollowers
	void DrawFollowers() {
		var followers = InGameLobby.instance.displayedAccount.followersOnly;
		
		// Header
		using(new GUIHorizontal(groupHeaderStyle)) {
			GUILayout.Label("Followers", groupNameStyle);
			GUILayout.FlexibleSpace();
		}
		
		// Followers
		using(new GUIVertical("box")) {
			if(followers == null || followers.Count == 0) {
				GUILayout.Label("You don't have any followers yet.", friendNameStyle);
			} else {
				foreach(var followerAccountId in followers) {
					var playerName = PlayerAccount.Get(followerAccountId).playerName;
					
					using(new GUIHorizontal()) {
						DrawPlayerName(playerName, new GUIContent(playerName), friendNameStyle);
						GUILayout.FlexibleSpace();
					}
				}
			}
		}
	}

	// --------------------------------------------------------------------------------
	// RPCs
	// --------------------------------------------------------------------------------
	
#region RPCs
	[RPC]
	void ReceiveFriendsList(string accountId, string newList) {
		friendsList = Jboy.Json.ReadObject<FriendsList>(newList);
		PlayerAccount.Get(accountId).friends = friendsList;

		LogManager.General.Log("FriendsGUI: Received friends list: " + friendsList);
	}
	
	[RPC]
	void ReceiveFollowersList(string accountId, string[] accountIdList, bool dummy) {
		PlayerAccount.Get(accountId).followers = accountIdList;
		
		LogManager.General.Log("FriendsGUI: Received followers list, count: " + accountIdList.Length);
	}
	
	[RPC]
	void FriendAddPlayerDoesntExistError(string friendName) {
		LogManager.General.LogWarning(_("Player '{0}' doesn't exist", friendName));
	}
	
	[RPC]
	void FriendAddCantAddYourselfError(string friendName) {
		LogManager.General.LogWarning("Can't add yourself to your own friends list");
	}
	
	[RPC]
	void FriendAddAlreadyExistsError(string friendName) {
		LogManager.General.LogWarning(_("Player '{0}' is already on your friends list", friendName));
	}
#endregion
}
