using UnityEngine;

// Config
public class Config : SingletonMonoBehaviour<Config> {
	[Header("Misc")]
	
	[Range(1, 10)]
	public float blockMinimumEnergyForUsage = 5.0f;
	
	public float raycastMaxDistance;
}