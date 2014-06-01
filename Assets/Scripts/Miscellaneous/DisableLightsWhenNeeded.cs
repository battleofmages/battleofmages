using UnityEngine;
using System.Collections;

public class DisableLightsWhenNeeded : MonoBehaviour {
	public const float updateInterval = 1.0f;
	public const int maxPlayerCount = 11;
	
	public Light[] lights;
	
	private bool lightsDisabled = false;
	
	void Start() {
		StartCoroutine(UpdateLights());
	}
	
	// UpdateLights
	IEnumerator UpdateLights() {
		while(this.enabled) {
			if(!lightsDisabled && Player.allPlayers.Count > maxPlayerCount) {
				foreach(var light in lights) {
					light.enabled = false;
				}
				
				lightsDisabled = true;
			} else if(lightsDisabled && Player.allPlayers.Count <= maxPlayerCount) {
				foreach(var light in lights) {
					light.enabled = false;
				}
				
				lightsDisabled = false;
			}
			
			yield return new WaitForSeconds(updateInterval);
		}
	}
}
