using UnityEngine;

namespace BoM.Maps.Items {
	// Data
	public class BounceData : MonoBehaviour {
		public Vector3 Vector;
		public float Speed;

		protected Vector3 startPosition;
	}

	// Logic
	public class Bounce : BounceData {
		private void Start() {
			startPosition = transform.localPosition;
		}

		private void Update() {
			transform.localPosition = startPosition + Mathf.Sin(Time.time * Speed) * Vector;
		}
	}
}
