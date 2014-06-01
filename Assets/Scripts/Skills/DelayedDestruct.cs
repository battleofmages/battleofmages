public class DelayedDestruct : SkillInstance {
	public float duration;
	
	// Start
	void Start() {
		Destroy(gameObject, duration);
	}
}
