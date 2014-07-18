using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]

public class Sounds : SingletonMonoBehaviour<Sounds> {
	public static AudioClip toggleTargetFocus;
	
	public AudioClip toggleTargetFocusSound;
	public AudioClip buttonClick;
	public AudioClip queueMatchFound;
	public AudioClip loginSuccess;
	public AudioClip loginFail;
	public AudioClip menuBack;
	public AudioClip menuNavigate;
	
	void Start() {
		toggleTargetFocus = toggleTargetFocusSound;
	}
	
	public void PlayButtonClick() {
		audio.PlayOneShot(buttonClick);
	}

	public void PlayMenuBack() {
		audio.PlayOneShot(menuBack);
	}

	public void PlayMenuNavigate() {
		audio.PlayOneShot(menuNavigate);
	}

	public void PlayLoginSuccess() {
		audio.PlayOneShot(loginSuccess);
	}

	public void PlayLoginFail() {
		audio.PlayOneShot(loginFail);
	}
	
	public void PlayQueueMatchFound() {
		audio.PlayOneShot(queueMatchFound);
	}
	
	public void PlayQueueMatchCanceled() {
		// TODO: Add sound
		//audio.PlayOneShot(queueMatchFound);
	}
}
