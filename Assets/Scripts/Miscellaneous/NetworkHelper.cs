public static class NetworkHelper {
	// Disable network emulation
	public static void DisableNetworkEmulation() {
		uLink.Network.emulation.minLatency = 0;
		uLink.Network.emulation.maxLatency = 0;
		uLink.Network.emulation.maxBandwidth = 0;
		uLink.Network.emulation.chanceOfDuplicates = 0;
		uLink.Network.emulation.chanceOfLoss = 0;
	}
}
