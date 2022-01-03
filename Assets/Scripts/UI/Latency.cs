using TMPro;
using UnityEngine;

namespace BoM.UI {
	// Data
	public class LatencyData : MonoBehaviour {
		[SerializeField] protected TextMeshProUGUI label;
		protected string baseText;
	}

	// Logic
	public class Latency : LatencyData {
		private void Start() {
			baseText = label.text;
		}

		public void OnLatencyReceived(float latency) {
			int ping = (int) (latency * 1000);
			label.text = $"{baseText} {ping} ms";
		}
	}
}
