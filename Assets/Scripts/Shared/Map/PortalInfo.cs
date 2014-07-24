public class PortalInfo : JsonSerializable<PortalInfo> {
	public int id;
	
	// Empty constructor
	public PortalInfo() {
		
	}
	
	// Constructor
	public PortalInfo(int portalId) {
		id = portalId;
	}
}