using UnityEngine;
using UnityEngine.UI;

public class PartyMemberWidget : MonoBehaviour {
	public string accountId;
	public Text nameComponent;
	public Text infoComponent;
	public Image onlineStatusImage;

	// Start
	void Start() {
		var account = PlayerAccount.Get(accountId);

		// Fetch name
		account.playerName.Connect(data => {
			name = data;
			nameComponent.text = data;
		});

		// Online status
		account.onlineStatus.Connect(data => {
			onlineStatusImage.sprite = OnlineStatusSprites.instance.sprites[(int)data];
		});
	}
}
