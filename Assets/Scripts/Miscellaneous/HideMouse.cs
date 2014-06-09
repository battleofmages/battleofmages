using UnityEngine;
using System.Collections;

public class HideMouse : MonoBehaviour {
	// Use this for initialization
	void Start() {
		Screen.showCursor = false;
		Screen.lockCursor = true;
	}
}
