using System;

public static class EnumSerializer<T> {
	// Writer
	public static void WriteJSON(Jboy.JsonWriter writer, object instance) {
		T status = (T)instance;

		writer.WriteObjectStart();
		writer.WritePropertyName("v");
		writer.WriteNumber(Convert.ToInt32(status));
		writer.WriteObjectEnd();
	}
	
	// Reader
	public static object ReadJSON(Jboy.JsonReader reader) {
		reader.ReadObjectStart();
		reader.ReadPropertyName("v");
		int status = Convert.ToInt32(reader.ReadNumber());
		reader.ReadObjectEnd();

		return status;
	}
}
