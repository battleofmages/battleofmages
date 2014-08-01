using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class MusicCategory : MonoBehaviour {
	public MusicTrack[] tracks;
	
	private List<MusicTrack> tracksLeftToPlay;

	// Start
	void Start() {
		if(GameManager.isServer)
			UnloadMusic();
	}

	// GetNextTrack
	public MusicTrack GetNextTrack(MusicTrack previousTrack) {
		int numOfTracksLeft;
		MusicTrack newTrack = null;
		int index = -1;
		
		// Get next track
		if(tracksLeftToPlay == null) {
			tracksLeftToPlay = new List<MusicTrack>(tracks);
			numOfTracksLeft = tracksLeftToPlay.Count;
			
			for(int i = 0; i < tracksLeftToPlay.Count; i++) {
				if(tracksLeftToPlay[i].isFirstTrack) {
					index = i;
					break;
				}
			}
		} else {
			numOfTracksLeft = tracksLeftToPlay.Count;
		}
		
		if(index == -1) {
			while(true) {
				index = Random.Range(0, numOfTracksLeft);
				
				// Don't select the same audio clip again
				if(previousTrack == null || numOfTracksLeft == 1 || tracksLeftToPlay[index].audioClip != previousTrack.audioClip)
					break;
			}
		}
		
		newTrack = tracksLeftToPlay[index];
		newTrack.playCount += 1;
		
		// Remove picked track from the "left to play" list
		if(tracksLeftToPlay.Count > 1) {
			tracksLeftToPlay.RemoveAt(index);
		} else {
			// Refill playlist
			tracksLeftToPlay = new List<MusicTrack>(tracks);
		}
		
		return newTrack;
	}

	// UnloadMusic
	void UnloadMusic() {
		foreach(var track in tracks) {
			track.audioClip = null;
		}

		tracks = null;
		Resources.UnloadUnusedAssets();
	}
}
