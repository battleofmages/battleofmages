using UnityEngine;

static class TransformExtensions {
	// MoveToLayer
	public static void MoveToLayer(this Transform root, int layer) {
		root.gameObject.layer = layer;
		
		foreach(Transform child in root)
			child.MoveToLayer(layer);
	}

	// DeleteChildrenWithComponent
	public static void DeleteChildrenWithComponent<T>(this Transform root) {
		foreach(Transform child in root) {
			if(child.GetComponent<T>() != null)
				GameObject.Destroy(child.gameObject);
		}
	}
}
