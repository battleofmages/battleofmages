using UnityEngine;

namespace BoM.Players {
	public class Cursor : MonoBehaviour {
		public LayerMask LayerMask;
		public Camera Cam;
		public Vector3 Position { get; private set; }
		private RaycastHit hit;
		private Transform crossHair;

		private void Awake() {
			crossHair = GameObject.FindGameObjectWithTag("Crosshair").transform;
		}

		private void Update() {
			var ray = Cam.ScreenPointToRay(crossHair.position);

			if(Physics.Raycast(ray, out hit, LayerMask)) {
				Position = hit.point;
			} else {
				Position = Cam.transform.position + Cam.transform.forward * 100f;
			}
		}
	}
}
