using UnityEngine;
using System.Collections;

public class VisibilityTracker : MonoBehaviour {
	private bool _isVisible;
	
	public bool isVisible {
		get { return _isVisible; }
	}
	
	void OnBecameVisible() {
		_isVisible = true;
	}
	
	void OnBecameInvisible() {
		_isVisible = false;
	}
}
