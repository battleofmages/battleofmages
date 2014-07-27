using UnityEngine;

// Progress bar style
[System.Serializable]
public class ProgressBarStyle {
	public Color backgroundColor;
	public GUIStyle backgroundStyle;
	public Color backgroundFillColor;
	public GUIStyle backgroundFillStyle;
	public Color fillColor;
	public GUIStyle fillStyle;
	public Color textColor;
	public GUIStyle textStyle;
}

public class GUIHelper : MonoBehaviour {
	// Focuses a control and registers all key presses before that.
	// This is necessary to keep them from being buffered because
	// Input.eatKeyPressOnTextFieldFocus is now obsolete.
	public static void Focus(string controlName) {
		InputManager.instance.Clear();
		GUI.FocusControl(controlName);
	}
	
	// BeginBox
	public static void BeginBox(float x, float y, float width, float height, GUIStyle style = null) {
		GUILayout.BeginArea(new Rect(x, y, width, height));
		GUILayout.BeginVertical(style != null ? style : "box");

		//var padding = GUI.skin.box.padding;
		//GUILayout.BeginArea(new Rect(x + padding.left, y + padding.top, width - padding.left - padding.right, height - padding.top - padding.bottom));
	}

	// BeginBox
	public static void BeginBox(float width, float height) {
		BeginBox(Screen.width / 2 - width / 2, Screen.height / 2 - height / 2, width, height);
	}
	
	// BeginArea
	public static void BeginArea(int width, int height) {
		GUILayout.BeginArea(new Rect(Screen.width / 2 - width / 2, Screen.height / 2 - height / 2, width, height));
	}

	// EndBox
	public static void EndBox() {
		//GUILayout.FlexibleSpace();
		GUILayout.EndVertical();
		GUILayout.EndArea();
	}

	// PlayerNameField
	public static string PlayerNameField(string text) {
		return GUILayout.TextField(text, GameDB.maxPlayerNameLength).PrettifyPlayerName();
	}
	
	// Progress bar
	public static void ProgressBar(
		string text,
		float progress,
		string tooltip = null,
		ProgressBarStyle style = null
	) {
		if(style == null)
			style = Login.instance.progressBarStyle;
		
		if(progress < 0f)
			progress = 0f;
		else if(progress > 1f)
			progress = 1f;
		
		GUI.color = style.backgroundColor;
		GUILayout.Box("", style.backgroundStyle);
		
		var levelRect = GUILayoutUtility.GetLastRect();
		
		if(levelRect.Contains(Event.current.mousePosition))
			GUI.tooltip = tooltip;
		
		var levelFilledRect = new Rect(levelRect);
		levelFilledRect.x += 1;
		levelFilledRect.y += 1;
		levelFilledRect.width -= 2;
		levelFilledRect.height -= 2;
		
		GUI.color = style.backgroundFillColor;
		GUI.Box(levelFilledRect, "", style.backgroundFillStyle);
		
		levelFilledRect.width *= progress;
		
		GUI.color = style.fillColor;
		GUI.Box(levelFilledRect, "", style.fillStyle);
		
		GUI.color = style.textColor;
		GUI.Label(levelRect, text, style.textStyle);
	}
	
	// A horizontal slider which is vertically centered
	public static float HorizontalSliderVCenter(float val, float leftValue, float rightValue, params GUILayoutOption[] options) {
		GUILayout.BeginVertical(options);
		GUILayout.FlexibleSpace();
		val = GUILayout.HorizontalSlider(val, leftValue, rightValue);
		GUILayout.FlexibleSpace();
		GUILayout.EndVertical();
		
		return val;
	}

	// Rect picker
	public static void RectPicker(ref Rect rect) {
		rect.x = float.Parse(GUILayout.TextField(rect.x.ToString()));
		rect.y = float.Parse(GUILayout.TextField(rect.y.ToString()));
		rect.width = float.Parse(GUILayout.TextField(rect.width.ToString()));
		rect.height = float.Parse(GUILayout.TextField(rect.height.ToString()));
	}
	
