using UnityEngine;

public class GenericSlider : MonoBehaviour {
	public float max;
	private float _current;
	
	// Awake
	void Awake() {
		current = max;
	}

#region Properties
	// Available
	public bool available {
		get {
			return current > 0;
		}
	}

	// Current
	public float current {
		get {
			return _current;
		}

		set {
			_current = value;

			if(_current < 0)
				_current = 0;
			else if(_current > max)
				current = max;
		}
	}
#endregion
}