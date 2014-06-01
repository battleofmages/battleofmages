public static class uLinkNetworkMessageInfoExtensions {
	public static float GetPacketArrivalTime(this uLink.NetworkMessageInfo info) {
		return (float)(uLink.Network.time - info.timestamp);
	}
	
	public static double GetPacketArrivalTimeDouble(this uLink.NetworkMessageInfo info) {
		return uLink.Network.time - info.timestamp;
	}
}