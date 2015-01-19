using UnityEngine;
using System;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]

public class Sounds : SingletonMonoBehaviour<Sounds>, Initializable {
	[Serializable]
	public struct SoundPoolEntry {
		public string id;
		public AudioClip clip;
	}

	public SoundPoolEntry[] soundPool;
	private AudioSource audioSource;
	private Dictionary<string, AudioClip> idToClip = new Dictionary<string, AudioClip>();

	// Init
	public void Init() {
		audioSource = GetComponent<AudioSource>();

		// Build dictionary
		foreach(var entry in soundPool) {
			idToClip[entry.id] = entry.clip;
		}
	}

	// Play
	public void Play(AudioClip clip) {
		// This prevents sounds from playing until everything is initialized
		if(!StartUp.finished)
			return;

		audioSource.PlayOneShot(clip);
	}

	// Play
	public void Play(string id) {
		Play(idToClip[id]);
	}
}