	// Color picker
	public static Color ColorPicker(Color col) {
		return ColorPicker(col, new Vector3(0f, 0f, 0f), new Vector3(1f, 1f, 1f));
	}
	
	// Color picker
	public static Color ColorPicker(Color col, Vector3 min) {
		return ColorPicker(col, min, new Vector3(1f, 1f, 1f));
	}
	
	// Color picker
	public static Color ColorPicker(Color col, Vector3 min, Vector3 max, bool pickAlpha = false) {
		var oldColor = GUI.color;
		
		GUILayout.BeginHorizontal();
		GUI.color = Color.red;
		col.r = GUILayout.HorizontalSlider(col.r, min.x, max.x);
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		GUI.color = Color.green;
		col.g = GUILayout.HorizontalSlider(col.g, min.y, max.y);
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		GUI.color = Color.blue;
		col.b = GUILayout.HorizontalSlider(col.b, min.z, max.z);
		GUILayout.EndHorizontal();
		
		GUI.color = oldColor;
		
		if(pickAlpha)
			col.a = GUILayout.HorizontalSlider(col.a, 0f, 1f);
		else
			col.a = 1f;
		
		return col;
	}
	
	// Delegate for Shadowed
	public delegate void GUIShadowFunc(float x, float y);
	
	// Draw shadowed version
	public static void Shadowed(float x, float y, GUIShadowFunc func) {
		Shadowed(x, y, func, GUIColor.Shadow);
	}
	
	// Draw shadowed version
	public static void Shadowed(float x, float y, GUIShadowFunc func, Color shadowColor) {
		var originalColor = GUI.color;
		
		// Shadow
		GUI.color = new Color(shadowColor.r, shadowColor.g, shadowColor.b, shadowColor.a * originalColor.a);
		func(x + 1, y + 1);
		
		// Original
		GUI.color = originalColor;
		func(x, y);
	}
	
	// Not the best algorithm, but it'll work for a while...
	public static string MakePrettyVersion(int versionInt) {
		string version = versionInt.ToString();
		
		if(version.Length == 1)
			return "0.0." + version;
		
		if(version.Length == 2)
			return "0." + version.Substring(0, 1) + "." + version.Substring(1, 1);
		
		return version.Substring(0, 1) + "." + version.Substring(1, 1) + "." + version.Substring(2, 1);
	}
	
	// Fixes Unity focus bugs
	public static void UnityFocusFix() {
		// This fix here is for being able to focus a control that always exists.
		// If you try to focus controls that aren't always drawn, weird things happen.
		bool enabled = GUI.enabled;
		GUI.enabled = false;
		GUI.SetNextControlName("CLEAR_ALL_FOCUS_CONTROL");
		GUI.TextField(new Rect(-100, -100, 1, 1), "");
		GUI.enabled = enabled;
		
		// This fix here is for selecting text in a textarea.
		// The problem is selecting leads to hotControl never resetting to 0 again.
		// Therfore we fix it on the mouse up event.
		if(Event.current.type == EventType.MouseUp && GUIUtility.hotControl == GUIUtility.keyboardControl)
			GUIUtility.hotControl = 0;
	}
	
	// ClearAllFocus: MUST BE CALLED IN OnGUI()
	public static void ClearAllFocus() {
		GUI.FocusControl("CLEAR_ALL_FOCUS_CONTROL");
		
		GUIUtility.keyboardControl = 0;
		GUIUtility.hotControl = 0;
	}
	
	// Returns the number plus the correct plural version of the string (if included)
	public static string Plural(int count, string singular, string plural = "") {
		if(plural == "")
			plural = singular + "s";
		
		if(count == 1)
			return string.Concat(count, " ", singular);
		else
			return string.Concat(count, " ", plural);
	}
	
	public static bool HCenteredButton(string text, params GUILayoutOption[] options) {
		bool result;
		
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		result = GUIHelper.Button(text, options);
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		
		return result;
	}
	
