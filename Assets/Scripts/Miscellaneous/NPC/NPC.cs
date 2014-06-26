using UnityEngine;

[RequireComponent(typeof(SphereCollider))]

public class NPC : Entity,  ActionTarget {
	public string componentName;
	
	private NPCModule module;

	// Start
	void Start() {
		// Disable colliders on server
		// Don't use if(uLink.Network.isServer) because we might not have initialized the server yet
		if(Application.loadedLevelName == "Server") {
			collider.enabled = false;
		}

		// NPC text color
		/*var entityLabel = GetComponent<EntityLabel>();
		if(entityLabel != null) {
			entityLabel.textColor = Config.instance.npcLabelColor;
		}*/
	}
	
	// Action key
	public void OnAction(Entity entity) {
		OnTriggerEnter(entity.collider);
	}
	
	public bool CanAction(Entity entity) {
		return ((Player)entity).talkingWithNPC == null;
	}
	
	// Player comes in range
	void OnTriggerEnter(Collider coll) {
		if(!GameManager.isPvE)
			return;
		
		if(Player.main == null)
			return;
		
		if(coll != Player.main.collider)
			return;
		
		Player.main.actionTarget = this;
		Player.main.talkingWithNPC = this;
		
		if(InGameLobby.instance)
			InGameLobby.instance.displayedAccount = PlayerAccount.mine;

		// Chat alpha
		//Camera.main.GetComponent<ChatGUI>().msgColor = new Color(1f, 1f, 1f, 0.1f);

		// Disable mouse look
		//GameObject.FindGameObjectWithTag("CamPivot").GetComponent<ToggleMouseLook>().DisableMouseLook();
		
		if(InGameLobby.instance != null && module == null) {
			if(componentName == "InGameLobby") {
				module = (NPCModule)InGameLobby.instance;
			} else if(componentName == "MusicManager") {
				module = (NPCModule)MusicManager.instance;
			} else {
				module = (NPCModule)InGameLobby.instance.GetComponent(componentName);
			}
		}
		
		if(module != null)
			module.OnNPCEnter();
	}
	
	// Player goes out of range
	void OnTriggerExit(Collider coll) {
		if(GameManager.serverType != ServerType.Town)
			return;
		
		if(Player.main == null)
			return;
		
		if(coll != Player.main.collider)
			return;
		
		Player.main.actionTarget = null;
		Player.main.talkingWithNPC = null;
		
		if(InGameLobby.instance)
			InGameLobby.instance.displayedAccount = PlayerAccount.mine;

		// Chat alpha
		//Camera.main.GetComponent<ChatGUI>().msgColor = new Color(1f, 1f, 1f, 1.0f);

		// Disable mouse look
		//GameObject.FindGameObjectWithTag("CamPivot").GetComponent<ToggleMouseLook>().EnableMouseLook();
		
		if(module != null)
			module.OnNPCExit();
	}
	
	public void Draw() {
		if(uLink.Network.isServer)
			return;
		
		if(Player.main == null)
			return;
		
		if(Player.main.talkingWithNPC != this)
			return;
		
		if(module == null)
			return;
		
		//using(new GUIArea(Screen.width * 0.75f, Screen.height * 0.75f)) {
		module.Draw();
		//}
	}
}
