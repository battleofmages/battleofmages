using UnityEngine;

public class MultiProjectiles : SkillInstance {
	private Transform myTransform;
	
	// Start
	void Start() {
		int childCount = this.gameObject.transform.childCount;
		myTransform = this.transform;
		
		for(int i = 0; i < childCount; i++) {
			GameObject child = myTransform.GetChild(i).gameObject;
			child.layer = this.gameObject.layer;
			
			SkillInstance inst = child.GetComponent<SkillInstance>();
			inst.caster = this.caster;
			inst.skill = this.skill;
			inst.skillStage = this.skillStage;
			inst.hitPoint = this.hitPoint;

			// Ignore collision with caster
			if(child.collider && caster.collider && child.collider.enabled && caster.collider.enabled)
				Physics.IgnoreCollision(caster.collider, child.collider);
		}
	}
}
