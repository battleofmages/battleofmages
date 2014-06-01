using UnityEngine;

public class ConstantRotationAll : MonoBehaviour {
	public float rotationSpeed = 300f;

	// Update
	void Update() {
		float rota = rotationSpeed * Time.deltaTime;

		transform.Rotate(rota, rota, rota);
	}
}
