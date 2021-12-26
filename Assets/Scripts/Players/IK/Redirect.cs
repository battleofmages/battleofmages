using UnityEngine;

namespace BoM.Players.IK {
	public class Redirect : MonoBehaviour {
		public Behaviour target;

		private void OnAnimatorIK(int layerIndex) {
			target.OnAnimatorIK(layerIndex);
		}
	}
}
