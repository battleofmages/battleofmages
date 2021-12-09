using UnityEngine;
using System.Collections.Generic;

public class CameraManager : MonoBehaviour {
	public List<Camera> cameras;
	public static CameraManager Instance { get; private set; }

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
