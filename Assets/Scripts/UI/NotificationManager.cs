using UnityEngine;
using UnityEngine.UI;

public class NotificationManager : DestroyableSingletonMonoBehaviour<NotificationManager> {
	public GameObject notificationPrefab;

	// CreateNotification
	public void CreateNotification(string msg, float duration = 3f) {
		var clone = Instantiate(notificationPrefab);
		clone.transform.SetParent(this.transform, false);
		clone.GetComponent<Notification>().duration = duration;
		clone.GetComponentInChildren<Text>().text = msg;
	}
}
