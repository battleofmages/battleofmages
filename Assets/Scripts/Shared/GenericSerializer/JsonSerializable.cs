public class JsonSerializable<T> where T : JsonSerializable<T>, new() {
	// Writer
	public static void JsonSerializer(Jboy.JsonWriter writer, object instance) {
		GenericSerializer.WriteJSONClassInstance<T>(writer, (T)instance);
	}
	
	// Reader
	public static object JsonDeserializer(Jboy.JsonReader reader) {
		return GenericSerializer.ReadJSONClassInstance<T>(reader);
	}
	
	// Static constructor, called explicitly in GameDB.InitCodecs()
	static JsonSerializable() {
		InitCodec();
	}
	
	// InitCodec
	public static void InitCodec() {
		LogManager.Spam.Log("Registering JSON Codec: " + typeof(T).ToString());
		Jboy.Json.AddCodec<T>(JsonDeserializer, JsonSerializer);
	}
}