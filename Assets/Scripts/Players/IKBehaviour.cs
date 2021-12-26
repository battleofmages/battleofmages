using UnityEngine;

namespace BoM.Players {
	public abstract class IKBehaviour : MonoBehaviour {
		public abstract void OnAnimatorIK(int layerIndex);
	}
}
