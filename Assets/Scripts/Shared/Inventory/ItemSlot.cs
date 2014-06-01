using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class ItemSlot {
	public Item item;
	public ulong count;

	// Constructor
	public ItemSlot() {
		Reset();
	}

	// Constructor
	public ItemSlot(Item nItem, ulong nCount = 1) {
		item = nItem;
		count = nCount;
	}

	// Constructor
	public ItemSlot(int itemId, ulong nCount = 1) {
		item = ItemFactory.CreateFromId(itemId);
		count = nCount;
	}

	// Reset
	public void Reset() {
		item = null;
		count = 0;
	}

	// Writer
	public static void JsonSerializer(Jboy.JsonWriter writer, object instance) {
		if(instance == null) {
			writer.WriteNull();
			return;
		}

		var slot = (ItemSlot)instance;

		writer.WriteObjectStart();

		// ID
		writer.WritePropertyName("item");
		Jboy.Json.WriteObject(slot.item, writer);

		// Count
		writer.WritePropertyName("count");
		writer.WriteNumber(slot.count);

		writer.WriteObjectEnd();
	}
	
	// Reader
	public static object JsonDeserializer(Jboy.JsonReader reader) {
		if(reader.TryReadNull())
			return null;
		
		var itemSlot = new ItemSlot();

		reader.ReadObjectStart();

		// ID
		reader.ReadPropertyName("item");
		itemSlot.item = Jboy.Json.ReadObject<Item>(reader);

		// Count
		reader.ReadPropertyName("count");
		itemSlot.count = (ulong)reader.ReadNumber();

		reader.ReadObjectEnd();
		
		return itemSlot;
	}
}
