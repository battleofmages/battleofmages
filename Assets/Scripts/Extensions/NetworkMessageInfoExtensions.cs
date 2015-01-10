static class NetworkMessageInfoExtensions {
	// GetPacketArrivalTime
	public static float GetPacketArrivalTime(this uLink.NetworkMessageInfo info) {
		return (float)(uLink.Network.time - info.timestamp);
	}

	// GetPacketArrivalTimeDouble
	public static double GetPacketArrivalTimeDouble(this uLink.NetworkMessageInfo info) {
		return uLink.Network.time - info.timestamp;
	}
}