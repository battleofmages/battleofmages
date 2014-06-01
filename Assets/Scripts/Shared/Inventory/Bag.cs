using System.Collections.Generic;

[System.Serializable]
public class Bag : JsonSerializable<Bag> {
	public int itemLimit;
	public List<ItemSlot> itemSlots;

	// Constructor
	public Bag() {
		itemLimit = 0;
		itemSlots = null;
	}

	// Constructor
	public Bag(int nItemLimit) {
		itemLimit = nItemLimit;
		itemSlots = new List<ItemSlot>(new ItemSlot[nItemLimit]);
	}

	// AddItem
	public void AddItem(int itemId, ulong count) {
		// Trying to find the item in the inventory and increasing its count
		int freePos = -1;
		for(int i = 0; i < itemSlots.Count; i++) {
			if(itemSlots[i] == null) {
				if(freePos == -1) {
					freePos = i;
				}
			} else {
				if(itemSlots[i].item != null && itemSlots[i].item.id == itemId) {
					itemSlots[i].count += count;
					return;
				}
			}
		}
		
		// New slot assigned
		if(freePos != -1) {
			itemSlots[freePos] = new ItemSlot(itemId, count);
			return;
		}
		
		// In case inventory is full
		itemSlots.Add(new ItemSlot(itemId, count));
	}

	// AddItem
	public void AddItem(Item item, ulong count) {
		AddItem(item.id, count);
	}

	// RemoveItem
	public void RemoveItem(int itemId, ulong count = 1) {
		ulong removed;

		for(int i = 0; i < itemSlots.Count; i++) {
			var itemSlot = itemSlots[i];

			if(itemSlot == null)
				continue;

			if(itemSlot.item == null)
				continue;
			
			if(itemSlot.item.id == itemId) {
				removed = System.Math.Min(count, itemSlot.count);

				itemSlot.count -= removed;
				count -= removed;

				if(itemSlot.count == 0) {
					itemSlots[i].Reset();
					itemSlots[i] = null;
				}

				if(count == 0)
					return;
			}
		}
	}

	// RemoveItem
	public void RemoveItem(Item item, ulong count = 1) {
		RemoveItem(item.id, count);
	}

	// RemoveItemSlot
	public void RemoveItemSlot(int slotId) {
		itemSlots[slotId].Reset();
		itemSlots[slotId] = null;
	}
}

