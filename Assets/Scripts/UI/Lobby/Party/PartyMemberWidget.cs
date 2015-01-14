using UnityEngine;
using UnityEngine.UI;

public class PartyMemberWidget : MonoBehaviour {
	public PlayerAccount account;
	public Text nameComponent;
	public Text infoComponent;
	public Image onlineStatusImage;

	private AsyncProperty<string>.ConnectCallBack nameCallBack;
	private AsyncProperty<OnlineStatus>.ConnectCallBack statusCallBack;

	// Start
	void Start() {
		// Fetch name
		nameCallBack = newName => {
			name = newName;
			nameComponent.text = newName;
		};
		
		// Online status
		statusCallBack = status => {
			onlineStatusImage.sprite = OnlineStatusSprites.Get(status);
		};

		// Connect
		account.playerName.Connect(nameCallBack);
		account.onlineStatus.Connect(statusCallBack);

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
		account.playerName.Disconnect(nameCallBack);
		account.onlineStatus.Disconnect(statusCallBack);
	}
}
