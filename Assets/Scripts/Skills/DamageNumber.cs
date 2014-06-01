using UnityEngine;

public class DamageNumber : MonoBehaviour {
	public float duration = 1.8f; // time to die
	public float yOffset;
	public float scrollVelocity = 0.05f; // scrolling velocity
	
	protected float alpha = 1.0f;
	protected float offset = 0.0f;
	protected Transform myTransform;
	protected Transform targetTransform;
	protected Vector3 worldOffset;
	
	private Entity _target;
	
	// Start
	void Start() {
		// Apply alpha
		Color c = guiText.material.color;
		guiText.material.color = new Color(c.r, c.g, c.b, alpha);
		
		myTransform = this.transform;
	}
	
	// Update
	void Update() {
		if(alpha > 0){
			offset += scrollVelocity * Time.deltaTime;
			alpha -= Time.deltaTime / duration;
			
			if(targetTransform == null)
				return;
			
			Vector3 dmgNumPos = Camera.main.WorldToViewportPoint(targetTransform.position + worldOffset);
			dmgNumPos.x = Mathf.Clamp(dmgNumPos.x, 0.05f, 0.95f);
			dmgNumPos.y = Mathf.Clamp(dmgNumPos.y + offset, 0.05f, 0.90f);
			
			myTransform.position = dmgNumPos; 
			
			Color c = guiText.material.color;
			guiText.material.color = new Color(c.r, c.g, c.b, alpha);
		} else {
			Destroy(gameObject);
		}
	}
	
	// Target
	public Entity target {
		get {
			return _target;
		}
		
		set {
			_target = value;
			
			worldOffset.y = _target.height + _target.entityGUI.yOffset + yOffset;
			if(_target != null)
				targetTransform = _target.transform;
		}
	}
}
