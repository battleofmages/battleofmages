[System.Serializable]
public class GraphicsEffect {
	private bool _activated;
	public string name;
	public string componentName;
	public string prefsId;

	// Activated
	public bool activated {
		get {
			return _activated;
		}
		
		set {
			_activated = value;
		}
	}
}