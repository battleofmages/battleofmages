using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace BoM.Cameras {
	// Data
	public class ControllerData : MonoBehaviour {
		[SerializeField] protected Camera cam;
		[SerializeField] protected Transform target;
		[SerializeField] protected Transform center;
		[SerializeField] protected LayerMask layerMask;
		[SerializeField] protected VolumeProfile volumeProfile;
		[SerializeField] protected float distanceAdjustmentSpeed;
		[SerializeField] protected float distanceToObstacle;

		protected float maxDistance;
		protected float targetDistance;
		protected float distance;
		protected RaycastHit hit;
		protected DepthOfField depthOfField;
		protected Vector3[] frustumCorners;
		protected Rect viewport;
	}

	// Logic
	public class Controller : ControllerData {
		private void Awake() {
			maxDistance = -target.localPosition.z;
			distance = maxDistance;
			targetDistance = maxDistance;
			frustumCorners = new Vector3[4];
			viewport = new Rect(0, 0, 1, 1);

			if(!volumeProfile.TryGet(out depthOfField)) {
				throw new System.NullReferenceException(nameof(depthOfField));
			}
		}

		private void Update() {
			cam.CalculateFrustumCorners(viewport, cam.nearClipPlane, Camera.MonoOrStereoscopicEye.Mono, frustumCorners);
			targetDistance = maxDistance;

			for(int i = 0; i < 4; i++) {
				var cornerPosition = cam.transform.TransformVector(frustumCorners[i]);

				if(!Physics.Raycast(center.position, cam.transform.position - center.position + cornerPosition, out hit, maxDistance + distanceToObstacle, layerMask)) {
					continue;
				}

				var cornerDistance = (hit.point - center.position).magnitude - distanceToObstacle;

				if(cornerDistance < targetDistance) {
					targetDistance = cornerDistance;
				}
			}

			distance = Mathf.Lerp(distance, targetDistance, Time.deltaTime * distanceAdjustmentSpeed);
			target.localPosition = new Vector3(0f, 0f, -distance);
			depthOfField.focusDistance.Override(distance);
		}
	}
}
