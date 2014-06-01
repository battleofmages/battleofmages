using UnityEngine;
using System.Collections;

public class FollowCasterHands : CastEffect {
	private Transform myTransform;
	
	// Start
	void Start() {
		myTransform = this.transform;
		myTransform.position = this.caster.rightHand.transform.position;
		myTransform.rotation = this.caster.charGraphics.rotation;
	}
	
	// Update
	void Update () {
		if(!this.caster || !this.caster.rightHand)
			return;
		
		Vector3 rightHandPos = this.caster.rightHand.transform.position;
		
		myTransform.position = rightHandPos;
		
		/*myTransform.position = new Vector3(
			rightHandPos.x,
			myTransform.position.y,
			rightHandPos.z
		);*/
		
		/*myTransform.eulerAngles = new Vector3(
			myTransform.eulerAngles.x,
			this.caster.charGraphics.eulerAngles.y,
			myTransform.eulerAngles.z
		);*/
	}
	
	// Stop
	public override void Stop() {
		var children = myTransform.GetComponentsInChildren<ParticleSystem>();
		foreach(var child in children)
			child.enableEmission = false;
		
		/*foreach(Transform obj in this.transform) {
			SkillInstance.StopEmitters(obj.gameObject);
		}*/
	}
}
