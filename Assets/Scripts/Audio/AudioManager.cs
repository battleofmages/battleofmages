using UnityEngine;
using UnityEngine.Audio;
using BoM.UI.Lobby;

public class AudioManager : SingletonMonoBehaviour<AudioManager>, Initializable {
	public AudioMixer mixer;

	// Init
	public void Init() {
		AudioListener.volume = PlayerPrefs.GetFloat("Audio_MasterVolume", 1f);

		// Login: Deactivate low pass
		Login.instance.onLogIn += (account) => {
			this.Fade(
				4f,
				(val) => {
					mixer.SetFloat("musicCutOffFreq", 500f + 21500f * val * val);
				}
			);
		};

		// Logout: Activate low pass
		Login.instance.onLogOut += (account) => {
			this.Fade(
				2f,
				(val) => {
					val = 1f - val;
					mixer.SetFloat("musicCutOffFreq", 500f + 21500f * val * val);
				}
			);
		};
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
