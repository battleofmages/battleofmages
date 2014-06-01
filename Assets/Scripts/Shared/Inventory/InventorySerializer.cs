public class InventorySerializer<T> where T : Inventory, new() {
	// Writer
	public static void JsonSerializer(Jboy.JsonWriter writer, object instance) {
		var inv = (T)instance;
		
		writer.WriteArrayStart();
		for(var i = 0; i < inv.bags.Length; i++) {
			GenericSerializer.WriteJSONClassInstance<Bag>(writer, inv.bags[i]);
		}
		writer.WriteArrayEnd();
	}

	// Reader
	public static object JsonDeserializer(Jboy.JsonReader reader) {
		var inv = new T();
		
		reader.ReadArrayStart();
		for(var i = 0; i < inv.bags.Length; i++) {
			inv.bags[i] = GenericSerializer.ReadJSONClassInstance<Bag>(reader);
		}
		reader.ReadArrayEnd();
		
		return inv;
	}
}
