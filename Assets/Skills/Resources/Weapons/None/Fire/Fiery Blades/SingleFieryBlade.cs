using UnityEngine;
using System.Collections;

public class SingleFieryBlade : SkillInstance {
	public static double explosionCooldown = 0.05d;
	
	public Transform explosionPrefab;
	public SkillInstance skillInstanceParent;
	private Transform myTransform;
	private double lastExplosionTime;
	
	// Use this for initialization
	void Start () {
		this.caster = skillInstanceParent.caster;
		this.skill = skillInstanceParent.skill;
		this.skillStage = skillInstanceParent.skillStage;
		this.gameObject.layer = skillInstanceParent.gameObject.layer;
		
		myTransform = this.transform;
	}
	
	void OnTriggerEnter(Collider coll) {
		// Do not collide with blades from other players
		if(uLink.Network.time - lastExplosionTime > explosionCooldown && coll.gameObject.GetComponent<SingleFieryBlade>() == null) {
			this.caster.SpawnExplosion(explosionPrefab, myTransform.position, Quaternion.identity, this);
			lastExplosionTime = uLink.Network.time;
		}
	}
}
