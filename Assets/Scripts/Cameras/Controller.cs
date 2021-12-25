using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace BoM.Cameras {
	public class Controller : MonoBehaviour {
		public Transform target;
		public Transform center;
		public LayerMask layerMask;
		public VolumeProfile volumeProfile;
		public float distanceAdjustmentSpeed;
		private float maxDistance;
		private float targetDistance;
		private float distance;
		private RaycastHit hit;
		private DepthOfField depthOfField;

		private void Awake() {
			maxDistance = -target.localPosition.z;
			distance = maxDistance;
			targetDistance = maxDistance;

			if(!volumeProfile.TryGet(out depthOfField)) {
				throw new System.NullReferenceException(nameof(depthOfField));
			}
		}

		private void Update() {
			if(Physics.Raycast(center.position, -center.forward, out hit, maxDistance, layerMask)) {
				targetDistance = (hit.point - center.position).magnitude;
			} else {
				targetDistance = maxDistance;
			}

			distance = Mathf.Lerp(distance, targetDistance, Time.deltaTime * distanceAdjustmentSpeed);
			target.localPosition = new Vector3(0f, 0f, -distance);
			depthOfField.focusDistance.Override(distance);
		}
	}
}
