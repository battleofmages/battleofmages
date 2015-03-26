using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace BoM.UI {
	// OnEnableFocus
	public class OnEnableFocus : MonoBehaviour {
		// The input fields we check for empty text
		public InputField[] fields;
		
		// OnEnable
		void OnEnable() {
			// Select it once it's active
			StartCoroutine(SelectWhenActive(GetComponent<Selectable>()));
		}

		// OnDisable
		void OnDisable() {
			StopAllCoroutines();
		}
		
		// SelectWhenActive
		public static IEnumerator SelectWhenActive(Selectable selectable, int maxTries = 10) {
			int count = 0;

			// Wait until the selectable is active
			while(!selectable.IsActive() && count <= maxTries) {
				count += 1;
				yield return null;
			}
			
			// Select it!
			selectable.Select();
		}
	}
}