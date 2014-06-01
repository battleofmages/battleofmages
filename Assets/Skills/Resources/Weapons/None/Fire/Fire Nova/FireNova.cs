using UnityEngine;

public class FireNova : SkillInstance {
	// Start
	public void Start() {
		Destroy(this.gameObject, 3.0f);
	}
}
