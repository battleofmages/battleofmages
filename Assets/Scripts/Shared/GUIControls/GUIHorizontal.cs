using UnityEngine;

public class GUIHorizontal : System.IDisposable {
	// Constructor
	public GUIHorizontal(params GUILayoutOption[] options) {
		GUILayout.BeginHorizontal(options);
	}

	// Constructor
	public GUIHorizontal(GUIStyle style, params GUILayoutOption[] options) {
		GUILayout.BeginHorizontal(style, options);
	}

	// Constructor
	public GUIHorizontal(GUIContent content, GUIStyle style, params GUILayoutOption[] options) {
		GUILayout.BeginHorizontal(content, style, options);
	}

	// Constructor
	public GUIHorizontal(Texture image, GUIStyle style, params GUILayoutOption[] options) {
		GUILayout.BeginHorizontal(image, style, options);
	}

	// Constructor
	public GUIHorizontal(string text, GUIStyle style, params GUILayoutOption[] options) {
		GUILayout.BeginHorizontal(text, style, options);
	}
	
	// Dispose
	void System.IDisposable.Dispose() {
		GUILayout.EndHorizontal();
	}
}
