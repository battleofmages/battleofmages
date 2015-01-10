using UnityEngine;

public class Notification : MonoBehaviour {
	public const float fadeOutTime = 0.5f;

	[System.NonSerialized]
	public float duration;

	// Start
	void Start() {
		if(duration != 0f)
			Invoke("Remove", duration);
	}

	// Remove
	public void Remove() {
		var canvasGroup = GetComponent<CanvasGroup>();

		this.Fade(
			fadeOutTime,
			(val) => {
				canvasGroup.alpha = 1f - val;
			},
			() => {
				Destroy(this.gameObject);
			}
		);
	}
}
