public static class ItemSerializer {
	// Writer
	public static void JsonSerializer(Jboy.JsonWriter writer, object instance) {
		if(instance == null) {
			writer.WriteNull();
			return;
		}
		
		var item = (Item)instance;
		
		writer.WriteObjectStart();
		
		// ID
		writer.WritePropertyName("id");
		writer.WriteNumber(item.id);
		
		// Meta data
		item.WriteItemMetaData(writer);
		
		writer.WriteObjectEnd();
	}
	
	// Reader
	public static object JsonDeserializer(Jboy.JsonReader reader) {
		if(reader.TryReadNull())
			return null;
		
		reader.ReadObjectStart();
		
		// ID
		reader.ReadPropertyName("id");
		var itemId = (int)reader.ReadNumber();
		var item = ItemFactory.CreateFromId(itemId);
		
		// Meta data
		if(item != null)
			item.ReadItemMetaData(reader);
		
		reader.ReadObjectEnd();
		
		return item;
	}
}