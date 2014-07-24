using UnityEngine;

public class PlayerLocation : JsonSerializable<PlayerLocation> {
	public string mapName;
	public ServerType serverType;
	
	// For reconnecting
	public string ip;
	public int port;
	
	// Empty constructor
	public PlayerLocation() {
		// ...
	}
	
	// Constructor
	public PlayerLocation(string nMapName, ServerType nServerType) {
		mapName = nMapName;
		serverType = nServerType;
	}
}
