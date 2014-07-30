public class PortalInfo : JsonSerializable<PortalInfo> {
	public string mapName;
	
	// Empty constructor
	public PortalInfo() {
		
	}
	
	// Constructor
	public PortalInfo(string nMapName) {
		mapName = nMapName;
	}
}