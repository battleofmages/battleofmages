using UnityEngine;
using UnityEngine.UI;

namespace BoM.UI {
	// Data
	public class CrosshairData : MonoBehaviour {
		[SerializeField] protected CanvasScaler canvasScaler;
		protected RectTransform rectTransform;
		protected float localY;
	}

	// Logic
	public class Crosshair : CrosshairData {
		private void Awake() {
			rectTransform = GetComponent<RectTransform>();
			localY = rectTransform.localPosition.y;
			ScreenSize.HeightChanged += Reposition;
		}

		private void Reposition(int screenHeight) {
			var scale = screenHeight / canvasScaler.referenceResolution.y;
			rectTransform.localPosition = new Vector3(0f, localY * scale, 0f);
		}
	}
}
