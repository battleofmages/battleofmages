using UnityEngine;
using System.Collections;

public class InputSettings {
	//public float mouseSensitivity;
	public InputControl[] controls;
	
	public InputSettings() {
		//mouseSensitivity = 0.5f;
		controls = new InputControl[0];
	}
	
	public InputSettings(InputManager inputMgr) {
		//mouseSensitivity = inputMgr.mouseSensitivity;
		controls = inputMgr.controls;
	}
	
	// Writer
	public static void JsonSerializer(Jboy.JsonWriter writer, object instance) {
		GenericSerializer.WriteJSONClassInstance<InputSettings>(writer, (InputSettings)instance);
	}
	
	// Reader
	public static object JsonDeserializer(Jboy.JsonReader reader) {
		return GenericSerializer.ReadJSONClassInstance<InputSettings>(reader);
	}
}
