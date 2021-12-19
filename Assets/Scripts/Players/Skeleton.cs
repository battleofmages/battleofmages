using UnityEngine;

namespace BoM.Players {
	public class Skeleton : MonoBehaviour {
		public Transform leftHand;
		public Transform rightHand;

		public Vector3 handsCenter {
			get {
				return (leftHand.position + rightHand.position) / 2f;
			}
		}
	}
}
