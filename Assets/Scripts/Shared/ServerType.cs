public enum ServerType {
	Arena,
	FFA,
	Town,
	World
}

public static class ServerTypeSerializer {
	// Writer
	public static void JsonSerializer(Jboy.JsonWriter writer, object instance) {
		writer.TryWritePropertyName("serverType");
		writer.WriteString(((ServerType)instance).ToString());
	}
	
	// Reader
	public static object JsonDeserializer(Jboy.JsonReader reader) {
		reader.TryReadPropertyName("serverType");
		switch(reader.ReadString()) {
			case "Arena":
				return ServerType.Arena;
				
			case "FFA":
				return ServerType.FFA;
				
			case "Town":
				return ServerType.Town;
				
			case "World":
				return ServerType.World;
				
			default:
				throw new System.ArgumentException("Unknown server type");
		}
	}
}