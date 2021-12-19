using TMPro;
using UnityEngine;

namespace BoM.UI {
	public class FPSView : MonoBehaviour {
		public TextMeshProUGUI label;
		
		private int frames;
		private float lastUpdate;
		private string baseText;

		private void Start() {
			baseText = label.text;
		}

		private void Update() {
			frames++;

			if(Time.unscaledTime - lastUpdate >= 1f) {
				label.text = $"{baseText} {frames}";
				frames = 0;
				lastUpdate = Time.unscaledTime;
			}
		}
	}
}
