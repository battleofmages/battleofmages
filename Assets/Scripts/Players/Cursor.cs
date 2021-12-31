using UnityEngine;

namespace BoM.Players {
	// Data
	public class CursorData : MonoBehaviour {
		public Vector3 Position { get; protected set; }
		public Ray Ray { get; protected set; }
		public LayerMask LayerMask { get; set; }

		[SerializeField] protected Camera Cam;
		protected RaycastHit hit;
		protected Transform crossHair;
	}

	// Logic
	public class Cursor : CursorData {
		public Vector3 FarPoint { get => Ray.origin + Ray.direction * 100f; }

		private void OnEnable() {
			crossHair = GameObject.FindGameObjectWithTag("Crosshair").transform;
		}

		private void Update() {
			Ray = Cam.ScreenPointToRay(crossHair.position);

			if(Physics.Raycast(Ray, out hit, LayerMask)) {
				Position = hit.point;
			} else {
				Position = FarPoint;
			}
		}
	}
}
