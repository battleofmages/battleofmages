using UnityEngine;

public class Projectile : MonoBehaviour {
	public float speed;
	public Rigidbody rigidBody;

	void OnEnable() {
		rigidBody.AddRelativeForce(Vector3.forward * speed);
	}
}
