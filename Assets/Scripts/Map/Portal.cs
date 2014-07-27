using uLobby;
using UnityEngine;

public class Portal : MonoBehaviour, ActionTarget {
	public ServerType serverType = ServerType.World;
	public int portalId;
	public int targetPortalId;
	public Transform[] spawns;

	// Player comes in range
	void OnTriggerEnter(Collider coll) {
		if(!GameManager.isPvE)
			return;
		
		if(Player.main == null)
			return;
		
		if(coll != Player.main.collider)
			return;

		// Set action target
		Player.main.actionTarget = this;
	}

	// Player goes out of range
	void OnTriggerExit(Collider coll) {
		if(!GameManager.isPvE)
			return;
		
		if(Player.main == null)
			return;
		
		if(coll != Player.main.collider)
			return;

		// Reset action target
		Player.main.actionTarget = null;
	}

	// OnAction
	public void OnAction(Entity entity) {
		if(!enabled)
			return;

		// We save it here because the game object might be destroyed in the lambda
		string mapNameCached = mapName;

		// Send a request to the lobby to change the map
		Lobby.RPC("ActivatePortal", Lobby.lobby, mapNameCached, serverType, portalId);

		// Disable portal
		enabled = false;

		// Activate loading screen
		LoadingScreen.instance.Enable(() => {
			LoadingScreen.instance.statusMessage = "Teleporting to: <color=yellow>" + mapNameCached + "</color>";
		});
	}

	// Map name
	public string mapName {
		get {
			return gameObject.name;
		}
	}
	
	// CanAction
	public bool CanAction(Entity entity) {
		return entity.canCast;
	}

	// GetCursorPosition
	public Vector3 GetCursorPosition() {
		return transform.collider.bounds.center;
	}
}