[System.Serializable]
public class Cooldown {
	public float cooldown;
	
	[System.NonSerialized]
	public float originalCooldown;
	
	[System.NonSerialized]
	public double lastUse;

	// Is on cooldown
	public bool isOnCooldown {
		get {
			return uLink.Network.time - lastUse < cooldown;
		}
	}

	// Cooldown remaining
	public double cooldownRemaining {
		get {
			return cooldown - (uLink.Network.time - lastUse);
		}
	}

	// Cooldown remaining as a float between 0 and 1
	public float cooldownRemainingRelative {
		get {
			return (float)(1.0d - (uLink.Network.time - lastUse) / cooldown);
		}
	}
}
