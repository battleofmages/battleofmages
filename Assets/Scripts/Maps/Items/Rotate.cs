using UnityEngine;

namespace BoM.Maps.Items {
	// Data
	public class RotateData : MonoBehaviour {
		public Vector3 Axis;
		public float Speed;
	}

	// Logic
	public class Rotate : RotateData {
		private void Update() {
			transform.Rotate(Axis, Speed * Time.deltaTime);
		}
	}
}
