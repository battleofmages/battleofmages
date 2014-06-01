using UnityEngine;
using System.Collections;

public class FireSpread : SkillInstance {
	public GameObject projectileSpawned;
	public FireNova skillInstanceParent;
	
	private float startRotationY;
	private Transform myTransform;
	
	// Start
	void Start() {
		Quaternion toCenter = Quaternion.LookRotation(skillInstanceParent.transform.position - this.transform.position);
		startRotationY = toCenter.eulerAngles.y;
		myTransform = this.transform;
		
		this.caster = skillInstanceParent.caster;
		this.skill = skillInstanceParent.skill;
		this.skillStage = skillInstanceParent.skillStage;
		this.gameObject.layer = skillInstanceParent.gameObject.layer;
		
		Invoke("Detonate", 2.0f);
	}
	
	// Detonate
	void Detonate() {
		for(int i = 0; i < 360; i += 36) {
			GameObject clone;
			SkillInstance inst;
			
			SpawnSkillPrefab(
				projectileSpawned,
				myTransform.position,
				Quaternion.AngleAxis(startRotationY + i, Vector3.up),
				out clone,
				out inst
			);
			
			// Destroy after a few seconds
			Destroy(clone, 7);
		}
	}
}
