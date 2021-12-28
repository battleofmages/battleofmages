using UnityEngine;

namespace BoM.Players {
	public class Cursor : MonoBehaviour {
		public LayerMask LayerMask;
		public Camera Cam;
		public Vector3 Position { get; private set; }
		public Ray Ray;
		private RaycastHit hit;
		private Transform crossHair;

		private void OnEnable() {
			crossHair = GameObject.FindGameObjectWithTag("Crosshair").transform;
		}

		private void Update() {
			Ray = Cam.ScreenPointToRay(crossHair.position);

			if(Physics.Raycast(Ray, out hit, LayerMask)) {
				Position = hit.point;
			} else {
				Position = Cam.transform.position + Cam.transform.forward * 100f;
			}
		}
	}
}
