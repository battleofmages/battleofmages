using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MapConfiguration : MonoBehaviour {
	public bool fog = true;
	public Color fogColor = new Color(0.5f, 0.5f, 0.5f, 1f);
	public FogMode fogMode = FogMode.ExponentialSquared;
	public float fogDensity = 0.0007f;
	public float linearFogStart = 0f;
	public float linearFogEnd = 1000f;
	public Color ambientLight = new Color(0.2f, 0.2f, 0.2f, 1f);
	public Material skyboxMaterial;
	
	// Render settings
	void Start() {
		ApplyRenderSettings();
		
		if(ChangeSky.instance != null)
			ChangeSky.instance.dayMaterial = skyboxMaterial;

#if UNITY_EDITOR
		if(uLink.Network.isClient && GameObject.Find("Client") == null) {
			Application.LoadLevel("Client");
		}
#endif
	}

	// Apply render settings
	public void ApplyRenderSettings() {
		RenderSettings.fog = fog;
		RenderSettings.fogColor = fogColor;
		RenderSettings.fogMode = fogMode;
		RenderSettings.fogDensity = fogDensity;
		RenderSettings.fogStartDistance = linearFogStart;
		RenderSettings.fogEndDistance = linearFogEnd;
		RenderSettings.ambientLight = ambientLight;
		RenderSettings.skybox = skyboxMaterial;
	}

#if UNITY_EDITOR
	[MenuItem("Battle of Mages/Map/Apply render settings")]
	static void StaticApplyRenderSettings() {
		GameObject.FindGameObjectWithTag("Map").GetComponent<MapConfiguration>().ApplyRenderSettings();
	}
#endif
}
