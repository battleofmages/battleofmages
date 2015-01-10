using UnityEngine;
using UnityEngine.UI;

public class LoadMasterVolume : MonoBehaviour, Initializable {
	// Init
	public void Init() {
		var slider = GetComponent<Slider>();
		slider.value = AudioListener.volume;
	}
}
