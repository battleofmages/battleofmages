using UnityEngine;

[RequireComponent(typeof(AudioSource))]

public class Sounds : SingletonMonoBehaviour<Sounds> {
	public AudioClip toggleTargetFocus;
	public AudioClip buttonClick;
	public AudioClip queueMatchFound;
	public AudioClip queueMatchCanceled;
	public AudioClip loginSuccess;
	public AudioClip loginFail;
	public AudioClip menuBack;
	public AudioClip menuNavigate;
}
