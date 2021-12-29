using BoM.Core;
using UnityEngine;

namespace BoM.UI.Overlays {
	public class FaceCamera : MonoBehaviour {
		public bool constantScreenSize;
		public float scaleMultiplier;
		private float referenceScale;

		private void Start() {
			referenceScale = transform.localScale.x;
		}

		private void LateUpdate() {
			var cam = Cameras.Manager.ActiveCamera;
			transform.rotation = cam.transform.rotation;

			if(constantScreenSize) {
				var distanceToCamera = (transform.position - cam.transform.position).magnitude;
				transform.localScale = referenceScale * scaleMultiplier * distanceToCamera * Const.OneVector;
			}
		}
	}
}
