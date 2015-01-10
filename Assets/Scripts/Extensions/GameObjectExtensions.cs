using UnityEngine;

static class GameObjectExtensions {
	// MoveToLayer
	public static void MoveToLayer(this GameObject root, int layer) {
		root.layer = layer;
		
		foreach(Transform child in root.transform)
			child.MoveToLayer(layer);
	}
}