using UnityEngine;
using UnityEngine.UI;

public class FriendWidget : MonoBehaviour {
	public Friend friend;
	public Image onlineStatusImage;
	public Text textComponent;

	private PlayerAccount friendAccount;
	private AsyncProperty<string>.ConnectCallBack nameCallBack;
	private AsyncProperty<OnlineStatus>.ConnectCallBack statusCallBack;

	// Start
	void Start () {
		friendAccount = friend.account;

		// Connect
		friendAccount.playerName.Connect(this, newName => {
			name = newName;
			textComponent.text = newName;
		});

		friendAccount.onlineStatus.Connect(this, status => {
			onlineStatusImage.sprite = OnlineStatusSprites.Get(status);
		});

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
		if(friendAccount == null)
			return;

		// Disconnect
		friendAccount.playerName.Disconnect(this);
		friendAccount.onlineStatus.Disconnect(this);
	}
}
