using UnityEngine;

namespace BoM.Players {
	// Data
	public class SkeletonData : MonoBehaviour {
		public Transform leftHand;
		public Transform rightHand;
		public Transform leftFoot;
		public Transform rightFoot;
	}

	// Logic
	public class Skeleton : SkeletonData {
		public Vector3 handsCenter { get => (leftHand.position + rightHand.position) * 0.5f; }
	}
}
