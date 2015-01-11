using UnityEngine;
using UnityEngine.UI;

public class FriendInstance : MonoBehaviour {
	public Friend friend;

	// Start
	void Start () {
		GetComponent<Button>().onClick.AddListener(() => {
			AccountDataConnector.instance.ViewProfile(PlayerAccount.Get(friend.accountId));
		});
	}
}
