using UnityEngine;

public class SetCursor : MonoBehaviour, Initializable {
	public Texture2D cursorIcon;
	public Vector2 cursorHotSpot;
	public CursorMode cursorMode;
	
	// Init
	public void Init() {
		// Cursor
		Cursor.SetCursor(cursorIcon, cursorHotSpot, cursorMode);
	}
}