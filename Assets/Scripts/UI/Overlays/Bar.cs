using UnityEngine;
using UnityEngine.UI;

namespace BoM.UI.Overlays {
	public class Bar : MonoBehaviour {
		public Image fill;
		public Gradient gradient;
		public float lerpTime;

		public void SetFillAmount(float amount) {
			var oldAmount = fill.fillAmount;
			var difference = amount - oldAmount;
			this.Fade(lerpTime, progress => SetFillAmountImmediately(oldAmount + difference * progress));
		}

		public void SetFillAmountImmediately(float amount) {
			fill.fillAmount = amount;
			fill.color = gradient.Evaluate(amount);
		}
	}
}
