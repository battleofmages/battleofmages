using UnityEngine;

namespace BoM.Players {
	public class Cursor : MonoBehaviour {
		public LayerMask LayerMask;
		public Camera Cam;
		public Vector3 Position;
		private RaycastHit hit;

		private void Update() {
			if(Physics.Raycast(Cam.transform.position, Cam.transform.forward, out hit, LayerMask)) {
				Position = hit.point;
			} else {
				Position = Cam.transform.position + Cam.transform.forward * 100f;
			}
		}
	}
}
