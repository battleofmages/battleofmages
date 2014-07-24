using UnityEngine;

public static class PortalDB {
	// --------------------------------------------------------------------------------
	// AccountToPortal
	// --------------------------------------------------------------------------------
	
	// Set portal
	public static Coroutine SetPortal(string accountId, PortalInfo portalInfo, GameDB.ActionOnResult<PortalInfo> func = null) {
		return GameDB.instance.StartCoroutine(GameDB.Set<PortalInfo>(
			"AccountToPortal",
			accountId,
			portalInfo,
			func
		));
	}
	
	// Get portal
	public static Coroutine GetPortal(string accountId, GameDB.ActionOnResult<PortalInfo> func) {
		return GameDB.instance.StartCoroutine(GameDB.Get<PortalInfo>(
			"AccountToPortal",
			accountId,
			func
		));
	}

	// Remove portal
	public static Coroutine RemovePortal(string accountId, GameDB.ActionOnResult<bool> func = null) {
		return GameDB.instance.StartCoroutine(GameDB.Remove(
			"AccountToPortal",
			accountId,
			func
		));
	}
}