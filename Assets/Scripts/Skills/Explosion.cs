using UnityEngine;

namespace BoM.Skills {
	public class Explosion : MonoBehaviour {
		private void Start() {
			Destroy(gameObject, 1f);
		}
	}
}
