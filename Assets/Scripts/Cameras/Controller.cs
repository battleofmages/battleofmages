using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace BoM.Cameras {
	public class Controller : MonoBehaviour {
		[SerializeField] private Camera cam;
		[SerializeField] private Transform target;
		[SerializeField] private Transform center;
		[SerializeField] private LayerMask layerMask;
		[SerializeField] private VolumeProfile volumeProfile;
		[SerializeField] private float distanceAdjustmentSpeed;
		[SerializeField] private float distanceToObstacle;

		private float maxDistance;
		private float targetDistance;
		private float distance;
		private RaycastHit hit;
		private DepthOfField depthOfField;
		private Vector3[] frustumCorners;
		private Rect viewport;

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
