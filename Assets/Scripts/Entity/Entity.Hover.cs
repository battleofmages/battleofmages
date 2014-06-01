using UnityEngine;

public abstract partial class Entity : uLink.MonoBehaviour, PartyMember<Entity> {
#region RPCs
	// StartHover
	protected virtual bool StartHover() {
		return false;
	}
	
	// EndHover
	protected virtual bool EndHover() {
		return false;
	}
#endregion
}