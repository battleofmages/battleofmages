using UnityEngine;

namespace BoM.Core {
	public class Singleton<T> : ScriptableObject where T : Singleton<T> {
		protected static T instance;

		private void OnEnable() {
			if(instance != null) {
				throw new System.Exception($"Multiple instances of singleton type {this.GetType()} have been loaded.");
			}

			instance = (T) this;
		}
	}
}
