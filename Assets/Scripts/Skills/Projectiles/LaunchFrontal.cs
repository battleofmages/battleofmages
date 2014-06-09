using UnityEngine;
using System.Collections;

public class LaunchFrontal : SkillInstance {
	public GameObject projectileSpawned;
	public int stepSize = 10;
	public int fromAngle = -80;
	public int toAngle = 80;
	
	// Start
	void Start() {
		Detonate();
	}
	
	// Detonate
	void Detonate() {
		for(int i = fromAngle; i < toAngle; i += stepSize) {
			GameObject clone;
			SkillInstance inst;
			
			this.SpawnSkillPrefab(projectileSpawned, transform.position, Cache.quaternionIdentity, out clone, out inst);
			
			clone.transform.parent = this.transform;
			clone.transform.localRotation = Quaternion.Euler(-Mathf.Abs(i * 0.5f), i, 0);
			
			inst.hitPoint = this.hitPoint;
			
			// Activate light on middle one
			if(i == 0)
				clone.GetComponent<Light>().enabled = true;
		}
	}
}
