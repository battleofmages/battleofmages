public interface Item {
	int id { get; }

	void WriteItemMetaData(Jboy.JsonWriter writer);
	void ReadItemMetaData(Jboy.JsonReader reader);
}