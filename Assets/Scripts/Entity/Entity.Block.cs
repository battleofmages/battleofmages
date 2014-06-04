using UnityEngine;
using System;

public abstract partial class Entity : uLink.MonoBehaviour, PartyMember<Entity> {
	[NonSerialized]
	public Transform blockSphere;

	// Blocking enabled?
	public bool blockingEnabled;
	
#region Methods
	// Constructor
	void InitBlock() {
		// Center
		centerOffset = new Vector3(0, height / 2, 0);

		// The center offset isn't accurate because of customizable height.
		// We modify it later on when we receive character customization.

		// Position block sphere to center
		blockSphere = InstantiateChild(Config.instance.blockSphere);
		blockSphere.localPosition = centerOffset;

		// Scale block sphere
		var sphereScale = height * 1.2f;
		blockSphere.localScale = new Vector3(sphereScale, sphereScale, sphereScale);

		// Disabled at the beginning
		blockSphere.gameObject.SetActive(blocking);
		blockSphere.collider.enabled = false;
	}
#endregion

#region RPCs
	[RPC]
	protected void StartBlock() {
		if(blocking)
			return;
		
		if(!uLink.Network.isServer) {
			animator.SetBool("Block", true);
			animator.SetBool("Moving", false);
		}
		
		blockSphere.gameObject.SetActive(true);
		blockSphere.collider.enabled = true;
		blocking = true;
		
		if(currentSkill != null)
			InterruptCast();
		
		//Camera.main.GetComponent<BlurEffect>().enabled = true;
	}
	
	[RPC]
	protected void EndBlock() {
		if(!blocking)
			return;
		
		if(!uLink.Network.isServer) {
			animator.SetBool("Block", false);
		}
		
		blockSphere.gameObject.SetActive(false);
		blockSphere.collider.enabled = false;
		blocking = false;
		
		//Camera.main.GetComponent<BlurEffect>().enabled = false;
	}
#endregion
}
