using UnityEngine;

public class Bounce : MonoBehaviour {
	public Vector3 bounceVector;
	public float bounceSpeed;
	private Vector3 startPosition;

	private void Start() {
		startPosition = transform.position;
	}

	private void Update() {
		transform.position = startPosition + bounceVector * Mathf.Sin(Time.time * bounceSpeed);
	}
}
