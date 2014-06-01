using UnityEngine;

public class Eclipse : SkillInstance {
	public GameObject projectileSpawned;
	public float maxRadius = 10.0f;
	public float interval = 0.2f;
	public float duration = 6.0f;
	private bool changeSky = false;
	
	private ChangeSky skyChanger;
	
	private float radiusStart = 2.5f;
	private float radius;
	private int angle = 0;
	private float startTime;
	
	// Start
	void Start () {
		if(changeSky && !uLink.Network.isServer) {
			GameObject skyChangerObject = GameObject.Find("SkyChanger");
			skyChanger = skyChangerObject.GetComponent<ChangeSky>();
			skyChanger.IncreaseNight();
			
			Invoke("ResetSky", duration + 2.0f);
		}
		
		radius = radiusStart;
		startTime = Time.time;
		
		InvokeRepeating("SpawnDarkFireball", 0.001f, interval);
		
		//Destroy(this.gameObject, duration + 2.0f);
	}
	
	// ResetSky
	private void ResetSky() {
		if(!uLink.Network.isServer) {
			skyChanger.DecreaseNight();
		}
	}
	
	// SpawnDarkFireball
	void SpawnDarkFireball() {
		if(Time.time - startTime < duration) {
			// Spawn one
			Vector3 offset = Quaternion.AngleAxis(angle, Vector3.up) * Vector3.forward * radius;
			Vector3 direction = new Vector3(offset.x, -maxRadius, offset.z);
			
			GameObject clone;
			SkillInstance inst;
			
			SpawnSkillPrefab(
				projectileSpawned,
				transform.position + offset,
				Quaternion.LookRotation(direction),
				out clone,
				out inst
			);
			
			inst.hitPoint = hitPoint - offset * radius;
			Destroy(clone, 3.0f);
			
			// Next iteration
			angle += 18;
			radius += 2.5f;
			
			if(radius > maxRadius)
				radius = radiusStart;
		}
	}
}
