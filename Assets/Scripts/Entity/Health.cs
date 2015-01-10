using UnityEngine;
using UnityEngine.UI;

public class Health : GenericSlider {
	public GameObject healthBarPrefab;

	[System.NonSerialized]
	public GameObject bar;

	// UI
	protected UnityEngine.UI.Slider slider;

	// Start
	void Start() {
		bar = (GameObject)Instantiate(healthBarPrefab);

		slider = bar.GetComponent<UnityEngine.UI.Slider>();
		slider.minValue = 0f;
		slider.maxValue = max;
		slider.value = current;

		bar.transform.SetParent(Root.instance.hud, false);
	}

	// Update
	void Update() {
		slider.value = current;
		bar.transform.position = Camera.main.WorldToScreenPoint(transform.position);
	}
}