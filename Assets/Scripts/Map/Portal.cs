using uLobby;
using UnityEngine;

public class Portal : MonoBehaviour, ActionTarget {
	public ServerType serverType = ServerType.World;
	public int portalId;
	public int targetPortalId;

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
		Lobby.RPC("ActivatePortal", Lobby.lobby, gameObject.name, serverType, portalId);
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