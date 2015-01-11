using UnityEngine;
using UnityEngine.UI;

public class FriendsListManager : MonoBehaviour {
	public GameObject friendsGroupPrefab;
	public GameObject friendPrefab;

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

		for(int i = 0; i < friendsList.groups.Count; i++) {
			var group = friendsList.groups[i];
			var clone = (GameObject)Instantiate(friendsGroupPrefab);

			clone.transform.SetParent(transform, false);
			clone.transform.SetSiblingIndex(i);
			clone.GetComponentInChildren<Text>().text = group.name;

			BuildFriendsGroup(group, clone);
		}
	}

	// BuildFriendsGroup
	void BuildFriendsGroup(FriendsGroup group, GameObject groupObject) {
		for(int i = 0; i < group.friends.Count; i++) {
			var friend = group.friends[i];
			var clone = (GameObject)Instantiate(friendPrefab);
			clone.transform.SetParent(groupObject.transform, false);

			PlayerAccount.Get(friend.accountId).playerName.Connect(data => {
				clone.GetComponentInChildren<Text>().text = data;
			});
		}
	}
}
