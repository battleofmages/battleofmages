using UnityEngine;

public class EntityGUI : MonoBehaviour {
	public float yOffset;
	public Texture healthContainerTex;
	public Texture healthTex;
	public Texture energyTex;

	protected Rect healthRect;
	protected Rect blockRect;

	protected Rect healthFillRect;
	protected Rect blockFillRect;

	protected Vector3 healthBarPos;
	protected Vector3 screenPosition;
	
	protected Entity entity;
	protected Camera cam;
	protected ObjectLabel nameLabel;
	
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
		healthBarPos = entity.myTransform.position;
		healthBarPos.y += entity.nameLabel.offset.y;
		screenPosition = cam.WorldToScreenPoint(healthBarPos);
		
		// Invert y
		screenPosition.y = Screen.height - (screenPosition.y + 1);
		
		var healthBarWidth = entity.healthBarWidth;
		
		// Container
		healthRect = new Rect(screenPosition.x - healthBarWidth / 2 - 1, screenPosition.y - 14, healthBarWidth + 2, 6);
		blockRect = new Rect(screenPosition.x - healthBarWidth / 2 - 1, screenPosition.y - 14 + 7, healthBarWidth + 2, 6);
		
		GUI.DrawTexture(healthRect, healthContainerTex, ScaleMode.StretchToFill, true);

		if(entity.blockingEnabled)
			GUI.DrawTexture(blockRect, healthContainerTex, ScaleMode.StretchToFill, true);
		
		// Fill the bars
		healthFillRect = new Rect(healthRect.x + 1, healthRect.y + 1, entity.curHealthBarWidth, healthRect.height - 2);
		blockFillRect = new Rect(blockRect.x + 1, blockRect.y + 1, (blockRect.width - 2) * entity.energyRatio, blockRect.height - 2);
		
		GUI.DrawTexture(healthFillRect, healthTex, ScaleMode.StretchToFill, true);

		if(entity.blockingEnabled)
			GUI.DrawTexture(blockFillRect, energyTex, ScaleMode.StretchToFill, true);
	}
}
