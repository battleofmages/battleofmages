using TMPro;
using UnityEngine;

namespace BoM.UI {
	// Data
	public class FPSData : MonoBehaviour {
		[SerializeField] protected TextMeshProUGUI label;
		protected int frames;
		protected float lastUpdate;
		protected string baseText;
	}

	// Logic
	public class FPS : FPSData {
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
