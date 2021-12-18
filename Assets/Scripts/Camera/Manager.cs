using UnityEngine;
using System.Collections.Generic;

namespace BoM.Camera {
	public class Manager : MonoBehaviour {
		public List<UnityEngine.Camera> cameras;
		public static Manager Instance { get; private set; }

		private void Awake() {
			Instance = this;
		}

		public static void AddCamera(UnityEngine.Camera cam) {
			Instance.cameras.Add(cam);
		}

		public static void RemoveCamera(UnityEngine.Camera cam) {
			Instance.cameras.Remove(cam);
		}

		public static void SetActiveCamera(UnityEngine.Camera activeCam) {
			foreach(var cam in Instance.cameras) {
				if(cam == activeCam) {
					cam.enabled = true;
				} else {
					cam.enabled = false;
				}
			}

			if(activeCam == null) {
				Instance.cameras[0].enabled = true;
			}
		}
	}
}
