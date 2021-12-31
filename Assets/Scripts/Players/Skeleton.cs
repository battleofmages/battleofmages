using UnityEngine;

namespace BoM.Players {
	// Data
	public class SkeletonData : MonoBehaviour {
		public Transform LeftHand;
		public Transform RightHand;
		public Transform LeftFoot;
		public Transform RightFoot;
	}

	// Logic
	public class Skeleton : SkeletonData {
		public Vector3 HandsCenter { get => (LeftHand.position + RightHand.position) * 0.5f; }
	}
}
