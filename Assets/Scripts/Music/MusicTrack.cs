using UnityEngine;
using System.Collections;

[System.Serializable]
public class MusicTrack {
	public AudioClip audioClip;
	public float pitchMin = 1.0f;
	public float pitchMax = 1.0f;
	public bool isFirstTrack = false;
	
	[System.NonSerialized]
	public int playCount;
}
