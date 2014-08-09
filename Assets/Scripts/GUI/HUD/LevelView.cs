using UnityEngine;

public class LevelView : HUDElement {
	public GUIStyle levelStyle;
	private string level;

	// Start
	void Start() {
		InvokeRepeating("UpdateLevel", 0.001f, 0.1f);
	}

	// UpdateLevel
	void UpdateLevel() {
		var account = PlayerAccount.mine;
		
		if(account == null)
			return;

		level = "Level <b>" + (int)account.level + "</b>";
	}

	// Draw
	public override void Draw() {
		GUILayout.Label(level, levelStyle);
	}
}