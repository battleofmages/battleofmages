using UnityEngine;
using UnityEngine.UI;

public class OnEnableFocus : MonoBehaviour {
	// OnEnable
	void OnEnable() {
		GetComponent<Selectable>().Select();
	}
}
