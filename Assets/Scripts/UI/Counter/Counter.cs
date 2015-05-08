using UnityEngine;
using UnityEngine.UI;

public class Counter : MonoBehaviour {
	public Text textComponent;
	protected int counter = 0;
	private string format;
	private Color defaultColor;
	
	[System.Serializable]
	public struct LimitColor {
		public int limit;
		public Color color;
	}
	
	public LimitColor[] colors;
	
	// Start
	void Start() {
		format = textComponent.text;
		defaultColor = textComponent.color;
		
		InvokeRepeating("UpdateCounter", 0.1f, 1.0f);
	}
	
	// UpdateCounter
	void UpdateCounter() {
		// Text
		textComponent.text = format.Replace("{fps}", counter.ToString());
		
		// Color
		UpdateColor();
		
		// Reset counter
		counter = 0;
	}
	
	// UpdateColor
	void UpdateColor() {
		foreach(var limit in colors) {
			if(counter >= limit.limit)
				continue;
			
			textComponent.color = limit.color;
			return;
		}
		
		textComponent.color = defaultColor;
	}
}
