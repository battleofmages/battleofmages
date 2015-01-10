using UnityEngine;

static class AudioClipExtensions {
	// Play
	public static void Play(this AudioClip clip) {
		if(clip == null)
			return;
		
		Sounds.instance.Play(clip);
	}
}