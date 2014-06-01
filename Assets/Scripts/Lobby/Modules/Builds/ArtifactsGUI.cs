using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using uLobby;

public sealed class ArtifactsGUI : LobbyModule<ArtifactsGUI> {
	public static byte InvalidDiscard = 255;
	
	public ArtifactName[] artifactNames;
	public GUIStyle artifactStyle;
	public GUIStyle artifactSlotStyle;
	public GUIStyle itemCountStyle;
	public Texture2D[] rarityIcons;
	
	public GUIContent[] artifactContextMenuContents;
	
	[HideInInspector]
	public ArtifactTree artifactTree;
	
	[HideInInspector]
	public ArtifactInventory artifactInventory;
	
	[HideInInspector]
	public int pendingArtifactTreeRequests = 0;
	
	//private LoginClientGUI loginClientGUI;
	//private InGameLobby inGameLobby;
	
	private string artifactTreeStatsInfo;
	
	private Vector2 inventoryScrollPosition;
	private Vector2 scrollPosition;

	// Start
	void Start() {
		//loginClientGUI = this.GetComponent<LoginClientGUI>();
		//inGameLobby = this.GetComponent<InGameLobby>();
		
		artifactTree = null;
		artifactInventory = null;
		//statsTree.Randomize();
		
		// Receive lobby RPCs
		Lobby.AddListener(this);
	}

	// GetArtifactName
	public string GetArtifactName(Artifact arti) {
		foreach(var artiName in artifactNames) {
			int count = 1;
			
			if(artiName.stats[0] == artiName.stats[1])
				count += 1;
			
			if(artiName.stats[0] == artiName.stats[2])
				count += 1;
			
			if(arti.HasStat(artiName.stats[0], count)) {
				if(count == 3)
					return artiName.name;
				
				count = 1;
				
				if(artiName.stats[1] == artiName.stats[2])
					count += 1;
				
				if(arti.HasStat(artiName.stats[1], count)) {
					if(count == 2)
						return artiName.name;
					
					if(arti.HasStat(artiName.stats[2], 1))
						return artiName.name;
				}
			}
		}
		
		return "Unknown Artifact";
	}

	// Draw
	public override void Draw() {
		using(new GUIHorizontal()) {
			var acc = InGameLobby.instance.displayedAccount;
			artifactInventory = acc.artifactInventory;

			ItemInventoryGUI.DrawInventory(artifactInventory, ref inventoryScrollPosition, acc.isMine, true);
			DrawArtifactSlots();
		}
	}

	// DrawArtifactSlots
	public void DrawArtifactSlots() {
		var acc = InGameLobby.instance.displayedAccount;
		artifactTree = acc.artifactTree;
		artifactTreeStatsInfo = acc.charStats.GetMultiLineStringCombined(artifactTree.charStats);
		
		if(artifactTree == null)
			return;
		
		using(new GUIVertical("box")) {
			using(new GUIScrollView(ref scrollPosition)) {
				GUI.Label(new Rect(5, 0, 200, 300), artifactTreeStatsInfo);
				
				for(int i = Artifact.maxLevel - 1; i >= 0; i--) {
					var slotLevel = artifactTree.slots[i];
					
					using(new GUIHorizontalCenter()) {
						for(int slotIndex = 0; slotIndex < slotLevel.Length; slotIndex++) {
							var slot = slotLevel[slotIndex];
							
							if(slot.artifact != null) {
								if(GUIHelper.Button(new GUIContent("", slot.artifact.icon, slot.artifact.tooltip), artifactStyle) && acc.isMine) { // && !saving
									Lobby.RPC("ClientArtifactUnequip", Lobby.lobby, (byte)i, (byte)slotIndex);
								}
							} else {
								GUIHelper.Button(_("L{0}", slot.requiredLevel + 1), artifactSlotStyle);
							}
						}
					}
				}
			}
		}
	}
	
#region RPCs
	// --------------------------------------------------------------------------------
	// RPCs
	// --------------------------------------------------------------------------------
	
	[RPC]
	void ReceiveArtifactTree(string accountId, string jsonTree) {
		var acc = PlayerAccount.Get(accountId);
		acc.artifactTree = Jboy.Json.ReadObject<ArtifactTree>(jsonTree);
		
		LogManager.General.Log("ArtifactsGUI: Received artifact tree!");
	}
	
	[RPC]
	void ReceiveArtifactInventory(string accountId, string jsonTree) {
		var acc = PlayerAccount.Get(accountId);
		acc.artifactInventory = Jboy.Json.ReadObject<ArtifactInventory>(jsonTree);
		
		LogManager.General.Log("ArtifactsGUI: Received artifact inventory!");
	}
	
	[RPC]
	void ArtifactEquip(int itemId) {
		var arti = new Artifact(itemId);
		var acc = PlayerAccount.mine;
		
		if(acc.artifactTree.AddArtifact(itemId)) {
			acc.artifactInventory.RemoveArtifact(arti);
			
			// Live update on game server
			if(Player.main != null)
				Player.main.networkView.RPC("ArtifactTreeUpdate", uLink.RPCMode.Server, Jboy.Json.WriteObject(artifactTree));
		}
	}
	
	[RPC]
	void ArtifactUnequip(byte level, byte slotIndex) {
		var slot = artifactTree.slots[level][slotIndex];
		if(slot.artifact == null)
			return;
		
		var acc = PlayerAccount.mine;
		
		acc.artifactInventory.AddArtifact(slot.artifact);
		slot.artifact = null;
		
		// Live update on game server
		if(Player.main != null)
			Player.main.networkView.RPC("ArtifactTreeUpdate", uLink.RPCMode.Server, Jboy.Json.WriteObject(artifactTree));
	}
	
	[RPC]
	void ArtifactDiscard(byte level, byte slotIndex) {
		var inv = artifactInventory.bags[level];
		inv.RemoveItemSlot(slotIndex);
	}

	[RPC]
	void ArtifactInventorySaveError() {
		LogManager.General.LogError("ArtifactsGUI: Couldn't save inventory!");
	}
	
	/*[RPC]
	void ArtifactTreeSaveSuccess() {
		LogManager.General.Log("ArtifactsGUI: Successfully saved the artifact tree and inventory!");
	}*/
#endregion
}
