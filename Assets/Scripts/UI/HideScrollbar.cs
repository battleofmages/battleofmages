using UnityEngine;
using UnityEngine.UI;

namespace BoM.UI {
	// HideScrollbar
	public class HideScrollbar : MonoBehaviour {
		public const float threshold = 0.99f;

		// Update
		void Update() {
			var scrollBar = GetComponent<Scrollbar>();

			if(scrollBar.size >= threshold)
				GetComponent<CanvasGroup>().alpha = 0f;
			else
				GetComponent<CanvasGroup>().alpha = 1f;
		}
	}
}