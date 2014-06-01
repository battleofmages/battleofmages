using uLobby;
using UnityEngine;
using System.Collections;

public sealed class ItemInventoryGUI : LobbyModule<ItemInventoryGUI> {
	public int inventorySlotSize;

	private Vector2 scrollPosition;
	
	// Start
	void Start() {
		// Receive lobby RPCs
		Lobby.AddListener(this);
	}

	// Draw
	public override void Draw() {
		var account = InGameLobby.instance.displayedAccount;

		if(account == null || !account.isMine)
			return;

		var inventory = account.itemInventory;
		DrawInventory(inventory, ref scrollPosition, account.isMine);
	}

	// DrawInventory
	public static void DrawInventory(Inventory inventory, ref Vector2 scrollPosition, bool enabled = true, bool descending = false) {
		if(inventory == null || inventory.bags == null)
			return;

		using(new GUIVertical("box")) {
			using(new GUIScrollView(ref scrollPosition)) {
				var bags = inventory.bags;

				if(bags.Length >= 1) {
					int i = bags.Length - 1;

					while(true) {
						var bag = bags[i];

						DrawBag(bag, i, enabled);

						// Loop
						if(descending) {
							i--;

							if(i < 0)
								break;
						} else {
							i++;

							if(i >= bags.Length)
								break;
						}
					}
				}
			}
		}
	}

	// DrawBag
	public static void DrawBag(Bag bag, int i, bool enabled) {
		if(bag == null)
			return;

		GUILayout.Label(_("L{0} inventory", i + 1));
		GUILayout.BeginHorizontal();
		for(int index = 0; index < bag.itemLimit; index++) {
			if(index % 10 == 0) {
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
			}
			
			var itemSlot = bag.itemSlots[index];
			GUILayoutOption[] options = {GUILayout.Width(ItemInventoryGUI.instance.inventorySlotSize), GUILayout.Height(ItemInventoryGUI.instance.inventorySlotSize)};
			
			if(itemSlot != null && itemSlot.item != null) {
				var arti = (Artifact)itemSlot.item;
				
				GUI.enabled = enabled;
				
				if(GUIHelper.Button(new GUIContent("", arti.icon, arti.tooltip), null, options)) { // && !saving
					if(Event.current.button == 0) {
						Lobby.RPC("ClientArtifactEquip", Lobby.lobby, arti.id);
					} else if(Event.current.button == 1) {
						byte level = (byte)i;
						byte slotIndex = (byte)index;
						
						new PopupMenu(
							ArtifactsGUI.instance.artifactContextMenuContents,
							new PopupMenu.CallBack[] {
							// Equip
							() => {
								Lobby.RPC("ClientArtifactEquip", Lobby.lobby, arti.id);
							},
							// Discard
							() => {
								new Confirm(
									"Do you really want to delete " + arti.name + " x" + itemSlot.count + "?",
									() => {
									Lobby.RPC("ClientArtifactDiscard", Lobby.lobby, level, slotIndex);
								},
								null
								);
							}
						}
						);
					}
				}
				
				if(itemSlot.count > 1)
					GUI.Label(GUILayoutUtility.GetLastRect(), itemSlot.count.ToString(), ArtifactsGUI.instance.itemCountStyle);
			} else {
				GUI.enabled = false;
				GUIHelper.Button(new GUIContent(""), null, options);
			}
		}
		
		GUI.enabled = true;
		
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
	}
	
	// --------------------------------------------------------------------------------
	// RPCs
	// --------------------------------------------------------------------------------

	[RPC]
	void ReceiveItemInventory(string accountId, string jsonInventory) {
		var inv = Jboy.Json.ReadObject<ItemInventory>(jsonInventory);
		LogManager.General.Log("ItemInventoryGUI: Received item inventory " + jsonInventory + "!");
		
		PlayerAccount.Get(accountId).itemInventory = inv;
	}
}
