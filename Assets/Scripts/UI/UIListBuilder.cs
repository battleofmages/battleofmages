using UnityEngine;
using System.Collections.Generic;

namespace BoM.UI {
	// UIListBuilder
	public static class UIListBuilder<T> {
		public delegate void GameObjectCallBack(GameObject obj, T instance);

		// Build
		public static void Build(List<T> collection, GameObject prefab, Transform parent, GameObjectCallBack callBack, int offset = 0) {
			for(int i = 0; i < collection.Count; i++) {
				var instance = collection[i];
				var clone = (GameObject)Object.Instantiate(prefab);
				clone.transform.SetParent(parent, false);
				clone.transform.SetSiblingIndex(i + offset);
				
				callBack(clone, instance);
			}
		}

		// Build
		public static void Build(List<T> collection, GameObject prefab, GameObject parent, GameObjectCallBack callBack, int offset = 0) {
			Build(collection, prefab, parent.transform, callBack, offset);
		}
	}
}