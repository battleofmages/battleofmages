using UnityEngine;

public class RotateY : MonoBehaviour {
	public float rotationSpeed;

	private void Update() {
		transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
	}
}
