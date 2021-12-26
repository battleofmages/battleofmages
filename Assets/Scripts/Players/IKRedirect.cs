using UnityEngine;

namespace BoM.Players {
	public class IKRedirect : MonoBehaviour {
		public IKBehaviour target;

		private void OnAnimatorIK(int layerIndex) {
			target.OnAnimatorIK(layerIndex);
		}
	}
}
