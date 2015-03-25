using UnityEngine;
using UnityEngine.UI;
using BoM.Async;

public class FriendWidget : MonoBehaviour {
	public Friend friend;
	public FriendsGroup group;
	public Image onlineStatusImage;
	public Text textComponent;
	public RawImage avatar;

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
			FriendsListManager.instance.SortGroup(group);
		});

		friendAccount.onlineStatus.Connect(this, status => {
			onlineStatusImage.sprite = OnlineStatusSprites.Get(status);
			FriendsListManager.instance.SortGroup(group);
		}, false);

		friendAccount.avatarURL.Connect(this, url => {
			NetworkHelper.GetTexture(url, tex => {
				if(avatar == null)
					return;

				avatar.texture = tex;
				avatar.Fade(
					1.0f,
					val => {
						avatar.color = new Color(1f, 1f, 1f, val);
					}
				);
			});
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
		friendAccount.avatarURL.Disconnect(this);
	}
}
