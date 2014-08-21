using UnityEngine;
using UnityEngine.UI;

public class AudioVolumeIcon : MonoBehaviour {
	public Sprite full;
	public Sprite low;
	public Sprite none;
	
	// UpdateSprite
	public void UpdateSprite(float value) {
		var img = GetComponent<Image>();

		if(value >= 0.5f)
			img.sprite = full;
		else if(value > 0f)
			img.sprite = low;
		else
			img.sprite = none;
	}
}
