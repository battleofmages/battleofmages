using UnityEngine;
using System.Collections;

public class ChangeSky : SingletonMonoBehaviour<ChangeSky> {
	[HideInInspector]
	public Material dayMaterial;
	
	public Material nightMaterial;
	
	private int nightCounter = 0;
	
	public void Start() {
		if(dayMaterial == null)
			dayMaterial = RenderSettings.skybox;
		
		UpdateSky();
	}
	
	public void IncreaseNight() {
		nightCounter += 1;
		
		UpdateSky();
	}
	
	public void DecreaseNight() {
		nightCounter -= 1;
		
		UpdateSky();
	}
	
	void UpdateSky() {
		if(nightCounter > 0) {
			RenderSettings.skybox = nightMaterial;
			//RenderSettings.ambientLight = new Color(0.0f, 0.0f, 0.0f, 1.0f);
			//RenderSettings.fogColor = new Color(0.0f, 0.0f, 0.0f, 1.0f);
			//RenderSettings.fogDensity = 0.0050f;
		} else {
			RenderSettings.skybox = dayMaterial;
			//RenderSettings.ambientLight = new Color(0.2f, 0.2f, 0.2f, 1.0f);
			//RenderSettings.fogColor = new Color(0.5f, 0.5f, 0.5f, 1.0f);
			//RenderSettings.fogDensity = 0.0007f;
		}
	}
}
