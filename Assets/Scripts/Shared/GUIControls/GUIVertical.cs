using UnityEngine;

public class GUIVertical : System.IDisposable {
	// Constructor
	public GUIVertical(params GUILayoutOption[] options) {
		GUILayout.BeginVertical(options);
	}

	// Constructor
	public GUIVertical(GUIStyle style, params GUILayoutOption[] options) {
		GUILayout.BeginVertical(style, options);
	}

	// Constructor
	public GUIVertical(GUIContent content, GUIStyle style, params GUILayoutOption[] options) {
		GUILayout.BeginVertical(content, style, options);
	}

	// Constructor
	public GUIVertical(Texture image, GUIStyle style, params GUILayoutOption[] options) {
		GUILayout.BeginVertical(image, style, options);
	}

	// Constructor
	public GUIVertical(string text, GUIStyle style, params GUILayoutOption[] options) {
		GUILayout.BeginVertical(text, style, options);
	}
	
	// Dispose
	void System.IDisposable.Dispose() {
		GUILayout.EndVertical();
	}
}
