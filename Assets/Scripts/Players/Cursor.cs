using UnityEngine;

namespace BoM.Players {
	public class Cursor : MonoBehaviour {
		public Vector3 Position { get; private set; }
		public LayerMask LayerMask { get; set; }

		[SerializeField] private Camera Cam;
		[SerializeField] private Ray Ray;

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
				Position = Ray.origin + Ray.direction * 100f;
			}
		}
	}
}
