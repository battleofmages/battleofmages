
using UnityEngine;

namespace BoM.Maps.Items {
	public class PickUp : MonoBehaviour {
		private void OnTriggerEnter(Collider other) {
			Destroy(gameObject);
		}
	}
}
