using UnityEngine;
using UnityEngine.UI;

// FPSCounter
// Fixed update rate of 1 second, it doesn't support dynamic update rates.
public class FPSCounter : Counter {
	// Update
	void Update() {
		counter += 1;
	}
}