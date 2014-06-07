using UnityEngine;

public class SummonWeapon : SkillInstance {
	public GameObject weapon;
	
	// Start
	void Start() {
		// Destroy old weapon of the caster
		if(caster.weaponModel != null) {
			Destroy(caster.weaponModel);
		}
		
		if(weapon != null) {
			GameObject weaponInstance = (GameObject)Object.Instantiate(weapon);
			
			weaponInstance.transform.parent = caster.weaponBone;
			weaponInstance.transform.localRotation = Quaternion.Euler(180f, 0f, -90f); //Quaternion.AngleAxis(90, Vector3.left)
			weaponInstance.transform.localPosition = Cache.vector3Zero; //new Vector3(-0.1f, 0, 0.055f); //-caster.rightHand.transform.right * 0.1f;
			
			SkillInstance inst = weaponInstance.GetComponent<SkillInstance>();
			inst.caster = caster;
			inst.skill = skill;
			inst.skillStage = skillStage;
			
			Entity.MoveToLayer(weaponInstance.transform, caster.gameObject.layer);
			
			// Make this the current weapon
			caster.weaponModel = weaponInstance;
			caster.weaponModelCollider = weaponInstance.transform.FindChild("Collider").collider;
			caster.weaponModelProjectileDeflector = weaponInstance.transform.FindChild("ProjectileDeflector").collider;
			
			// Disable collider
			caster.weaponModelCollider.enabled = false;
			caster.weaponModelProjectileDeflector.enabled = false;
		} else {
			caster.weaponModel = null;
			caster.weaponModelCollider = null;
			caster.weaponModelProjectileDeflector = null;
		}
	}
}
