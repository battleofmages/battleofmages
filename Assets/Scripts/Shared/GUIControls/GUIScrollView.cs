using UnityEngine;

public class GUIScrollView : System.IDisposable {
	// Constructor
	public GUIScrollView(ref Vector2 scrollPosition, params GUILayoutOption[] options) {
		scrollPosition = GUILayout.BeginScrollView(scrollPosition, options);
	}
	
	// Dispose
	void System.IDisposable.Dispose() {
		GUILayout.EndScrollView();
	}
}
