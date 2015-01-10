using UnityEngine;
using UnityEngine.UI;

public class HueEffect : MonoBehaviour {
	public RawImage image;
	public HSBColor color = new HSBColor(0f, 1f, 1f, 1f);

	// Start
	void Start() {
		
	}
	
	// Update
	void Update() {
		color.hue += Time.deltaTime;

		if(color.hue > 1f)
			color.hue = 0f;

		image.color = color.ToColor();
	}
}
