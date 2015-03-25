using UnityEngine;
using UnityEngine.UI;
using System.Globalization;

namespace BoM.UI.Lobby {
	// BuildVersion
	public class BuildVersion : MonoBehaviour {
		public Text textComponent;

		// Start
		void Start() {
			var date = Utility.GetBuildDate();
			var month = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(date.Month);
			var day = date.Day.ToString();
			var year = date.Year.ToString();

			textComponent.text = textComponent.text.Replace("{month}", month);
			textComponent.text = textComponent.text.Replace("{day}", day);
			textComponent.text = textComponent.text.Replace("{year}", year);
		}
	}
}