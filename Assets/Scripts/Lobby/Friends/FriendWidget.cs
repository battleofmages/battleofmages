using UnityEngine;
using UnityEngine.UI;

public class FriendWidget : MonoBehaviour {
	public Friend friend;
	public Image onlineStatusImage;

	// Start
	void Start () {
		var friendAccount = PlayerAccount.Get(friend.accountId);

		// Fetch name
		friendAccount.playerName.Connect(data => {
			name = data;
			GetComponentInChildren<Text>().text = data;
		});

		// Online status
		friendAccount.onlineStatus.Connect(data => {
			onlineStatusImage.sprite = OnlineStatusSprites.instance.sprites[(int)data];
		});

		GetComponent<Button>().onClick.AddListener(() => {
			AccountDataConnector.instance.ViewProfile(friendAccount);
		});
	}
}
