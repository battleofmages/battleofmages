using UnityEngine;

namespace BoM.Core {
	public class Singleton<T> : ScriptableObject where T : Singleton<T> {
		private static T instance;

		protected static T Instance {
			get {
				if(instance != null) {
					return instance;
				}

				var assets = Resources.LoadAll<T>("");

				if(assets.Length == 0) {
					throw new System.Exception($"Could not find any singleton type {nameof(T)} in the resources.");
				}

				if(assets.Length > 1) {
					throw new System.Exception($"Multiple instances of singleton type {nameof(T)} have been loaded.");
				}

				instance = assets[0];
				return instance;
			}
		}
	}
}
