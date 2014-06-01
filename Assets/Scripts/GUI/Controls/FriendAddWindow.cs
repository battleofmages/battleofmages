using uLobby;
using UnityEngine;
using System.Collections;

// Delegate
public delegate void FriendAddCallBack(string name, FriendsGroup group);

// FriendAddWindow
public class FriendAddWindow : AcceptWindow<FriendAddCallBack> {
	protected string friendName;
	protected FriendsGroup group;
	private bool friendNameFocused;

	// Constructor
	public FriendAddWindow(string nText, string nFriendName, FriendsGroup nGroup = null, FriendAddCallBack nYes = null, CallBack nNo = null) : base(nText, nYes, nNo) {
		// Default group
		if(nGroup == null) {
			nGroup = PlayerAccount.mine.friends.groups[0];
		}
		
		// Default callback
		if(nYes == null) {
			accept = (name, toGroup) => {
				LogManager.General.Log(string.Format("Adding player '{0}' to friends list group {1}", name, toGroup.name));
				Lobby.RPC("AddFriend", Lobby.lobby, name, toGroup.name);
			};
		}
		
		friendName = nFriendName;
		group = nGroup;
		friendNameFocused = false;
		acceptText = "Add";
		cancelText = "Cancel";
		popupWindowHash = "FriendAddWindow".GetHashCode();
		
		this.Init();
	}

	// Draw
	public override void Draw() {
		// Confirm box
		using(new GUIVertical("box")) {
			// Title
			DrawText();

			using(new GUIHorizontal()) {
				GUILayout.Space(4);

				GUI.SetNextControlName("FriendName");
				friendName = GUIHelper.PlayerNameField(friendName);

				if(!friendNameFocused) {
					GUIHelper.Focus("FriendName");
					friendNameFocused = true;
				}

				GUILayout.Space(4);
			}

			// TODO: Be able to change the group
			using(new GUIHorizontal()) {
				GUILayout.Space(4);
				GUIHelper.Button("Group: <b>" + group.name + "</b>", controlID);
				GUILayout.Space(4);
			}

			// Yes / No
			DrawButtons();
		}
	}

	// Accept
	public override void Accept() {
		accept(friendName, group);
	}
}
