using UnityEngine;

public class GUIHorizontalCenter : System.IDisposable {
	// Constructor
	public GUIHorizontalCenter(params GUILayoutOption[] options) {
		GUILayout.BeginHorizontal(options);
		GUILayout.FlexibleSpace();
	}

	// Constructor
	public GUIHorizontalCenter(GUIStyle style, params GUILayoutOption[] options) {
		GUILayout.BeginHorizontal(style, options);
		GUILayout.FlexibleSpace();
	}

	// Constructor
	public GUIHorizontalCenter(GUIContent content, GUIStyle style, params GUILayoutOption[] options) {
		GUILayout.BeginHorizontal(content, style, options);
		GUILayout.FlexibleSpace();
	}

	// Constructor
	public GUIHorizontalCenter(Texture image, GUIStyle style, params GUILayoutOption[] options) {
		GUILayout.BeginHorizontal(image, style, options);
		GUILayout.FlexibleSpace();
	}

	// Constructor
	public GUIHorizontalCenter(string text, GUIStyle style, params GUILayoutOption[] options) {
		GUILayout.BeginHorizontal(text, style, options);
		GUILayout.FlexibleSpace();
	}
	
	// Dispose
	void System.IDisposable.Dispose() {
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
	}
}
