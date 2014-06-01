using UnityEngine;

public class CopyRotationY : MonoBehaviour {
	private Transform source;
	private Transform myTransform;

	// Start
	void Start() {
		source = GetComponent<SkillInstance>().caster.charGraphicsBody;
		myTransform = transform;
	}
	
	// Update
	void Update() {
		myTransform.eulerAngles = new Vector3(0, source.eulerAngles.y, 0);
	}
}
