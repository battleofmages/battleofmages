using UnityEngine;

public static class PositionsDB {
	// --------------------------------------------------------------------------------
	// AccountToPosition
	// --------------------------------------------------------------------------------
	
	// Set position
	public static Coroutine SetPosition(string accountId, PlayerPosition position, GameDB.ActionOnResult<PlayerPosition> func = null) {
		return GameDB.instance.StartCoroutine(GameDB.Set<PlayerPosition>(
			"AccountToPosition",
			accountId,
			position,
			func
		));
	}

	// Set position
	public static Coroutine SetPosition(string accountId, Vector3 vec, GameDB.ActionOnResult<PlayerPosition> func = null) {
		return GameDB.instance.StartCoroutine(GameDB.Set<PlayerPosition>(
			"AccountToPosition",
			accountId,
			new PlayerPosition(vec),
			func
		));
	}

	// Get position
	public static Coroutine GetPosition(string accountId, GameDB.ActionOnResult<PlayerPosition> func) {
		return GameDB.instance.StartCoroutine(GameDB.Get<PlayerPosition>(
			"AccountToPosition",
			accountId,
			func
		));
	}
}