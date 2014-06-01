using System.Collections;

public static class SkillBuildsDB {
	// --------------------------------------------------------------------------------
	// AccountToSkillBuild
	// --------------------------------------------------------------------------------
	
	// Get skill build
	public static void GetSkillBuild(string accountId, GameDB.ActionOnResult<SkillBuild> func) {
		GameDB.instance.StartCoroutine(GameDB.Get<SkillBuild>(
			"AccountToSkillBuild",
			accountId,
			func
		));
	}
	
	// Set skill build
	public static void SetSkillBuild(string accountId, SkillBuild skillBuild, GameDB.ActionOnResult<SkillBuild> func = null) {
		GameDB.instance.StartCoroutine(GameDB.Set<SkillBuild>(
			"AccountToSkillBuild",
			accountId,
			skillBuild,
			func
		));
	}
}
