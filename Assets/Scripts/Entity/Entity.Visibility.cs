using UnityEngine;

public abstract partial class Entity : uLink.MonoBehaviour, PartyMember<Entity> {
	// Is visible
	private bool _isVisible = true;
	
#region Properties
	// --------------------------------------------------------------------------------
	// Properties
	// --------------------------------------------------------------------------------
	
	// Is visible
	public bool isVisible {
		get {
			return _isVisible;
		}

		set {
			_isVisible = value;

			//charGraphicsModel.gameObject.SetActive(_isVisible);
			
			//if(attunementVisuals != null)
			//	attunementVisuals.SetActive(_isVisible);
		}
	}
#endregion
	
#region RPCs
	// --------------------------------------------------------------------------------
	// RPCs
	// --------------------------------------------------------------------------------
	
	[RPC]
	public void Visible(Vector3 newServerPosition) {
		isVisible = true;
		nameLabel.enabled = true;
		//this.collisionLayer = this.party.layer;
		
		// Snap to current position
		serverPosition = newServerPosition;
		myTransform.position = serverPosition;
		
		//DSpamLog("Visible!");
	}
	
	[RPC]
	public void Invisible() {
		isVisible = false;
		nameLabel.enabled = false;
		//this.collisionLayer = Physics.kIgnoreRaycastLayer;
		
		//DSpamLog("Invisible!");
	}
#endregion
}