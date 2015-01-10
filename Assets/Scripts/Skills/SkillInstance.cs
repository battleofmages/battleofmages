using UnityEngine;
using System.Collections.Generic;

public class SkillInstance : uLink.MonoBehaviour {
	[System.NonSerialized]
	public Entity caster;

	// DetachParticles
	public static void DetachParticles(GameObject obj) {
		var particleTransforms = new List<Transform>(4);
		
		// Check all children
		foreach(Transform child in obj.transform) {
			if(child.tag != "Particles")
				continue;
			
			// Add to list for editing later.
			// We can't edit the transform here because
			// that would result in incorrect loop behaviour.
			particleTransforms.Add(child);
		}
		
		// Move to new root
		foreach(Transform child in particleTransforms) {
			child.GetComponent<ParticleSystem>().enableEmission = false;
			child.parent = Root.instance.particles;
		}
	}
}