using UnityEngine;

public class DisableParticlesOnServer : MonoBehaviour {
	// Use this for initialization
	void Start() {
		if(!uLink.Network.isServer)
			return;
		
		if(particleEmitter != null) {
			particleEmitter.enabled = false;
			if(this.GetComponent<ParticleAnimator>().autodestruct)
				Destroy(this.gameObject);
		}
		
		if(particleSystem != null) {
			particleSystem.enableEmission = false;
			if(!particleSystem.isStopped)
				particleSystem.Stop();
		}
	}
}
