using UnityEngine;
using System.Collections;

public class Explosion : MonoBehaviour {
	private void Start() {
		Destroy(gameObject, 1f);
	}
}
