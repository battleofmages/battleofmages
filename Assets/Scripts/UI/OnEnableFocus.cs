using UnityEngine;
using UnityEngine.UI;

namespace BoM.UI {
	// OnEnableFocus
	public class OnEnableFocus : MonoBehaviour {
		// OnEnable
		void OnEnable() {
			GetComponent<Selectable>().Select();
		}
	}
}