using UnityEngine;
using UnityEngine.UI;

namespace BoM.UI {
	// OnEnableFocusEmptyField
	public class OnEnableFocusEmptyField : MonoBehaviour {
		public InputField[] fields;

		// OnEnable
		void OnEnable() {
			foreach(var field in fields) {
				if(string.IsNullOrEmpty(field.text)) {
					field.Select();
					return;
				}
			}
		}
	}
}