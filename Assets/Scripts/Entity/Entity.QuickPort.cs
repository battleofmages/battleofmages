using UnityEngine;

public abstract partial class Entity : uLink.MonoBehaviour, PartyMember<Entity> {
	protected double lastQuickPort;

	[RPC]
	protected void QuickPort(float angle) {
		Log("QuickPort: " + angle);
		
		// Consume energy
		energy -= Config.instance.quickPortEnergyDrain;
		
		// Save time
		lastQuickPort = uLink.Network.time;
		
		// Move
		var distance = new Vector3(0, 0, Config.instance.quickPortDistance);
		characterController.Move(Quaternion.AngleAxis(angle, Vector3.up) * distance);
	}
}
