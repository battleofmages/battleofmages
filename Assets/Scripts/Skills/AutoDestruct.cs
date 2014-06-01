using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]

public class AutoDestruct : MonoBehaviour {
	private ParticleSystem myParticleSystem;

	// Start
	void Start() {
		myParticleSystem = particleSystem;
		
		// Server side doesn't compute particles
		if(uLink.Network.isServer) {
			// Disable emission
			myParticleSystem.enableEmission = false;

			if(!myParticleSystem.isStopped)
				myParticleSystem.Stop();

			// Is there a delayed explosion component?
			var delayedExplosion = GetComponent<DelayedExplosion>();

			// Destroy this object after the standard duration of the particle system
			if(delayedExplosion == null)
				Destroy(gameObject, myParticleSystem.duration);
			else
				Destroy(gameObject, delayedExplosion.delayTime + 0.1f);

			// No LateUpdate on the server
			enabled = false;
		}
	}

	// LateUpdate
	void LateUpdate() {
		if(!myParticleSystem.IsAlive() || (myParticleSystem.particleCount == 0 && !myParticleSystem.enableEmission)) {
			if(!audio || !audio.isPlaying) {
				Destroy(gameObject);
			}
		}
	}
}
