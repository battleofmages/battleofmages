using BoM.Core;
using UnityEngine;
using System.Collections.Generic;

namespace BoM.Cameras {
	[CreateAssetMenu(fileName = "Camera Manager", menuName = "BoM/Camera Manager", order = 100)]
	public class Manager : Singleton<Manager> {
		public static List<Camera> Cameras { get { return Instance.cameras; } }
		private static Camera activeCamera;
		private List<Camera> cameras;

		private void OnEnable() {
			cameras = new List<Camera>();
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
