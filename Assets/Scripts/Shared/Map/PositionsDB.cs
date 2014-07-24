using UnityEngine;

public static class PositionsDB {
	// --------------------------------------------------------------------------------
	// AccountToPosition
	// --------------------------------------------------------------------------------
	
	// Set position
	public static Coroutine SetPosition(string accountId, Vector3 position, GameDB.ActionOnResult<Vector3> func = null) {
		return GameDB.instance.StartCoroutine(GameDB.Set<Vector3>(
			"AccountToPosition",
			accountId,
			position,
			func
		));
	}
	
	// Get position
	public static Coroutine GetPosition(string accountId, GameDB.ActionOnResult<Vector3> func) {
		return GameDB.instance.StartCoroutine(GameDB.Get<Vector3>(
			"AccountToPosition",
			accountId,
			func
		));
	}
}