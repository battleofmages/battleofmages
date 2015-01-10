public class JSONSerializable<T> where T : JSONSerializable<T>, new() {
	// Writer
	public static void WriteJSON(Jboy.JsonWriter writer, object instance) {
		GenericSerializer.WriteJSON<T>(writer, (T)instance);
	}
	
	// Reader
	public static object ReadJSON(Jboy.JsonReader reader) {
		return GenericSerializer.ReadJSON<T>(reader);
	}
	
	// Static constructor, called explicitly in GameDB.InitCodecs()
	static JSONSerializable() {
		InitCodec();
	}

	// TestJSONCodec
	public static void TestJSONCodec() {
		var obj = new T();
		
		var json = Jboy.Json.WriteObject(obj);
		LogManager.General.Log(json);
		
		var newObject = Jboy.Json.ReadObject<T>(json);
		LogManager.General.Log(Jboy.Json.WriteObject(newObject));
	}
	
	// InitCodec
	public static void InitCodec() {
		LogManager.Spam.Log("Registering JSON Codec: " + typeof(T));
		Jboy.Json.AddCodec<T>(ReadJSON, WriteJSON);
	}
}