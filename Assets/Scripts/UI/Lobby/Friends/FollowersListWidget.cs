using UnityEngine;
using BoM.UI;
using BoM.UI.Lobby;
using BoM.Friends;
using System.Linq;
using System.Collections.Generic;

public class FollowersListWidget : SingletonMonoBehaviour<FollowersListWidget>, Initializable {
	public GameObject followerPrefab;

	private FriendsGroup followersGroup;

	// Init
	public void Init() {
		// Construct friends list on login
		Login.instance.onLogIn += account => {
			account.followers.Connect(
				this,
				data => {
					List<Friend> friends = data.Select(accountId => new Friend(accountId)).ToList();
					followersGroup = new FriendsGroup("Followers", friends);
					ConstructFollowersList();
				},
				false
			);
		};
		
		// Disconnect listeners on logout
		Login.instance.onLogOut += (account) => {
			account.followers.Disconnect(this);
			account.followers.Clear();
		};
	}

	// DeleteFollowersList
	void DeleteFollowersList() {
		transform.DeleteChildrenWithComponent<FriendWidget>();
	}

	// ConstructFollowersList
	void ConstructFollowersList() {
		DeleteFollowersList();
		var followers = PlayerAccount.mine.followers.value;

		UIListBuilder<string>.Build(
			followers,
			followerPrefab,
			gameObject,
			(clone, accountId) => {
				// Set friend instance
				var friendWidget = clone.GetComponent<FriendWidget>();
				friendWidget.friend = new Friend(accountId);
				friendWidget.group = followersGroup;
			},
			1
		);
	}
}
