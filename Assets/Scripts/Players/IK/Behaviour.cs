using UnityEngine;

namespace BoM.Players.IK {
	public abstract class Behaviour : MonoBehaviour {
		public abstract void OnAnimatorIK(int layerIndex);
	}
}
