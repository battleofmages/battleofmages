using UnityEngine;
using System.Collections;

public static class ItemInventoryDB {
	// --------------------------------------------------------------------------------
	// AccountToItemInventory
	// --------------------------------------------------------------------------------
	
	// Get item inventory
	public static Coroutine GetItemInventory(string accountId, GameDB.ActionOnResult<ItemInventory> func) {
		return GameDB.instance.StartCoroutine(GameDB.Get<ItemInventory>(
			"AccountToItemInventory",
			accountId,
			func
		));
	}
}
