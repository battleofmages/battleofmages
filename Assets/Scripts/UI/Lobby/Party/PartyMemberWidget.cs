using UnityEngine;
using UnityEngine.UI;

public class PartyMemberWidget : MonoBehaviour {
	public PlayerAccount account;
	public Text nameComponent;
	public Text infoComponent;
	public Image onlineStatusImage;
	public RawImage avatar;

	// Start
	void Start() {
		// Connect
		account.playerName.Connect(this, newName => {
			name = newName;
			nameComponent.text = newName;
		});

		account.onlineStatus.Connect(this, status => {
			onlineStatusImage.sprite = OnlineStatusSprites.Get(status);
		}, false);

		account.avatarURL.Connect(this, url => {
			NetworkHelper.GetTexture(url, tex => {
				avatar.texture = tex;
				avatar.Fade(
					1.0f,
					val => {
						avatar.color = new Color(1f, 1f, 1f, val);
					}
				);
			});
		});

		var button = GetComponent<Button>();
		
		button.onClick.AddListener(() => {
			Profile.instance.ViewProfile(account);
		});
		
		button.onClick.AddListener(() => {
			Sounds.instance.Play("buttonClick");
		});
	}

	// OnDestroy
	void OnDestroy() {
		// Disconnect
		account.playerName.Disconnect(this);
		account.onlineStatus.Disconnect(this);
		account.avatarURL.Disconnect(this);
	}
}
