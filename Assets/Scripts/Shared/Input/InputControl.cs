using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class InputControl {
	public string name;
	public string id;
	public string description;
	public KeyCode keyCode;
	public KeyCode altKeyCode;
	public KeyCode gamePadKeyCode;
	public bool active = true;
	
	// Key code string
	public string keyCodeString {
		get {
			return InputControl.KeyCodeToString(keyCode);
		}
	}
	
	// Alt key code string
	public string altKeyCodeString {
		get {
			return InputControl.KeyCodeToString(altKeyCode);
		}
	}
	
	// Key codes active
	public bool keyCodesActive {
		get {
			return keyCode != KeyCode.None || altKeyCode != KeyCode.None;
		}
	}
	
	// Capture a key
	public KeyCode Capture(bool altKey = false) {
		KeyCode kc = Event.current.keyCode;

		// Mouse buttons
		for(int i = 0; i < 6; i++) {
			if(Input.GetMouseButtonDown(i)) {
				LogManager.General.Log("[InputControl] Captured mouse button: " + i);
				kc = (KeyCode)(KeyCode.Mouse0 + i);
			}
		}
		
		if(kc == KeyCode.None) {
			if(Input.GetKey(KeyCode.LeftShift)) {
				LogManager.General.Log("[InputControl] Captured left shift");
				kc = KeyCode.LeftShift;
			} else if(Input.GetKey(KeyCode.RightShift)) {
				LogManager.General.Log("[InputControl] Captured right shift");
				kc = KeyCode.RightShift;
			} else {
				// Gamepad support
				for(KeyCode i = KeyCode.JoystickButton0; i <= KeyCode.JoystickButton19; i++) {
					if(Input.GetKey(i)) {
						LogManager.General.Log("[InputControl] Captured gamepad key: " + i);
						kc = i;
						break;
					}
				}
			}
		}

		if(kc == KeyCode.None)
			return KeyCode.None;
		
		if(altKey)
			altKeyCode = kc;
		else
			keyCode = kc;
		
		return kc;
	}
	
	// Erase
	public void Erase(bool altKey = false) {
		if(altKey)
			altKeyCode = KeyCode.None;
		else
			keyCode = KeyCode.None;
	}
	
	// Writer
	public static void JsonSerializer(Jboy.JsonWriter writer, object instance) {
		var control = (InputControl)instance;
		if(!control.active)
			return;
		
		var fieldFilter = new HashSet<string>() {
			"id",
			"keyCode",
			"altKeyCode",
			//"gamePadKeyCode",
		};
		GenericSerializer.WriteJSONClassInstance<InputControl>(writer, control, fieldFilter);
	}
	
	// Reader
	public static object JsonDeserializer(Jboy.JsonReader reader) {
		return GenericSerializer.ReadJSONClassInstance<InputControl>(reader);
	}
	
	// Key code to string
	public static string KeyCodeToString(KeyCode keyCode) {
		if(keyCode >= KeyCode.Alpha0 && keyCode <= KeyCode.Alpha9)
			return (keyCode - KeyCode.Alpha0).ToString();
		
		if(keyCode >= KeyCode.Mouse0 && keyCode <= KeyCode.Mouse6) {
			switch(keyCode) {
				case KeyCode.Mouse0:
					return "Left mouse button";
				case KeyCode.Mouse1:
					return "Right mouse button";
				case KeyCode.Mouse2:
					return "Middle mouse button";
				default:
					return "Mouse extra button " + (keyCode - KeyCode.Mouse2).ToString();
			}
		}
		
		return keyCode.ToString();
	}
}
