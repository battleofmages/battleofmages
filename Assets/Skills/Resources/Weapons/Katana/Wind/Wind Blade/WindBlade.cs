using UnityEngine;
using System.Collections;

public class WindBlade : SkillInstance {
	public float duration;
	public float powerUpWeaponDamage;
	
	private Weapon casterWeapon;
	
	void Start () {
		Transform myTransform = this.transform;
		myTransform.parent = this.caster.weaponModel.transform.FindChild("BladePosition");
		myTransform.localPosition = Vector3.zero;
		myTransform.localRotation = Quaternion.AngleAxis(90, Vector3.right);
		
		casterWeapon = this.caster.currentWeapon;
		casterWeapon.damageMultiplier += powerUpWeaponDamage;
		
		Invoke("DisableWindBlade", duration);
	}
	
	void DisableWindBlade() {
		casterWeapon.damageMultiplier -= powerUpWeaponDamage;
		
		SkillInstance.DestroyButKeepParticles(this.gameObject);
	}
}
