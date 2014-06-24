using UnityEngine;

public class EntityGUI : MonoBehaviour {
	public float yOffset;
	public Texture healthContainerTex;
	public Texture healthTex;
	public Texture energyTex;
	
	protected Entity entity;
	protected Camera cam;
	protected EntityLabel nameLabel;
	
	// Start
	void Start() {
		cam = Camera.main;
		entity = GetComponent<Entity>();
		nameLabel = entity.nameLabel;
	}
	
	// Draw
	public void Draw() {
		// Name label height
		nameLabel.offset.y = entity.height + yOffset;//entity.charGraphicsBody.renderer.bounds.size.y + 0.85f;
		
		// Health bar
		Vector3 healthBarPos = entity.myTransform.position;
		healthBarPos.y += entity.nameLabel.offset.y;
		Vector3 screenPosition = cam.WorldToScreenPoint(healthBarPos);
		
		// Invert y
		screenPosition.y = Screen.height - (screenPosition.y + 1);
		
		var healthBarWidth = entity.healthBarWidth;
		
		// Container
		Rect healthRect = new Rect(screenPosition.x - healthBarWidth / 2 - 1, screenPosition.y - 14, healthBarWidth + 2, 6);
		Rect blockRect = new Rect(screenPosition.x - healthBarWidth / 2 - 1, screenPosition.y - 14 + 7, healthBarWidth + 2, 6);
		
		GUI.DrawTexture(healthRect, healthContainerTex, ScaleMode.StretchToFill, true);

		if(entity.blockingEnabled)
			GUI.DrawTexture(blockRect, healthContainerTex, ScaleMode.StretchToFill, true);
		
		// Fill the bars
		Rect healthFillRect = new Rect(healthRect.x + 1, healthRect.y + 1, entity.curHealthBarWidth, healthRect.height - 2);
		Rect blockFillRect = new Rect(blockRect.x + 1, blockRect.y + 1, (blockRect.width - 2) * entity.energyRatio, blockRect.height - 2);
		
		GUI.DrawTexture(healthFillRect, healthTex, ScaleMode.StretchToFill, true);

		if(entity.blockingEnabled)
			GUI.DrawTexture(blockFillRect, energyTex, ScaleMode.StretchToFill, true);
	}
}
