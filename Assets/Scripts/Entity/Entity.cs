using UnityEngine;
using BoM.Skills;
using System.Collections.Generic;

[SelectionBase]
public class Entity : MonoBehaviour {
	public static Dictionary<ushort, Entity> idToEntity = new Dictionary<ushort, Entity>();

	public Motor motor;
	public Health health;

	// ApplyDamage
	public void ApplyDamage(SkillInstance skillInstance, int power) {
		health.current -= power;
	}

	// Spawns an explosion using a Collision
	public void SpawnExplosion(GameObject explosionPrefab, Collision collision, SkillInstance copyInst) {
		// Rotate the object so that the y-axis faces along the normal of the surface
		ContactPoint contact = collision.contacts[0];
		Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
		Vector3 pos = contact.point;
		
		SpawnExplosion(explosionPrefab, pos, rot, copyInst);
	}
	
	// Spawns an explosion using position and rotation
	public void SpawnExplosion(GameObject explosionPrefab, Vector3 pos, Quaternion rot, SkillInstance copyInst) {
		// Sometimes we get NaN values as input
		if(!rot.IsValid())
			rot = Cache.quaternionIdentity;
		
		// Spawn it
		var explosion = (GameObject)Instantiate(explosionPrefab, pos, rot);
		explosion.transform.parent = Root.instance.skills;

		if(explosion == null || gameObject == null)
			return;

		// Set layer
		explosion.MoveToLayer(gameObject.layer);
		
		// Set caster
		SkillInstance inst = explosion.GetComponent<SkillInstance>();
		
		if(inst == null)
			LogManager.General.LogWarning("You forgot to give the Explosion an Explosion script component");
		
		// Set skill instance info
		inst.caster = this;
		/*inst.skill = copyInst.skill;
		inst.skillStage = copyInst.skillStage;*/
	}

	// OnDestroy
	void OnDestroy() {
		if(onDestroy != null)
			onDestroy();
	}

#region Log
	protected string logPrefix;

	// Debug log with player name as prefix
	protected void Log(object message) {
		LogManager.General.Log(logPrefix + message);
	}
	
	// Debug log warning with player name as prefix
	protected void LogWarning(object message) {
		LogManager.General.LogWarning(logPrefix + message);
	}
	
	// Debug log error with player name as prefix
	protected void LogError(object message) {
		LogManager.General.LogError(logPrefix + message);
	}
	
	// Debug log with player name as prefix
	protected void LogSpam(object message) {
		LogManager.Spam.Log(logPrefix + message);
	}
#endregion

#region Events
	// Death
	public event CallBack onDeath;

	// Destroy
	public event CallBack onDestroy;
	
	// Kill
	public event KillHandler onKill;
	
	// Gain experience
	public event ExperienceGainHandler onGainExperience;

	// Delegates
	public delegate void KillHandler(Entity killer, Entity target, short skillId);
	public delegate void ExperienceGainHandler(uint exp);
#endregion

#region Properties
	// Layer mask for enemies
	public int enemiesLayerMask {
		get {
			// Enemies are in all layers except for own layer and "ignore raycast" layer
			return ~(1 << gameObject.layer | Physics.kIgnoreRaycastLayer);
		}
	}

	// Party
	public Party party {
		get;
		set;
	}
#endregion

#region RPCs
	[RPC]
	protected void RegisterKill(ushort killerId, ushort victimId, short skillId) {
		Log("RegisterKill: " + killerId + ", " + victimId + ", " + skillId);
		
		// Find entities
		Entity victim;
		Entity killer;
		
		if(!Entity.idToEntity.TryGetValue(victimId, out victim)) {
			LogError("Could not find entity with ID: " + victimId);
			return;
		}
		
		if(!Entity.idToEntity.TryGetValue(killerId, out killer)) {
			LogError("Could not find entity with ID: " + killerId);
			return;
		}
		
		// Update stats
		/*killer.stats.total.kills += 1;
		victim.stats.total.deaths += 1;*/
		
		// On kill
		if(killer.onKill != null)
			killer.onKill(killer, victim, skillId);
		
		// On death
		if(victim.onDeath != null)
			victim.onDeath();
	}

	// GainExperience
	[RPC]
	public void GainExperience(uint exp) {
		if(onGainExperience != null)
			onGainExperience(exp);
	}
#endregion
}
