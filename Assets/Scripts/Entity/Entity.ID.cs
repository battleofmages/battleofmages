using UnityEngine;

public abstract partial class Entity : uLink.MonoBehaviour, PartyMember<Entity> {
	// Entity ID
	private ushort _id = IDCreator.UndefinedId;
	
	// InitID
	void InitID() {
		// Pick an ID on the server
		if(uLink.Network.isServer)
			id = IDCreator.GetNextID();
	}
	
#region Properties
	// --------------------------------------------------------------------------------
	// Properties
	// --------------------------------------------------------------------------------
	
	// ID
	public ushort id {
		get {
			return _id;
		}
		
		set {
			_id = value;
			Entity.idToEntity[_id] = this;
			
			if(uLink.Network.isServer)
				networkView.RPC("SetId", uLink.RPCMode.Others, id);
			
			UpdateName();
		}
	}
#endregion
	
#region RPCs
	// --------------------------------------------------------------------------------
	// RPCs
	// --------------------------------------------------------------------------------
	
	[RPC]
	protected void SetId(ushort newId) {
		id = newId;

		// Log it
		LogSpam("ID: " + id);
	}
#endregion
}
