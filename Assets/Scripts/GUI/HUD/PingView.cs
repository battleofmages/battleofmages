using UnityEngine;
using System.Collections;

public class PingView : MonoBehaviour {
	public float updateInterval;
	
	private int ping = 0;
	
	// Start
	void Start() {
		StartCoroutine(UpdatePing());
	}
	
	// Update ping
	IEnumerator UpdatePing() {
		while(this.enabled) {
			if(Player.main != null) {
				var newPing = Player.main.stats.ping;
				if(ping != newPing) {
					ping = newPing;
					
					if(ping >= 100) {
						if(ping >= 200)
							guiText.material.color = Color.red;
						else
							guiText.material.color = Color.yellow;
					} else {
						guiText.material.color = Color.white;
					}
					
					guiText.text = System.String.Format("<b>{0}</b> ms", ping);
				}
			}
			
			yield return new WaitForSeconds(updateInterval);
		}
	}
}
