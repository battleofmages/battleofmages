using UnityEngine;
using System.Collections.Generic;

namespace BoM.Cameras {
	public class Manager : MonoBehaviour {
		public List<Camera> cameras;
		public static Manager Instance { get; private set; }
		private static Camera activeCamera;

		private void Awake() {
			Instance = this;
			activeCamera = Instance.cameras[0];
		}

		public static void AddCamera(Camera cam) {
			Instance.cameras.Add(cam);
		}

		public static void RemoveCamera(Camera cam) {
			Instance.cameras.Remove(cam);
		}

		public static Camera ActiveCamera {
			get {
				return activeCamera;
			}

			set {
				foreach(var cam in Instance.cameras) {
					if(cam == null) {
						continue;
					}

					if(cam == value) {
						cam.enabled = true;
						cam.gameObject.SetActive(true);
						activeCamera = cam;
					} else {
						cam.enabled = false;
						cam.gameObject.SetActive(false);
					}
				}
			}
		}
	}
}