	public static bool Button(string caption, params GUILayoutOption[] options) {
		return Button(new GUIContent(caption), null, -1, options);
	}
	
	public static bool Button(string caption, int popupControlID, params GUILayoutOption[] options) {
		return Button(new GUIContent(caption), null, popupControlID, options);
	}
	
	public static bool Button(string caption, GUIStyle btnStyle, params GUILayoutOption[] options) {
		return Button(new GUIContent(caption), btnStyle, -1, options);
	}
	
	public static bool Button(GUIContent content, params GUILayoutOption[] options) {
		return Button(content, null, -1, options);
	}
	
	public static bool Button(GUIContent content, GUIStyle btnStyle, params GUILayoutOption[] options) {
		return Button(content, btnStyle, -1, options);
	}
	
	public static bool Button(GUIContent content, GUIStyle btnStyle, int popupControlID, params GUILayoutOption[] options) {
		if(btnStyle == null)
			btnStyle = GUI.skin.button;
		
		Rect rect = GUILayoutUtility.GetRect(content, btnStyle, options);
		return Button(rect, content, btnStyle, popupControlID);
	}
	
	public static bool Button(Rect bounds, GUIContent content, GUIStyle btnStyle = null, int popupControlID = -1) {
		if(btnStyle == null)
			btnStyle = GUI.skin.button;
		
		int controlID = GUIUtility.GetControlID(bounds.GetHashCode(), FocusType.Passive);
		
		// Mouse position
		var mousePos = Event.current.mousePosition;
		
		bool isMouseOver = bounds.Contains(mousePos);
		bool isDown = GUIUtility.hotControl == controlID;
		
		if((GUIUtility.hotControl != 0 && (popupControlID == -1 || GUIUtility.hotControl != popupControlID)) && !isDown) {
			// ignore mouse while some other control has it
			// (this is the key bit that GUI.Button appears to be missing)
			isMouseOver = false;
		}
		
		if(isMouseOver)
			GUI.tooltip = content.tooltip;
		
		if(Event.current.type == EventType.Repaint) {
			btnStyle.Draw(bounds, content, isMouseOver, isDown, false, false);
		}
		
		switch (Event.current.GetTypeForControl(controlID)) {
		case EventType.mouseDown:
			if(isMouseOver) {
				// (note: isMouseOver will be false when another control is hot)
				GUIUtility.hotControl = controlID;
			}
			break;
		case EventType.mouseUp:
			if(GUIUtility.hotControl == controlID)
				GUIUtility.hotControl = 0;
			if(isMouseOver && bounds.Contains(mousePos))
				return true;
			break;
		}
		
		return false;
	}
	
	// Toolbar delegate
	public delegate bool IsAvailableFunc();
	
	public static int Toolbar(int selected, GUIContent[] contents, IsAvailableFunc[] availFuncs = null, params GUILayoutOption[] options) {
		var oldColor = GUI.backgroundColor;
		
		// Draw buttons
		for(int i = 0; i < contents.Length; i++) {
			if(selected == i) {
				if(availFuncs != null) {
					var availFunc = availFuncs[i];
					if(availFunc == null || availFunc()) {
						GUI.backgroundColor = GUIColor.MenuItemActive;
					} else {
						GUI.backgroundColor = GUIColor.MenuItemLoading;
					}
				} else {
					GUI.backgroundColor = GUIColor.MenuItemActive;
				}
			} else {
				GUI.backgroundColor = GUIColor.MenuItemInactive;
			}
			
			if(GUIHelper.Button(contents[i], options)) {
				selected = i;
			}
		}
		
		GUI.backgroundColor = oldColor;
		
		return selected;
	}
	
	public static bool ReturnPressed() {
		bool pressed = Event.current.type == EventType.KeyUp && Event.current.keyCode == KeyCode.Return;
		
		if(pressed)
			Event.current.Use();
		
		return pressed;
	}
}
