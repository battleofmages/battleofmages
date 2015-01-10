using UnityEngine;

static class TransformExtensions {
	// MoveToLayer
	public static void MoveToLayer(this Transform root, int layer) {
		root.gameObject.layer = layer;
		
		foreach(Transform child in root)
			child.MoveToLayer(layer);
	}
}
