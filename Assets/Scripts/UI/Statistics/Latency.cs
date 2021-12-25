using TMPro;
using UnityEngine;

namespace BoM.UI {
	public class Latency : MonoBehaviour {
		public TextMeshProUGUI label;
		private string baseText;

		private void Start() {
			baseText = label.text;
		}

		public void OnLatencyReceived(float latency) {
			int ping = (int) (latency * 1000);
			label.text = $"{baseText} {ping} ms";
		}
	}
}
