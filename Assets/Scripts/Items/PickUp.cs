
using UnityEngine;

namespace BoM.Items {
	public class PickUp : MonoBehaviour {
		private void OnTriggerEnter(Collider other) {
			Destroy(gameObject);
		}
	}
}
