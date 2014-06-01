using UnityEngine;
using System.Collections;

public class HandsParticles : CastEffect {
	public GameObject castParticlesLeftHand;
	public GameObject castParticlesRightHand;
	
	// Start
	void Start() {
		castParticlesLeftHand.transform.parent = this.caster.leftHand;
		castParticlesLeftHand.transform.localPosition = Vector3.zero;
		
		castParticlesRightHand.transform.parent = this.caster.rightHand;
		castParticlesRightHand.transform.localPosition = Vector3.zero;
		
		Destroy(this.gameObject, 3.0f);
	}
	
	// Stop
	public override void Stop() {
		if(castParticlesLeftHand != null)
			SkillInstance.StopEmitters(castParticlesLeftHand);
		
		if(castParticlesRightHand != null)
			SkillInstance.StopEmitters(castParticlesRightHand);
	}
}
