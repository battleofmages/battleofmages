using UnityEngine;
using System.Collections.Generic;

namespace BoM.Cameras {
	public class Manager : MonoBehaviour {
		public List<Camera> cameras;
		public static Manager Instance { get; private set; }

		private void Awake() {
			Instance = this;
		}

		public static void AddCamera(Camera cam) {
			Instance.cameras.Add(cam);
		}

		public static void RemoveCamera(Camera cam) {
			Instance.cameras.Remove(cam);
		}

		public static void SetActiveCamera(Camera activeCam) {
			foreach(var cam in Instance.cameras) {
				if(cam == null) {
					continue;
				}
				
				cam.enabled = (cam == activeCam);
			}

			if(activeCam == null) {
				Instance.cameras[0].enabled = true;
			}
		}
	}
}
