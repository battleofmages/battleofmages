using UnityEngine;

public static class ExperienceDB {
	// --------------------------------------------------------------------------------
	// AccountToExperience
	// --------------------------------------------------------------------------------
	
	// Set experience
	public static Coroutine SetExperience(string accountId, ExperienceEntry entry, GameDB.ActionOnResult<ExperienceEntry> func = null) {
		return GameDB.instance.StartCoroutine(GameDB.Set<ExperienceEntry>(
			"AccountToExperience",
			accountId,
			entry,
			func
		));
	}
	
	// Set experience
	public static Coroutine SetExperience(string accountId, uint exp, GameDB.ActionOnResult<ExperienceEntry> func = null) {
		return GameDB.instance.StartCoroutine(GameDB.Set<ExperienceEntry>(
			"AccountToExperience",
			accountId,
			new ExperienceEntry(exp),
			func
		));
	}
	
	// Get experience
	public static Coroutine GetExperience(string accountId, GameDB.ActionOnResult<ExperienceEntry> func) {
		return GameDB.instance.StartCoroutine(GameDB.Get<ExperienceEntry>(
			"AccountToExperience",
			accountId,
			func
		));
	}
}