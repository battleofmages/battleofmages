using UnityEngine;
using System.Collections;

public class ProjectileDeflector : MonoBehaviour {
	private MeleeWeapon parentWeapon;
	
	void Awake() {
		parentWeapon = this.transform.parent.GetComponent<MeleeWeapon>();
	}
	
	void OnTriggerEnter(Collider other) {
		if(other.tag == "Projectile") {
			MeleeWeapon.DeflectProjectile(other.gameObject, parentWeapon.caster);
		}
	}
}
