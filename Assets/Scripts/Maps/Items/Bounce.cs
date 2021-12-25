using UnityEngine;

namespace BoM.Maps.Items {
	public class Bounce : MonoBehaviour {
		public Vector3 bounceVector;
		public float bounceSpeed;
		private Vector3 startPosition;

		private void Start() {
			startPosition = transform.localPosition;
		}

		private void Update() {
			transform.localPosition = startPosition + bounceVector * Mathf.Sin(Time.time * bounceSpeed);
		}
	}
}
