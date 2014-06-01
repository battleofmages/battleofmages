using UnityEngine;
using System.Collections;

public class MultiProjectiles : SkillInstance {
	private Transform myTransform;
	
	// Use this for initialization
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
		}
	}
}
