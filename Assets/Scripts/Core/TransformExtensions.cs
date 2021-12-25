using UnityEngine;

namespace BoM.Core {
	public static class TransformExtensions {
		public static void SetLayer(this Transform root, int layer) {
			root.gameObject.layer = layer;

			for(int i = 0, count = root.childCount; i < count; i++) {
				root.GetChild(i).SetLayer(layer);
			}
		}
	}
}
