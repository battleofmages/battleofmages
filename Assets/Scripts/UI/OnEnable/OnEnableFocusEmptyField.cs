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
					LogManager.General.Log("[OnEnableFocusEmptyField] Focusing: " + field);
					field.Select();
					return;
				}
			}
		}
	}
}