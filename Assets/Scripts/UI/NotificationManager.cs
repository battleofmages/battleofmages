using UnityEngine;
using UnityEngine.UI;

public class NotificationManager : DestroyableSingletonMonoBehaviour<NotificationManager> {
	public GameObject notificationPrefab;
	public GameObject partyInvitationPrefab;

	// CreateNotification
	public GameObject CreateNotification(string msg, float duration = 3f, GameObject prefab = null) {
		if(prefab == null)
			prefab = notificationPrefab;

		var clone = Instantiate(prefab);
		clone.transform.SetParent(this.transform, false);
		clone.GetComponent<Notification>().duration = duration;
		clone.GetComponentInChildren<Text>().text = msg;
		return clone;
	}

	// CreatePartyInvitation
	public void CreatePartyInvitation(string msg, PlayerAccount account, float duration = 3f) {
		var clone = CreateNotification(msg, duration, partyInvitationPrefab);
		clone.GetComponent<PartyInvitation>().account = account;
	}
}
