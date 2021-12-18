using UnityEngine;

namespace BoM.Skill {
	public class Explosion : MonoBehaviour {
		private void Start() {
			Destroy(gameObject, 1f);
		}
	}
}
