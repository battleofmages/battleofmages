using UnityEngine;

namespace BoM.UI.Overlays {
	public class FaceCamera : MonoBehaviour {
		private void LateUpdate() {
			transform.rotation = Cameras.Manager.ActiveCamera.transform.rotation;
		}
	}
}
