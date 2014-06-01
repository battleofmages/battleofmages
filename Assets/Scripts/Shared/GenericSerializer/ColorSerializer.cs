using UnityEngine;
using System.Collections;

public static class ColorSerializer {
	// Writer
	public static void JsonSerializer(Jboy.JsonWriter writer, object instance) {
		Color col = (Color)instance;
		
		writer.WriteObjectStart();
		writer.WritePropertyName("r");
		writer.WriteNumber(col.r);
		writer.WritePropertyName("g");
		writer.WriteNumber(col.g);
		writer.WritePropertyName("b");
		writer.WriteNumber(col.b);
		writer.WritePropertyName("a");
		writer.WriteNumber(col.a);
		writer.WriteObjectEnd();
	}
	
	// Reader
	public static object JsonDeserializer(Jboy.JsonReader reader) {
		reader.ReadObjectStart();
		reader.ReadPropertyName("r");
		var r = (float)reader.ReadNumber();
		reader.ReadPropertyName("g");
		var g = (float)reader.ReadNumber();
		reader.ReadPropertyName("b");
		var b = (float)reader.ReadNumber();
		reader.ReadPropertyName("a");
		var a = (float)reader.ReadNumber();
		reader.ReadObjectEnd();
		
		return new Color(r, g, b, a);
	}
}
