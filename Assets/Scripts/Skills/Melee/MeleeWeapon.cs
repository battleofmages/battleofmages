using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Requires a rigidbody to have collisions with other objects
[RequireComponent(typeof(Rigidbody))]

public class MeleeWeapon : SkillInstance {
	public Transform impactExplosion;
	public AudioClip swordHitHumanSound;
	public AudioClip swordHitObjectSound;
	public AudioClip swordHitSwordSound;
	public GameObject sparksPrefab;
	
	// TODO: HashSet
	public Dictionary<GameObject, bool> hitList;
	
	// Start
	void Start() {
		hitList = new Dictionary<GameObject, bool>();
	}
	
	// ONLY FOR TESTING
	/*void Update() {
		caster.weaponModelCollider.enabled = true;
	}*/
	
	public static void DeflectProjectile(GameObject collidingObject, Entity newCaster) {
		var projectile = collidingObject.GetComponent<Projectile>();
		
		if(projectile != null) {
			projectile.rigidbody.AddForce(projectile.transform.forward * -projectile.projectileSpeed * 2);
			projectile.caster = newCaster;
			Entity.MoveToLayer(collidingObject.transform, newCaster.skillLayer);
		}
	}
	
	// Collision
	public void OnCollisionEnter(Collision collision) {
		// We prevent the weapon from hitting the same target twice
		if(!hitList.ContainsKey(collision.gameObject)) {
			hitList.Add(collision.gameObject, true);
			
			var collidingObject = collision.gameObject;

			if(collidingObject.tag == "Projectile") {
				MeleeWeapon.DeflectProjectile(collidingObject, caster);
			} else if(collidingObject.GetComponent<MeleeWeaponCollider>() != null) {
				if(!uLink.Network.isServer) {
					audio.PlayOneShot(swordHitSwordSound);
					Instantiate(sparksPrefab, collision.contacts[0].point, Cache.quaternionIdentity);
				}

				/*if(uLink.Network.isServer) {
					Vector3 diff = otherWeapon.transform.position - transform.position;
					caster.networkView.RPC("SwordClash", uLink.RPCMode.All, diff);
				} else {
					audio.PlayOneShot(swordHitSwordSound);
				}
				
				var casterAsPlayer = (Player)caster;
				casterAsPlayer.EndSkill();
				casterAsPlayer.StopMeleeAttack();*/
			} else {
				if(!uLink.Network.isServer) {
					if(collision.gameObject.GetComponent<Entity>() != null) {
						audio.PlayOneShot(swordHitHumanSound);
					} else {
						audio.PlayOneShot(swordHitObjectSound);
					}
				}
				
				// TODO: We changed caster.currentSkill to this due to code changes, check if this is still valid
				caster.SpawnExplosion(impactExplosion, collision, this); // caster.currentSkill
			}
		}
	}
}
