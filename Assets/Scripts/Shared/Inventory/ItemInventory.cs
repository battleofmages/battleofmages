public class ItemInventory : InventorySerializer<ItemInventory>, Inventory {
	public const int defaultBagCount = 1;
	public const int defaultSize = 40;
	
	// Bags
	public Bag[] bags { get; set; }
	
	// Constructor
	public ItemInventory() {
		bags = new Bag[defaultBagCount];
		
		for(int i = 0; i < defaultBagCount; i++) {
			bags[i] = new Bag(defaultSize);
		}
	}
}