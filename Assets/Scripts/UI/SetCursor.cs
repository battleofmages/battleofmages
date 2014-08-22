using UnityEngine;

public class SetCursor : MonoBehaviour {
	public Texture2D cursorIcon;
	public Vector2 cursorHotSpot;
	public CursorMode cursorMode;

	// Start
	void Start() {
		// Cursor
		Cursor.SetCursor(cursorIcon, cursorHotSpot, cursorMode);
	}
}
