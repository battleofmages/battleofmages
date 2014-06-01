using UnityEngine;
using System.Collections;

public static class Texture2DSerializer {
	// Writer
	public static void JsonSerializer(Jboy.JsonWriter writer, object instance) {
		writer.WriteNull();
		/*var tex = (Texture2D)instance;
		writer.WriteObjectStart();
		
		writer.WritePropertyName("width");
		writer.WriteNumber(tex.width);
		
		writer.WritePropertyName("height");
		writer.WriteNumber(tex.height);
		
		writer.WritePropertyName("data");
		Jboy.Json.WriteObject(tex.EncodeToPNG(), writer);
		
		writer.WriteObjectEnd();*/
	}
	
	// Reader
	public static object JsonDeserializer(Jboy.JsonReader reader) {
		reader.ReadNull();
		return null; //new Texture2D(64, 64);
	}
}
