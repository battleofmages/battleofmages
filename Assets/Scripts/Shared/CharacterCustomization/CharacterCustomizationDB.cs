using UnityEngine;

public static class CharacterCustomizationDB {
	// --------------------------------------------------------------------------------
	// AccountToCharacterCustomization
	// --------------------------------------------------------------------------------
	
	// Get character customization
	public static Coroutine GetCharacterCustomization(string accountId, GameDB.ActionOnResult<CharacterCustomization> func) {
		return GameDB.instance.StartCoroutine(GameDB.Get<CharacterCustomization>(
			"AccountToCharacterCustomization",
			accountId,
			func
		));
	}
	
	// Set character customization
	public static void SetCharacterCustomization(string accountId, CharacterCustomization custom, GameDB.ActionOnResult<CharacterCustomization> func = null) {
		GameDB.instance.StartCoroutine(GameDB.Set<CharacterCustomization>(
			"AccountToCharacterCustomization",
			accountId,
			custom,
			func
		));
	}
}
