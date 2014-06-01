using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]

public class Sounds : SingletonMonoBehaviour<Sounds> {
	public static AudioClip toggleTargetFocus;
	
	public AudioClip toggleTargetFocusSound;
	public AudioClip buttonClick;
	public AudioClip queueMatchFound;
	
	void Start() {
		toggleTargetFocus = toggleTargetFocusSound;
	}
	
	public void PlayButtonClick() {
		audio.PlayOneShot(buttonClick);
	}
	
	public void PlayQueueMatchFound() {
		audio.PlayOneShot(queueMatchFound);
	}
	
	public void PlayQueueMatchCanceled() {
		// TODO: Add sound
		//audio.PlayOneShot(queueMatchFound);
	}
}
