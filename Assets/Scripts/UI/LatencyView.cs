using TMPro;
using UnityEngine;

namespace BoM.UI {
	public class LatencyView : MonoBehaviour {
		public TextMeshProUGUI label;
		private string baseText;

		private void Start() {
			baseText = label.text;
		}

		public void OnLatencyReceived(long latencyIn, long latencyOut) {
			label.text = $"{baseText} {latencyOut} ms";
		}
	}
}
