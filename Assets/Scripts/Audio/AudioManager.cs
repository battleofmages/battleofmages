using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : SingletonMonoBehaviour<AudioManager>, Initializable {
	public AudioMixer mixer;

	// Init
	public void Init() {
		AudioListener.volume = PlayerPrefs.GetFloat("Audio_MasterVolume", 1f);
	}

	// Master volume
	public float masterVolume {
		get {
			return AudioListener.volume;
		}

		set {
			AudioListener.volume = value;
			PlayerPrefs.SetFloat("Audio_MasterVolume", AudioListener.volume);
		}
	}
}
