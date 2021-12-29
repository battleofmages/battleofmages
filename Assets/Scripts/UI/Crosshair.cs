using UnityEngine;
using UnityEngine.UI;

namespace BoM.UI {
	public class Crosshair : MonoBehaviour {
		public CanvasScaler canvasScaler;
		private RectTransform rectTransform;
		private float localY;

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
