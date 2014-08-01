using UnityEngine;

public abstract class TutorialScript : MonoBehaviour, Drawable {
	protected bool draw;
	
	// Start
	void Start() {
		if(GameManager.isServer)
			enabled = false;
	}
	
	// OnTriggerEnter
	void OnTriggerEnter(Collider other) {
		if(other.GetComponent<PlayerMain>() != null)
			draw = true;
	}
	
	// OnTriggerExit
	void OnTriggerExit(Collider other) {
		if(other.GetComponent<PlayerMain>() != null)
			draw = false;
	}
	
	// OnGUI
	void OnGUI() {
		if(!draw)
			return;

		using(new GUIArea(new Rect(16, 48, 500, 500))) {
			Draw();
		}
	}

	// Draw
	public virtual void Draw() {
		// ...
	}
}