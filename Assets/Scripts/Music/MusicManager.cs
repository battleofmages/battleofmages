#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MusicManager : LobbyModule<MusicManager> {
	public bool musicEnabled;

	public AnimationCurve fadeInCurve;
	public AnimationCurve fadeOutCurve;
	public float fadeDuration;
	
	public Texture2D playIcon;
	public Texture2D pauseIcon;
	public Texture2D stopIcon;
	public Texture2D skipIcon;
	public GUIStyle trackNameStyle;
	
	public GUIStyle spectrumBandLevelStyle;
	public int rawSpectrumLength;
	public int bandLevelsCount;
	
	public MusicCategory[] categories;
	
	private float _volume = -1f;
	private Dictionary<string, MusicCategory> categoryDict;
	private AudioSource[] audioSources;
	private AudioSource currentAudioSource;
	private MusicCategory currentCategory;
	private MusicTrack currentTrack;
	private bool paused;
	
	private AudioSource fadeOutSource;
	private AudioSource fadeInSource;
	private float fadeTime;
	
	private float[] rawSpectrum;
	private float[] bandLevels;
	private float[] meter;

	// Start
	void Start() {
		categoryDict = new Dictionary<string, MusicCategory>();
		foreach(var category in categories) {
			categoryDict[category.name] = category;
		}
		
		audioSources = new AudioSource[2];
		audioSources[0] = transform.GetChild(0).GetComponent<AudioSource>();
		audioSources[1] = transform.GetChild(1).GetComponent<AudioSource>();
		
		currentAudioSource = audioSources[0];
		
		currentTrack = null;
		fadeOutSource = null;
		fadeInSource = null;
		
		volume = PlayerPrefs.GetFloat("Audio_MusicVolume", 1f);
		paused = false;
		
		rawSpectrum = new float[rawSpectrumLength];
		bandLevels = new float[bandLevelsCount];
		meter = new float[bandLevels.Length];
	}

	// PlayCategory
	public void PlayCategory(string categoryName) {
		if(!musicEnabled)
			return;

		if(!categoryDict.ContainsKey(categoryName)) {
			LogManager.General.LogWarning("Music category '" + categoryName + "' does not exist");
			return;
		}
		
		currentCategory = categoryDict[categoryName];
		
		if(currentAudioSource.isPlaying) {
			CrossFadeNextClip();
		} else {
			PlayNextClip();
		}
		
		// We have to reset the paused state
		paused = false;
	}

	// Draw
	public override void Draw() {
		using(new GUIVerticalCenter()) {
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.BeginVertical("box");
			
			// 1st line: Track name
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			if(currentTrack != null)
				GUILayout.Label(currentTrack.audioClip.name, trackNameStyle);
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			
			// 2nd line: Controls
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			
			if(paused) {
				if(GUIHelper.Button(new GUIContent(playIcon))) {
					currentAudioSource.Play();
					paused = false;
				}
			} else {
				if(GUIHelper.Button(new GUIContent(pauseIcon))) {
					currentAudioSource.Pause();
					paused = true;
				}
			}
			
			if(GUIHelper.Button(new GUIContent(skipIcon))) {
				PlayNextClip();
			}
			
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			
			GUILayout.EndVertical();
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			
			// 3rd line: Spectrum
			using(new GUIHorizontalCenter()) {
				using(new GUIHorizontal("box")) {
					AudioListener.GetSpectrumData(rawSpectrum, 0, FFTWindow.BlackmanHarris);
					ConvertRawSpectrumToBandLevels();
					
					float multiplier = 512.0f;
					int maxHeight = 128;
					int height;
					
					for(int i = 0; i < meter.Length; i++) {
						meter[i] = Mathf.Max(meter[i] * Mathf.Exp(-10.0f * Time.deltaTime), bandLevels[i]);
						height = (int)(meter[i] * multiplier);
						
						if(height > maxHeight)
							height = maxHeight;
						
						GUILayout.BeginVertical();
						GUILayout.Space(maxHeight - height);
						GUILayout.Box("", spectrumBandLevelStyle, GUILayout.Height(height));
						GUILayout.EndVertical();
						//GUILayout.Space(1);
					}
				}
			}
		}
	}
	
	// On coming close to the NPC
	public override void OnNPCEnter() {
		InGameLobby.instance.currentLobbyModule = this;
	}

	// ConvertRawSpectrumToBandLevels
	void ConvertRawSpectrumToBandLevels() {
		var coeff = Mathf.Log(rawSpectrum.Length);
		var offs = 0;
		for(var i = 0; i < bandLevels.Length; i++) {
			var next = Mathf.Exp(coeff / bandLevels.Length * (i + 1));
			var weight = 1.0f / (next - offs);
			var sum = 0.0f;
			for(; offs < next; offs++) {
				sum += rawSpectrum[offs];
			}
			bandLevels[i] = Mathf.Sqrt(weight * sum);
		}
	}

	// FixedUpdate
	void FixedUpdate() {
		if(paused)
			return;
		
		if(isFading) {
			fadeTime += Time.fixedDeltaTime / fadeDuration;
			fadeOutSource.volume = fadeOutCurve.Evaluate(fadeTime) * _volume;
			fadeInSource.volume = fadeInCurve.Evaluate(fadeTime) * _volume;
			
			//LogManager.General.Log(currentAudioSource.time + " of " + currentAudioSource.clip.length + " FADING: " + fadeOutSource.volume + " to " + fadeInSource.volume);
			
			if(fadeTime > 1.0f) {
				StopFading();
			}
		} else if(currentAudioSource.isPlaying) {
			//LogManager.General.Log(currentAudioSource.time + " of " + currentAudioSource.clip.length + " [" + currentCategory.tracks.Length);
			if(currentAudioSource.time >= currentAudioSource.clip.length - fadeDuration) {
				CrossFadeNextClip();
			}
		} else if(currentCategory != null) {
			PlayNextClip();
		}
	}

	// CrossFadeNextClip
	void CrossFadeNextClip() {
		fadeTime = 0.0f;
		fadeOutSource = currentAudioSource;
		fadeInSource = (currentAudioSource == audioSources[0] ? audioSources[1] : audioSources[0]);
		fadeInSource.volume = fadeInCurve.Evaluate(fadeTime) * _volume;
		
		currentAudioSource = fadeInSource;
		PlayNextClip();
	}

	// StopFading
	void StopFading() {
		fadeOutSource.Stop();
		
		fadeOutSource = null;
		fadeInSource = null;
	}

	// PlayNextClip
	void PlayNextClip() {
		var nextTrack = currentCategory.GetNextTrack(currentTrack);
		currentAudioSource.clip = nextTrack.audioClip;
		currentAudioSource.pitch = Random.Range(nextTrack.pitchMin, nextTrack.pitchMax);
		currentAudioSource.Play();
		
		currentTrack = nextTrack;
	}
	
#if UNITY_EDITOR
	[MenuItem("Battle of Mages/Music/Play Next Clip")]
	static void StaticPlayNextClip() {
		MusicManager.instance.PlayNextClip();
	}
#endif

	// Is fading
	public bool isFading {
		get { return fadeOutSource != null && fadeInSource != null; }
	}

	// Volume
	public float volume {
		get {
			return _volume;
		}
		
		set {
			if(_volume == value)
				return;
			
			_volume = value;
			
			if(!isFading) {
				foreach(var audioSource in audioSources) {
					audioSource.volume = _volume;
				}
			}
		}
	}
}
