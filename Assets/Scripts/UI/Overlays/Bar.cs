using UnityEngine;
using UnityEngine.UI;

namespace BoM.UI.Overlays {
	public class Bar : MonoBehaviour {
		public Image fill;
		public Gradient gradient;

		private void LateUpdate() {
			transform.LookAt(transform.position + Cameras.Manager.ActiveCamera.transform.forward);
		}

		public void SetFillAmount(float amount) {
			fill.fillAmount = amount;
			fill.color = gradient.Evaluate(amount);
		}
	}
}
