using UnityEngine;

public class SkillInstance : uLink.MonoBehaviour {
	private Entity _caster;
	private Skill _skill;
	private Skill.Stage _skillStage;
	private Vector3 _hitPoint;

	// Constructor
	public SkillInstance(Skill skill = null, Entity castedBy = null) {
		_caster = castedBy;
		_skill = skill;
		
		if(_skill != null)
			_skillStage = _skill.currentStage;
	}

	// SpawnSkillPrefab
	public void SpawnSkillPrefab(
		GameObject prefabSpawned,
		Vector3 position,
		Quaternion rotation,
		out GameObject clone,
		out SkillInstance inst
	) {
		clone = (GameObject)Object.Instantiate(prefabSpawned, position, rotation);
		clone.transform.parent = Entity.skillInstancesRoot;
		clone.layer = gameObject.layer;
		
		inst = clone.GetComponent<SkillInstance>();
		inst.caster = caster;
		inst.skill = skill;
		inst.skillStage = skillStage;
	}
	
#region Particles
	public static void DestroyButKeepParticles(GameObject go) {
		// Stop emission
		StopEmitters(go);
		
		Destroy(go);
		
		// Destroy it
		/*if(go.particleSystem) {
			if(go.collider) {
				go.collider.enabled = false;
			}
			
			if(go.rigidbody) {
				go.rigidbody.velocity = Cache.vector3Zero;
				go.rigidbody.angularVelocity = Cache.vector3Zero;
				go.rigidbody.Sleep();
			}
		} else {
			// Audio source
			//if(go.audio) {
			//	go.audio.transform.parent = null;
			//} else {
				// Legacy particle system
				Destroy(go);
			//}
		}*/
	}
	
	// StopEmitters
	public static void StopEmitters(GameObject go) {
		LightningRenderer lightningRenderer;
		
		// Stop emitting
		foreach(Transform child in go.transform){
			if(child.particleEmitter != null)
				DetachParticleEmitter(child.particleEmitter);
			
			if(child.particleSystem != null)
				DetachParticleSystem(child.particleSystem);
			
			lightningRenderer = child.GetComponent<LightningRenderer>();
			if(lightningRenderer)
				lightningRenderer.FadeOut(1.0f);
		}
		
		// Own particle emitter
		if(go.particleEmitter)
			DetachParticleEmitter(go.particleEmitter);
		
		if(go.particleSystem)
			DetachParticleSystem(go.particleSystem);
		
		lightningRenderer = go.GetComponent<LightningRenderer>();
		if(lightningRenderer)
			lightningRenderer.FadeOut(1.0f);
		
		/*if(trailRenderer) {
			trailRenderer.enabled = false;
		}*/
	}
	
	// DetachParticleEmitter
	public static void DetachParticleEmitter(ParticleEmitter emitter) {
		// This stops the emitter from creating more particles
		emitter.emit = false;
		
		// This splits the particle off so it doesn't get deleted with the parent
		emitter.transform.parent = Config.particlesRoot;
		
		// This finds the particleAnimator associated with the emitter and then
		// sets it to automatically delete itself when it runs out of particles
		//emitter.GetComponent<ParticleAnimator>().autoDestruct = true;
	}
	
	// DetachParticleSystem
	public static void DetachParticleSystem(ParticleSystem pSys) {
		// This stops the emitter from creating more particles
		pSys.enableEmission = false;
		
		// This splits the particle off so it doesn't get deleted with the parent
		pSys.transform.parent = Config.particlesRoot;
	}
	
	// DetachTrail
	public static void DetachTrail(TrailRenderer tr) {
		if(tr)
			tr.transform.parent = Config.particlesRoot;
	}
#endregion
	
#region Properties
	// Caster
	public Entity caster {
		get { return _caster; }
		set { _caster = value; }
	}

	// Skill
	public Skill skill {
		get { return _skill; }
		set {
			_skill = value;
			//if(_skill != null)
			//	_skillStage = _skill.currentStage;
		}
	}

	// Skill stage
	public Skill.Stage skillStage {
		get { return _skillStage; }
		set { _skillStage = value; }
	}

	// Hit point
	public Vector3 hitPoint {
		get { return _hitPoint; }
		set { _hitPoint = value; }
	}
#endregion
}
