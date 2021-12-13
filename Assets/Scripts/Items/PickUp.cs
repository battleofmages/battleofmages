
using UnityEngine;

public class PickUp : MonoBehaviour {
	private void OnTriggerEnter(Collider other) {
		Destroy(gameObject);
	}
}
