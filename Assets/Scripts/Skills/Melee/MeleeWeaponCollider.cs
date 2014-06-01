using UnityEngine;

public class MeleeWeaponCollider : MonoBehaviour {
	private MeleeWeapon parentWeapon;

	// Awake
	void Awake() {
		parentWeapon = transform.parent.GetComponent<MeleeWeapon>();
	}

	// OnCollisionEnter
	void OnCollisionEnter(Collision collision) {
		parentWeapon.OnCollisionEnter(collision);
	}
}
