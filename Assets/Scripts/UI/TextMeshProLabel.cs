using BoM.Core;
using TMPro;

namespace BoM.UI {
	public class TextMeshProLabel : Label {
		public TextMeshProUGUI label;

		public override void SetText(string text) {
			label.SetText(text);
		}
	}
}
