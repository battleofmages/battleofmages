using UnityEngine;

public class LootTrail : MonoBehaviour {
	public AnimationCurve yCurve;
	public Entity target;
	public float duration;
	private Vector3 startPosition;
	private float time;
	private Vector3 lerpVector;

	// Start
	void Start() {
		startPosition = transform.position;
		Destroy(gameObject, duration);
	}

	// Update
	void Update() {
		if(target == null)
			return;

		time += Time.deltaTime / duration;

		lerpVector = Vector3.Lerp(startPosition, target.center, time);
		lerpVector.y += yCurve.Evaluate(time);

		transform.position = lerpVector;
	}
}
