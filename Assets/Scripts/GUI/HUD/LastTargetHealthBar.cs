using System;

public class LastTargetHealthBar : HUDElement {
	public ProgressBarStyle healthBar;

	[NonSerialized]
	public Entity entityLastHit;

	// Draw
	public override void Draw() {
		entityLastHit = Player.main.entityLastHit;

		// Check if we got a last target
		if(Player.main == null || Player.main.entityLastHit == null) 
			return;

		// Check if the player is alive
		if(!Player.main.entityLastHit.isAlive) 
			return;

		//Draw the health bar
		GUIHelper.ProgressBar(entityLastHit.name, entityLastHit.healthRatio, null, healthBar);
	}
}