using UnityEngine;

public class GUIVerticalCenter : System.IDisposable {
	// Constructor
	public GUIVerticalCenter(params GUILayoutOption[] options) {
		GUILayout.BeginVertical(options);
		GUILayout.FlexibleSpace();
	}

	// Constructor
	public GUIVerticalCenter(GUIStyle style, params GUILayoutOption[] options) {
		GUILayout.BeginVertical(style, options);
		GUILayout.FlexibleSpace();
	}

	// Constructor
	public GUIVerticalCenter(GUIContent content, GUIStyle style, params GUILayoutOption[] options) {
		GUILayout.BeginVertical(content, style, options);
		GUILayout.FlexibleSpace();
	}

	// Constructor
	public GUIVerticalCenter(Texture image, GUIStyle style, params GUILayoutOption[] options) {
		GUILayout.BeginVertical(image, style, options);
		GUILayout.FlexibleSpace();
	}

	// Constructor
	public GUIVerticalCenter(string text, GUIStyle style, params GUILayoutOption[] options) {
		GUILayout.BeginVertical(text, style, options);
		GUILayout.FlexibleSpace();
	}
	
	// Dispose
	void System.IDisposable.Dispose() {
		GUILayout.FlexibleSpace();
		GUILayout.EndVertical();
	}
}
