using UnityEngine;
using UnityEngine.UI;

public class FriendWidget : MonoBehaviour {
	public Friend friend;
	public Image onlineStatusImage;

	private PlayerAccount friendAccount;
	private AsyncProperty<string>.ConnectCallBack nameCallBack;
	private AsyncProperty<OnlineStatus>.ConnectCallBack statusCallBack;

	// Start
	void Start () {
		friendAccount = friend.account;

		// Fetch name
		nameCallBack = newName => {
			name = newName;
			GetComponentInChildren<Text>().text = newName;
		};

		// Online status
		statusCallBack = status => {
			onlineStatusImage.sprite = OnlineStatusSprites.Get(status);
		};

		// Connect
		friendAccount.playerName.Connect(nameCallBack);
		friendAccount.onlineStatus.Connect(statusCallBack);

		// Button setup
		var button = GetComponent<Button>();

		button.onClick.AddListener(() => {
			Profile.instance.ViewProfile(friendAccount);
		});

		button.onClick.AddListener(() => {
			Sounds.instance.Play("buttonClick");
		});
	}

	// OnDestroy
	void OnDestroy() {
		// Disconnect
		friendAccount.playerName.Disconnect(nameCallBack);
		friendAccount.onlineStatus.Disconnect(statusCallBack);
	}
}
