using System;
using UnityEngine;

namespace BoM.UI {
	public class ScreenSize : MonoBehaviour {
		public static event Action<Vector2Int> Changed;
		public static event Action<int> WidthChanged;
		public static event Action<int> HeightChanged;
		private Vector2Int resolution;

		private void Update() {
			bool widthChanged = resolution.x != Screen.width;
			bool heightChanged = resolution.y != Screen.height;

			if(!widthChanged && !heightChanged) {
				return;
			}

			if(widthChanged) {
				resolution.x = Screen.width;
				WidthChanged?.Invoke(resolution.x);
			}

			if(heightChanged) {
				resolution.y = Screen.height;
				HeightChanged?.Invoke(resolution.y);
			}

			Changed?.Invoke(resolution);
		}
	}
}
