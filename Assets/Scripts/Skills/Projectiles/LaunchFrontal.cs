using UnityEngine;
using System.Collections.Generic;

public class LaunchFrontal : SkillInstance {
	public GameObject projectileSpawned;
	public int stepSize = 10;
	public int fromAngle = -80;
	public int toAngle = 80;

	//private List<GameObject> projectiles = new List<GameObject>();
	
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

			//projectiles.Add(clone);

			clone.transform.parent = this.transform;
			clone.transform.localRotation = Quaternion.Euler(-Mathf.Abs(i * 0.5f), i, 0);
			
			inst.hitPoint = this.hitPoint;
			
			// Activate light on middle one
			if(i == 0)
				clone.GetComponent<Light>().enabled = true;
		}

		/*// Ignore collision between each other
		for(int i = 0; i < projectiles.Count; i++) {
			for(int h = i + 1; h < projectiles.Count; h++) {
				Physics.IgnoreCollision(projectiles[i].collider, projectiles[h].collider);
			}
		}*/
	}
}
