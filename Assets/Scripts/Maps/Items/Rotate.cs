using UnityEngine;

namespace BoM.Maps.Items {
	public class Rotate : MonoBehaviour {
		public Vector3 axis;
		public float speed;

		private void Update() {
			transform.Rotate(axis, speed * Time.deltaTime);
		}
	}
}
