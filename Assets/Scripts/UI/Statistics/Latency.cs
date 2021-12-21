using TMPro;
using UnityEngine;

namespace BoM.UI {
	public class Latency : MonoBehaviour {
		public TextMeshProUGUI label;
		private string baseText;

		private void Start() {
			baseText = label.text;
		}

		public void OnLatencyReceived(long latency) {
			label.text = $"{baseText} {latency} ms";
		}
	}
}
