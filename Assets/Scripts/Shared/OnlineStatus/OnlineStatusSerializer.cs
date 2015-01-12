public static class OnlineStatusSerializer {
	// Writer
	public static void WriteJSON(Jboy.JsonWriter writer, object instance) {
		OnlineStatus status = (OnlineStatus)instance;

		writer.WriteObjectStart();
		writer.WritePropertyName("status");
		writer.WriteNumber((int)status);
		writer.WriteObjectEnd();
	}
	
	// Reader
	public static object ReadJSON(Jboy.JsonReader reader) {
		reader.ReadObjectStart();
		reader.ReadPropertyName("status");
		OnlineStatus status = (OnlineStatus)((int)reader.ReadNumber());
		reader.ReadObjectEnd();
		
		return status;
	}
}
